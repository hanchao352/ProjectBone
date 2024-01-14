using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Config.character;
using Google.Protobuf;
using UnityEngine;
public static class UtilHelper
{
       
        
       
}
public static class ObjectCreator
{
    private static readonly ConcurrentDictionary<Type, Func<object>> CreatorCache = new ConcurrentDictionary<Type, Func<object>>();

    public static object CreateInstance(Type type)
    {
        // 尝试从缓存中获取委托
        if (!CreatorCache.TryGetValue(type, out var creator))
        {
            // 如果缓存中没有，则创建并编译一个新的表达式树
            var newExpr = Expression.New(type);
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(newExpr, typeof(object)));
            creator = lambda.Compile();

            // 将编译的委托添加到缓存
            CreatorCache[type] = creator;
        }

        return creator();
    }
}
public static class UIUtilHleper
{
    public static T MakeComponent<T>(this GameObject gameObject) where T:ComponentBase
    {
        T component = (T)ObjectCreator.CreateInstance(typeof(T));
        component.Root = gameObject;
        component.Initialize();
        return component;
    }
    
}