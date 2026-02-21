using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum GameMode
{
    EditorMode,
    PackageBundle,
    UpdateMode
}

public enum GameEvent
{
    GameInit = 10000,
    StartLua,
}

public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";
    public static GameMode GameMode = GameMode.EditorMode;
    public static bool OpenLog = true;
    //热更资源的地址
    public const string ResourcesUrl = "http://192.168.1.3/AssetBundles";
}
