using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



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
    public Color backgroundColor;
    public GameObject startButton;
    public GameObject stopButton;


    public System.DateTime startTime;
    public System.DateTime endTime;
    public bool isTiming = false;


    private bool isLoaded = false;
    private GameObject clockObject;


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

    private void OnDestroy()
    {
        ReleaseButtons();
        DestroyImmediate(clockObject, true);
    }

    private void Update()
    {
        if (clockObject == null)
            return;

        if (isTiming)
            endTime = System.DateTime.Now;
        
        System.TimeSpan duration = endTime - startTime;
        hours = duration.Hours;
        minutes = duration.Minutes;
        seconds = duration.Seconds;
        milliseconds = duration.Milliseconds;

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

        string text = "";
        if (hours > 0)
            text += $"{hours:0} 时 ";
        if (minutes > 0 || text.Length > 0)
            text += $"{minutes:0} 分 ";
        if (hours == 0)
            text += $"{seconds:0} 秒 ";
        clockObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
    }

    public Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }

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

    private void UpdateArgs()
    {
        Transform circles = clockObject.transform.Find("Content/Circles");
        Transform pointers = clockObject.transform.Find("Content/Pointers");

        circles.Find("Hour").GetComponent<Image>().color = hoursCircleColor;
        circles.Find("Minute").GetComponent<Image>().color = minutesCircleColor;
        circles.Find("Second").GetComponent<Image>().color = secondsCircleColor;
        circles.Find("Core").GetComponent<Image>().color = coreColor;
        pointers.Find("Hour").GetComponent<Image>().color = hoursPointerColor;
        pointers.Find("Minute").GetComponent<Image>().color = minutesPointerColor;
        pointers.Find("Second").GetComponent<Image>().color = secondsPointerColor;

        Update();
    }

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


    public void StartTiming()
    {
        if (isTiming)
            return;
        startTime = System.DateTime.Now;
        isTiming = true;
    }

    public void StopTiming()
    {
        if (!isTiming)
            return;
        endTime = System.DateTime.Now;
        isTiming = false;
    }

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
