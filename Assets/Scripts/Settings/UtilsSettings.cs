using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 常用工具设置类
/// 提供获取存储路径等常用功能。
/// </summary>
public class UtilsSettings : MonoBehaviour
{

    /// <summary>
    /// 获取持久化存储路径
    /// </summary>
    public static void GetStoragePath()
    {
        string path = Application.persistentDataPath;
        Debug.Log("Persistent Data Path: " + path);
    }

}
