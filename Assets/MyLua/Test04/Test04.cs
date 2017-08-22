using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class Test04 : MonoBehaviour
{
    [CSharpCallLua]
    public interface ITest04Calc
    {
        int value { get; set; }
        int Add(params int[] values);
        float Mult(params double[] values);
        float Sub(float number, params float[] values);
        double Div(double number, params float[] values);
    }

    [CSharpCallLua]
    public delegate ITest04Calc Test04CalcDg(int mult, params string[] args);

    public TextAsset luaText;

    // Use this for initialization
    void Start()
    {
        LuaEnv luaenv = new LuaEnv();
        Test(luaenv);//调用了带可变参数的delegate，函数结束都不会释放delegate，即使置空并调用GC
        luaenv.Dispose();
    }

    void Test(LuaEnv luaenv)
    {
        luaenv.DoString(luaText.text);
        Test04CalcDg calcDg = luaenv.Global.GetInPath<Test04CalcDg>("Test04CalcDg.New");
        ITest04Calc calc = calcDg(10, "hello", "world"); //constructor
        Debug.Log("value = " + calc.value);
        Debug.Log("Add = " + calc.Add(1, 2, 4, 8));
        Debug.Log("Mult = " + calc.Mult(3, 5, 7, 9));
        Debug.Log("Sub = " + calc.Sub(3, 5, 7, 9));
        Debug.Log("Div = " + calc.Div(1024, 2, 4, 8));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
