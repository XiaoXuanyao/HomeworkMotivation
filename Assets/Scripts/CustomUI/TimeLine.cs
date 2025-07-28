using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// 时间线结点，包含开始时间、持续时间、文本描述和显示颜色。
/// </summary>
[System.Serializable]
public struct TimeLineNode
{

    public float timeVal;
    public string timeStr;
    public float durationVal;
    public string durationStr;
    public string text;
    public Color color;

    public TimeLineNode(
        float timeVal,
        string timeStr,
        float durationVal,
        string durationStr,
        string text,
        Color color
    ) {
        this.timeVal = timeVal;
        this.timeStr = timeStr;
        this.durationVal = durationVal;
        this.durationStr = durationStr;
        this.text = text;
        this.color = color;
    }

}



/// <summary>
/// 时间线组件
/// 把此组件拖动到一个空物体上，可以自动生成时间线组件。
/// 修改数据后，时间线会自动更新。
/// </summary>
[ExecuteInEditMode]
public class TimeLine : MonoBehaviour
{

    public float ratio;
    public List<TimeLineNode> data;
    public bool useScrollRect = true;


    private bool isLoaded = false;
    private GameObject timeLine;
    private List<GameObject> dataObject;
    

    /// <summary>
    /// 当组件被添加时，初始化相关物体。
    /// 如果已经存在其他 TimeLine 组件，则销毁当前组件。
    /// 如果组件更新，则更新相关物体。
    /// </summary>
    private void OnValidate()
    {
        if (GetComponents<TimeLine>().Length > 1 && !isLoaded)
        {
            Debug.LogWarning("TimeLine component should not be added to GameObject "
                + "already containing a TimeLine component.");
            Invoke(nameof(DestroyThis), 0.01f);
            return;
        }
        if (!isLoaded)
        {
            Invoke(nameof(InitTimeLine), 0.01f);
            isLoaded = true;
        }
        else
        {
            Invoke(nameof(UpdateData), 0.01f);
        }
    }

    /// <summary>
    /// 移除组件时，销毁所有此时间线相关物体。
    /// </summary>
    private void OnDestroy()
    {
        if (isLoaded)
        {
            isLoaded = false;
            DestroyImmediate(timeLine, true);
        }
    }

    /// <summary>
    /// 初始化时间线，清除旧的子物体，加载时间线预制件并设置相关属性。
    /// 如果使用 ScrollRect，则自动关联滑动条和视口。
    /// </summary>
    private void InitTimeLine()
    {
        foreach (Transform obj in transform)
        {
            DestroyImmediate(obj.gameObject, true);
        }
        timeLine = Resources.Load<GameObject>("Prefabs/TimeLine");
        if (timeLine == null)
        {
            Debug.LogError("TimeLine prefab not found in Resources/Prefabs!");
            return;
        }
        ratio = 1f;
        timeLine = Instantiate(timeLine, transform);
        timeLine.name = "TimeLine";
        data = new List<TimeLineNode>();
        dataObject = new List<GameObject>();
        if (useScrollRect)
        {
            if (GetComponent<ScrollRect>() == null)
                gameObject.AddComponent<ScrollRect>();
            GetComponent<ScrollRect>().content = timeLine.GetComponent<RectTransform>();
            GetComponent<ScrollRect>().viewport = GetComponent<RectTransform>();
        }
        UpdateData();
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
    /// 更新时间线数据。
    /// 清除旧的时间线节点，重新生成新的时间线节点。
    /// </summary>
    private void UpdateData()
    {
        // 清除旧的时间线节点
        foreach (GameObject obj in dataObject)
        {
            DestroyImmediate(obj, true);
        }
        dataObject.Clear();

        // 计算时间线的最大长度
        float maxLength = 0;

        // 遍历数据，生成新的时间线节点
        foreach (TimeLineNode item in data)
        {
            maxLength = Mathf.Max(maxLength, (item.timeVal + item.durationVal) * ratio);

            GameObject obj = Instantiate(
                timeLine.transform.Find("KeyPoint0").gameObject, timeLine.transform
            );
            float xPosition = obj.transform.GetComponent<RectTransform>().anchoredPosition.x;
            obj.transform.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(xPosition, -item.timeVal * ratio - 30);
            obj.name = $"KeyPoint{dataObject.Count + 1}";
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.text;
            obj.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = item.timeStr;
            obj.transform.Find("Duration").GetComponent<RectTransform>().sizeDelta =
                new Vector2(obj.transform.Find("Duration").GetComponent<RectTransform>().sizeDelta.x,
                            item.durationVal * ratio);
            obj.transform.Find("Duration").GetComponent<Image>().color = item.color;
            obj.transform.Find("Duration/Text").GetComponent<TextMeshProUGUI>().text = item.durationStr;
            obj.transform.Find("Duration/Text").GetComponent<TextMeshProUGUI>().color = item.color;
            obj.SetActive(true);
            dataObject.Add(obj);
        }

        // 更新时间线的长度
        timeLine.transform.Find("Line").transform.GetComponent<RectTransform>()
            .sizeDelta = new Vector2(5f, maxLength + 75f);
        timeLine.GetComponent<RectTransform>().sizeDelta =
            new Vector2(timeLine.GetComponent<RectTransform>().sizeDelta.x, maxLength + 75f);
    }

}
