using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test01 : MonoBehaviour
{
    [XLua.CSharpCallLua]
    public delegate double LuaArgs(params double[] args);


    private XLua.LuaEnv luaEnv;

    void Start()
    {
        luaEnv = new XLua.LuaEnv();
        luaEnv.DoString("CS.UnityEngine.Debug.Log('Hello Wold')");

        var max = luaEnv.Global.GetInPath<LuaArgs>("math.max");
        var min = luaEnv.Global.GetInPath<LuaArgs>("math.min");
        var random = luaEnv.Global.GetInPath<LuaArgs>("math.random");
        Debug.Log("max:" + max(32, 12));
        Debug.Log("min:" + min(32, 12));
        Debug.Log("random:" + random(1,10));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        luaEnv.Dispose();
    }
}
