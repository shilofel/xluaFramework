using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    //根目录
    public static readonly string AseetsPath = Application.dataPath;

    //需要打bundle的目录
    public static readonly string BuildResourcesPath = AseetsPath + "/BuildResources/";

    //bundle 输出目录
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    //可读写目录
    public static readonly string ReadWritePath = Application.persistentDataPath;

    //bundle资源路径
    public static string BundleResourcePath

    {
        get
        {
            if (AppConst.GameMode == GameMode.UpdateMode)
                return Application.persistentDataPath;
            return Application.streamingAssetsPath;
        }
    }
    /// <summary>
    /// 获取Unity的相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// 获取标准路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Trim().Replace("\\", "/");
    }

    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", name);
    }

    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", name);
    }
    //类型不明确
    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    }

    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }

    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    }

    public static string GetModelPath(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefabs/{0}.prefab", name);
    }

    public static string GetSpritePath(string name)
    {
        return string.Format("Assets/BuildResources/Sprites/{0}", name);
    }

    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scenes/{0}.unity", name);
    }
}
