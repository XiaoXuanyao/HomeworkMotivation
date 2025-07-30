using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// 时钟组件，提供计时功能和可视化显示。
/// 该组件可以在 Unity 编辑器中使用，支持开始、结束、重置计时
/// </summary>
[ExecuteInEditMode]
public class Clock : MonoBehaviour
{

    public float hours;
    public float minutes;
    public float seconds;
    public float milliseconds;
    public Color hoursCircleColor;
    public Color minutesCircleColor;
    public Color secondsCircleColor;
    public Color coreColor;
    public Color hoursPointerColor;
    public Color minutesPointerColor;
    public Color secondsPointerColor;
    public Color backgroundColor = Color.black;
    public Color textColor = Color.white;
    public GameObject startButton;
    public GameObject stopButton;


    public System.DateTime startTime;
    public System.DateTime endTime;
    public bool isTiming = false;


    private bool isLoaded = false;
    private GameObject clockObject;


    /// <summary>
    /// 当组件参数发生改变时，更新并载入时钟颜色等设置。
    /// </summary>
    private void OnValidate()
    {
        if (GetComponents<Clock>().Length > 1 && !isLoaded)
        {
            Debug.LogWarning("Clock component should not be added to GameObject "
                + "already containing a Clock component.");
            Invoke(nameof(DestroyThis), 0.01f);
            return;
        }
        if (!isLoaded)
        {
            Invoke("InitClock", 0.01f);
            isLoaded = true;
        }
        else
        {
            Invoke("UpdateArgs", 0.01f);
        }
    }

    /// <summary>
    /// 销毁时钟组件时，释放按钮事件监听器并销毁时钟对象。
    /// </summary>
    private void OnDestroy()
    {
        ReleaseButtons();
        DestroyImmediate(clockObject, true);
    }

    /// <summary>
    /// 每帧更新时钟显示。
    /// 如果正在计时，则计算当前时间与开始时间的差值。
    /// 更新时钟的外圈、指针、文字等UI。
    /// </summary>
    private void Update()
    {
        if (clockObject == null)
            return;

        if (isTiming)
            endTime = System.DateTime.Now;

        // 计算时间差
        System.TimeSpan duration = endTime - startTime;
        hours = duration.Hours;
        minutes = duration.Minutes;
        seconds = duration.Seconds;
        milliseconds = duration.Milliseconds;

        // 更新时钟外圈与指针
        Transform circles = clockObject.transform.Find("Content/Circles");
        Transform pointers = clockObject.transform.Find("Content/Pointers");

        circles.Find("Hour").GetComponent<Image>().fillAmount
            = ((hours >= 12 ? hours - 12 : hours) + (minutes / 60f)) / 12f;
        circles.Find("Minute").GetComponent<Image>().fillAmount
            = (minutes + seconds / 60f) / 60f;
        circles.Find("Second").GetComponent<Image>().fillAmount
            = (seconds + milliseconds / 1000f) / 60f;
        pointers.Find("Hour").localRotation = Quaternion.Euler(
            0, 0, -(hours + minutes / 60f) / 12f * 360f
        );
        pointers.Find("Minute").localRotation = Quaternion.Euler(
            0, 0, -(minutes + seconds / 60f) / 60f * 360f
        );
        pointers.Find("Second").localRotation = Quaternion.Euler(
            0, 0, -(seconds + milliseconds / 1000f) / 60f * 360f
        );

        // 更新时钟文字显示
        string text = "";
        if (hours > 0)
            text += $"{hours:0} 时 ";
        if (minutes > 0 || text.Length > 0)
            text += $"{minutes:0} 分 ";
        if (hours == 0)
            text += $"{seconds:0} 秒 ";
        clockObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// 生成一个随机颜色。
    /// 该颜色的 RGB 分量均在 [0, 1] 范围内，Alpha 分量为 1。
    /// 用于随机化时钟的颜色设置。
    /// </summary>
    /// <returns></returns>
    public Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }

    /// <summary>
    /// 初始化时钟对象及其相关设置。
    /// 会为时钟随机颜色。
    /// </summary>
    private void InitClock()
    {
        foreach (Transform obj in transform)
        {
            DestroyImmediate(obj.gameObject, true);
        }
        if (clockObject == null)
            clockObject = Resources.Load<GameObject>("Prefabs/Clock");
        if (clockObject == null)
        {
            Debug.LogError("Clock prefab not found in Resources/Prefabs!");
            return;
        }
        clockObject = Instantiate(clockObject, transform);
        clockObject.name = "ClockInstance";
        hoursCircleColor = RandomColor();
        minutesCircleColor = RandomColor();
        secondsCircleColor = RandomColor();
        coreColor = RandomColor();
        hoursPointerColor = RandomColor();
        minutesPointerColor = RandomColor();
        secondsPointerColor = RandomColor();
        startButton = clockObject.transform.Find("Start").gameObject;
        stopButton = clockObject.transform.Find("Stop").gameObject;
        UpdateArgs();
        BindButtons();
    }
    
    /// <summary>
    /// 销毁当前组件。
    /// 由于 Unity 编辑器的限制，不能直接在 OnValidate 中销毁组件，
    /// 因此使用 Invoke 延迟调用 DestroyImmediate。
    /// </summary>
    private void DestroyThis()
    {
        DestroyImmediate(this, true);
    }

    /// <summary>
    /// 更新时钟的颜色和其他参数。
    /// 该方法会在 OnValidate 中调用。
    /// 会调用 Update() 方法以更新时钟的显示。
    /// </summary>
    private void UpdateArgs()
    {
        Transform circles = clockObject.transform.Find("Content/Circles");
        Transform pointers = clockObject.transform.Find("Content/Pointers");

        circles.Find("Hour").GetComponent<Image>().color = hoursCircleColor;
        circles.Find("Minute").GetComponent<Image>().color = minutesCircleColor;
        circles.Find("Second").GetComponent<Image>().color = secondsCircleColor;
        circles.Find("Hour/Background").GetComponent<Image>().color = backgroundColor;
        circles.Find("Minute/Background").GetComponent<Image>().color = backgroundColor;
        circles.Find("Second/Background").GetComponent<Image>().color = backgroundColor;
        circles.Find("Core").GetComponent<Image>().color = coreColor;
        pointers.Find("Hour").GetComponent<Image>().color = hoursPointerColor;
        pointers.Find("Minute").GetComponent<Image>().color = minutesPointerColor;
        pointers.Find("Second").GetComponent<Image>().color = secondsPointerColor;
        clockObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = textColor;

        Update();
    }

    /// <summary>
    /// 为时钟绑定按钮事件。
    /// </summary>
    private void BindButtons()
    {
        ReleaseButtons();
        if (startButton != null)
        {
            Button startBtn = startButton.GetComponent<Button>();
            if (startBtn != null)
                startBtn.onClick.AddListener(StartTiming);
        }
        if (stopButton != null)
        {
            Button stopBtn = stopButton.GetComponent<Button>();
            if (stopBtn != null)
                stopBtn.onClick.AddListener(StopTiming);
        }
    }

    /// <summary>
    /// 释放时钟绑定的按钮事件。
    /// </summary>
    private void ReleaseButtons()
    {
        if (startButton != null)
        {
            Button startBtn = startButton.GetComponent<Button>();
            if (startBtn != null)
                startBtn.onClick.RemoveListener(StartTiming);
        }
        if (stopButton != null)
        {
            Button stopBtn = stopButton.GetComponent<Button>();
            if (stopBtn != null)
                stopBtn.onClick.RemoveListener(StopTiming);
        }
    }

    /// <summary>
    /// 开始计时。
    /// 如果已经在计时，则不会重复开始。
    /// </summary>
    public void StartTiming()
    {
        if (isTiming)
            return;
        startTime = System.DateTime.Now;
        isTiming = true;
    }

    /// <summary>
    /// 结束计时。
    /// 如果当前没有在计时，则不会执行任何操作。
    /// </summary>
    public void StopTiming()
    {
        if (!isTiming)
            return;
        endTime = System.DateTime.Now;
        isTiming = false;
    }

    /// <summary>
    /// 重置计时。
    /// </summary>
    public void ResetTiming()
    {
        startTime = System.DateTime.Now;
        endTime = System.DateTime.Now;
        isTiming = false;
        hours = 0;
        minutes = 0;
        seconds = 0;
        milliseconds = 0;
        UpdateArgs();
    }

}
