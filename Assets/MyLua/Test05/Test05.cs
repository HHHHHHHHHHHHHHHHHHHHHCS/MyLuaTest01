using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


[LuaCallCSharp]
public struct UnitID
{
    public byte series;
    public byte type;
    public byte id;
}


[LuaCallCSharp]
public struct MyProperty
{
    public MyProperty(string _name, int _lv, double _hp, decimal _money, UnitID _unitID)
    {
        name = _name;
        lv = _lv;
        hp = _hp;
        money = _money;
        unitID = _unitID;
    }
    public string name;
    public int lv;
    public double hp;
    public decimal money;
    public UnitID unitID;
}

[LuaCallCSharp]
public enum MyTypeEnum
{
    God,
    Evil,
    Human
}

[CSharpCallLua]
public delegate int ToIntParam(int p);

[CSharpCallLua]
public delegate Vector3 ToVector3Param(Vector3 p);

[CSharpCallLua]
public delegate MyProperty ToCustomValueTypeParam(MyProperty p);

[CSharpCallLua]
public delegate MyTypeEnum ToEnumParam(MyTypeEnum p);

[CSharpCallLua]
public delegate decimal ToDecimalParam(decimal p);

[CSharpCallLua]
public delegate Array SwapArray(Array arr);

[CSharpCallLua]
public interface IChangeTest
{
    void ExChange(Array arr);
    void NoChange(Array arr);
}

[LuaCallCSharp]
public class Test05 : MonoBehaviour
{
    LuaEnv luaenv = new LuaEnv();

    public TextAsset luaText;

    ToIntParam intParam;
    ToVector3Param vector3Param;
    ToCustomValueTypeParam customValueTypeParam;
    ToEnumParam enumParam;
    ToDecimalParam decimalParam;
    SwapArray swapArray;

    Action actionTest;

    IChangeTest changeTest;

    LuaFunction add;

    [NonSerialized]
    public double[] my_double = new double[] { 1, 2 };
    [NonSerialized]
    public Vector3[] my_vector3 = new Vector3[] { new Vector3(1, 2, 3), new Vector3(4, 5, 6) };
    [NonSerialized]
    public MyProperty[] my_myProperty = new MyProperty[] { new MyProperty("Hello",1, 2.2,3M,new UnitID())
        , new MyProperty("Hello", 1, 2.2, 3M, new UnitID(){  type=0,series=1,id=2  }) };
    [NonSerialized]
    public MyTypeEnum[] my_myTypeEnum = new MyTypeEnum[] { MyTypeEnum.Evil, MyTypeEnum.God };
    [NonSerialized]
    public decimal[] myDecimal = new decimal[] { 1.00001M, 2.00002M };

    public float FloatParamMethod(float p)
    {
        return p;
    }

    public Vector3 Vector3ParamMethod(Vector3 p)
    {
        return p;
    }

    public MyProperty StructParamMethod(MyProperty p)
    {
        return p;
    }

    public MyTypeEnum EnumParamMethod(MyTypeEnum p)
    {
        return p;
    }

    public decimal DecimalParamMethod(decimal p)
    {
        return p;
    }

    // Use this for initialization
    void Start()
    {
        luaenv.DoString(luaText.text);

        luaenv.Global.Set("monoBehaviour", this);


        luaenv.Global.Get("ToArgsParam", out intParam);
        luaenv.Global.Get("ToArgsParam", out vector3Param);
        luaenv.Global.Get("ToArgsParam", out customValueTypeParam);
        luaenv.Global.Get("ToArgsParam", out enumParam);
        luaenv.Global.Get("ToArgsParam", out decimalParam);
        luaenv.Global.Get("SwapArray", out swapArray);
        luaenv.Global.Get("Lua_Access_CSharp", out actionTest);
        luaenv.Global.Get("Exchanger", out changeTest);
        luaenv.Global.Get("Add", out add);

        luaenv.Global.Set("g_int", 1024);
        luaenv.Global.Set(1024, 2048);
        int i;
        luaenv.Global.Get("g_int", out i);
        Debug.Log("g_int:" + i);
        luaenv.Global.Get(1024, out i);
        Debug.Log("1024:" + i);
    }


    // Update is called once per frame
    void Update()
    {
        // c# call lua function with value type but no gc (using delegate)
        intParam(1024); // primitive type

        Vector3 v3 = new Vector3(1, 2, 3); // vector3
        vector3Param(v3);

        MyProperty mystruct = new MyProperty("Hello", 1, 2.2, 3M, new UnitID()); // custom complex value type
        customValueTypeParam(mystruct);

        enumParam(MyTypeEnum.God); //enum 


        decimal d = -32132143143100109.00010001010M;
        decimal dr = decimalParam(d);
        Debug.Log(string.Format("decimal:{0}    match:{1}", d, d == dr));

        // using LuaFunction.Func<T1, T2, TResult>
        var addA = add.Func<int, int, int>(34, 56);
        var addB = (34 + 56);
        Debug.Log(string.Format("{0}=={1}?{2}", addA,addB,addA==addB));


        // lua access c# value type array no gc
        var a = swapArray(my_double); //primitive value type array
        swapArray(my_vector3); //vector3 array
        swapArray(my_myProperty); //custom struct array
        swapArray(my_myTypeEnum); //enum arry
        swapArray(myDecimal); //decimal arry

        // lua call c# no gc with value type
        actionTest();

        //c# call lua using interface
        changeTest.ExChange(my_double);
        changeTest.NoChange(my_vector3);

        //no gc LuaTable use
        luaenv.Global.Set("g_int", 2333);
        int i;
        luaenv.Global.Get("g_int", out i);
        Debug.Log(string.Format("g_int:{0}==2333?{1}",i,i==2333));

        luaenv.Global.Set(123.0001, mystruct);
        MyProperty mystruct2;
        luaenv.Global.Get(123.0001, out mystruct2);
        Debug.Log(string.Format("mystruct.name:{0}=={1}?{2}", mystruct.name, mystruct2.name, mystruct.name == mystruct2.name));

        decimal dr2 = 0.0000001M;
        luaenv.Global.Set((byte)12, d);
        luaenv.Global.Get((byte)12, out dr2);
        Debug.Log(string.Format("decimal:{0}=={1}?", d, dr2, d == dr2));


        int number = luaenv.Global.Get<int>("number");
        luaenv.Global.SetInPath("number", number + 1);
        //Debug.Log("number_int:" + number);

        double abc = luaenv.Global.GetInPath<double>("A.B.C");
        luaenv.Global.SetInPath("A.B.C", abc + 1.1);
        //Debug.Log("abc_double:" + abc);

        luaenv.Tick();
    }

    void OnDestroy()
    {
         intParam = null ;
         vector3Param=null;
         customValueTypeParam=null;
         enumParam=null;
         decimalParam=null;
         swapArray=null;

         actionTest=null;

         changeTest=null;

         add=null;

        luaenv.Dispose();
    }
}