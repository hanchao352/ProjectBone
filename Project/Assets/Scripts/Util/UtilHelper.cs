using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using UnityEngine;
public static class UtilHelper
{
       
    //位运算,包含
    public static bool IsContains(int value, int target)
    {
        if (target == 0)
        {
            return false;
        }
        return (value & target) == target;
    }   
    //位运算 移除
    public static int Remove(int value, int target)
    {
        return value & ~target;
    }
    //位运算 添加
    public static int Add(int value, int target)
    {
        return value | target;
    }
    // 将任意二进制数的最高位的 1 置为 0
    public static int ClearHighestBit(int num)
    {
        if (num == 0) return 0; // 如果是 0，直接返回 0

        int highestOne = 1;
        while (highestOne <= num)
        {
            highestOne <<= 1; // 左移直到找到高于 num 最高位的位置
        }

        highestOne >>= 1; // 右移一位以回到 num 的最高位的1
        return num ^ highestOne; // 使用异或操作翻转最高位的1
    }
    // 将任意二进制数的最高位前再加个 1
    public static int AddOneBeforeHighestBit(int num)
    {
        if (num == 0) return 1; // 如果是 0，则最高位前加个 1 就是 1

        // 找到除最高位以外的所有位都为0的数字
        // 这个数字的二进制表示将会是 1 后面跟着 n 个 0，其中 n 是原数字的位数
        int highestOne = 1;
        while (highestOne <= num)
        {
            highestOne <<= 1;
        }

        // 最后，返回这个数字减去 1 的结果，这样就能保留原有的所有位，并在最前面加上一个 1
        return highestOne | num;
    }
       
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
    public static T MakeComponent<T>(this RectTransform gameObject) where T:ComponentBase
    {
        T component = (T)ObjectCreator.CreateInstance(typeof(T));
        component.Root = gameObject.gameObject;
        component.Initialize();
        return component;
    }
    
    public static T MakeComponent<T>(this Transform gameObject) where T:ComponentBase
    {
        T component = (T)ObjectCreator.CreateInstance(typeof(T));
        component.Root = gameObject.gameObject;
        component.Initialize();
        return component;
    }
    
  

}