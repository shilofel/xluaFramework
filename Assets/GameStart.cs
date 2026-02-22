using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    // Start is called before the first frame update
    void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        Manager.Lua.Init(
            ()=>
            {
                Manager.Lua.StartLua("main");
            }
            );
//        Manager.Lua.StartLua("main");
        //全局查找Main函数，效率低
//        XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
//       func.Call();
    }
}
