﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[System.Serializable]
public class MyObject
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class Test02 : MonoBehaviour
{
    public TextAsset luaScript;
    public MyObject[] injections;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    void Awake()
    {
        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        foreach (var injection in injections)
        {
            scriptEnv.Set(injection.name, injection.value);
        }

        luaEnv.DoString(luaScript.text, "LuaBehaviour", scriptEnv);

        Debug.Log(scriptEnv.Get<string,GameObject>("0"));
        Debug.Log("====================================");
        foreach (var item in scriptEnv.GetKeys())
        {
            Debug.Log(scriptEnv.Get<string, object>((string)item));
        }
        Debug.Log("====================================");


        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
