using Bright.Serialization;
using Config;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableManager : SingletonManager<TableManager>,IGeneric
{
    private Tables tables ;

    public Tables Tables { get => tables; private set => tables = value; }

    public override void Initialize()
    {
        base.Initialize();
        var tablesCtor = typeof(Tables).GetConstructors()[0];
        var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
        // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
        System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?
            new System.Func<string, ByteBuf>(LoadByteBuf)
            : (System.Delegate)new System.Func<string, JSONNode>(Loader);
         Tables = (Tables)tablesCtor.Invoke(new object[] { loader });
    }

    private ByteBuf LoadByteBuf(string arg)
    {
       return new ByteBuf(File.ReadAllBytes($"{Application.dataPath}/GenJsonBin/{arg}.bytes"));
    }

    private JSONNode Loader(string arg)
    {
        return JSON.Parse(File.ReadAllText(Application.dataPath + "/GenJsonData/" + arg + ".json"));
    }


}
