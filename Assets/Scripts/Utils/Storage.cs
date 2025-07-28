using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/// <summary>
/// 持久化存储工具类
/// 提供存储和加载字符串数据以及泛型类数据的功能。
/// </summary>
public class Storage
{

    /// <summary>
    /// 将字符串数据进行持久化存储
    /// </summary>
    /// <param name="domain">域，填写类名</param>
    /// <param name="name">文件名</param>
    /// <param name="data">数据</param>
    /// <returns>是否成功存储</returns>
    public static bool Store(string domain, string name, string data)
    {
        string path = Application.persistentDataPath + "/" + domain + "/" + name + ".json";
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, data);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to store data into " + path + ": " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 将泛型类进行持久化存储
    /// </summary>
    /// <typeparam name="T">要进行存储的类</typeparam>
    /// <param name="domain">域，可填写执行操作的类</param>
    /// <param name="name">文件名</param>
    /// <param name="data">数据</param>
    /// <returns></returns>
    public static bool Store<T>(string domain, string name, T data)
    {
        string jsonData = JsonUtility.ToJson(data);
        return Store(domain, name, jsonData);
    }

    /// <summary>
    /// 从持久化存储中加载字符串数据
    /// </summary>
    /// <param name="domain">域，可填写执行操作的类</param>
    /// <param name="name">文件名</param>
    /// <param name="data">加载的字符串数据</param>
    /// <returns>是否成功加载</returns>
    public static bool Load(string domain, string name, out string data)
    {
        string path = Application.persistentDataPath + "/" + domain + "/" + name + ".json";
        try
        {
            if (File.Exists(path))
            {
                data = File.ReadAllText(path);
                return true;
            }
            Debug.LogError("Data file not found: " + path);
            data = null;
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load data from " + path + ": " + e.Message);
            data = null;
            return false;
        }
    }

    /// <summary>
    /// 从持久化存储中加载泛型类数据
    /// </summary>
    /// <typeparam name="T">读取的目标类</typeparam>
    /// <param name="domain">域，可填写执行操作的类</param>
    /// <param name="name">文件名</param>
    /// <param name="data">加载的目标类数据</param>
    /// <returns>是否成功加载</returns>
    public static bool Load<T>(string domain, string name, out T data)
    {
        string jsonData;
        try
        {
            if (Load(domain, name, out jsonData))
            {
                data = JsonUtility.FromJson<T>(jsonData);
                return true;
            }
            else
            {
                Debug.LogError("Failed to load data for " + domain + "/" + name);
                data = default;
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to deserialize data from " + domain + "/" + name + ": " + e.Message);
            data = default;
            return false;
        }
    }

}
