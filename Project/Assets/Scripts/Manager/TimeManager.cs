using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public class TimerManager : SingletonManager<TimerManager>, IGeneric
{
    private class TimerData
    {
        public Action Callback;
        public Action<object[]> CallbackWithParams;
        public float Delay;
        public float Interval;
        public object[] Args;
        public float NextTick;
        public bool Repeat;
    }

    private Dictionary<int, TimerData> timers = new Dictionary<int, TimerData>();
    private int nextId = 1;
    private float elapsedTime = 0f;

    public override void Initialize()
    {
        elapsedTime = 0f;
    }

    public override void Update(float deltaTime)
    {
        elapsedTime += deltaTime * 1000f;

        List<int> timersToRemove = new List<int>();

        foreach (var entry in timers)
        {
            int id = entry.Key;
            TimerData data = entry.Value;

            if (elapsedTime >= data.NextTick)
            {
                if (data.Callback != null)
                {
                    data.Callback();
                }
                else if (data.CallbackWithParams != null)
                {
                    data.CallbackWithParams(data.Args);
                }

                if (data.Repeat)
                {
                    data.NextTick = elapsedTime + data.Interval;
                }
                else
                {
                    timersToRemove.Add(id);
                }
            }
        }

        foreach (int id in timersToRemove)
        {
            timers.Remove(id);
#if UNITY_EDITOR
            Debug.Log("移除计时器id"+id); 
#endif
        }
    }

    public int SetTimeout(Action callback, float delayInMilliseconds)
    {
        var data = new TimerData
        {
            Callback = callback,
            Delay = delayInMilliseconds,
            NextTick = elapsedTime + delayInMilliseconds,
            Repeat = false
        };
        timers[nextId] = data;
        return nextId++;
    }

    public int SetTimeout(Action<object[]> callback, float delayInMilliseconds, params object[] args)
    {
        var data = new TimerData
        {
            CallbackWithParams = callback,
            Args = args,
            Delay = delayInMilliseconds,
            NextTick = elapsedTime + delayInMilliseconds,
            Repeat = false
        };
        timers[nextId] = data;
        return nextId++;
    }

    public int SetInterval(Action callback, float intervalInMilliseconds)
    {
        var data = new TimerData
        {
            Callback = callback,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + intervalInMilliseconds,
            Repeat = true
        };
        timers[nextId] = data;
        return nextId++;
    }

    public int SetIntervalAtOnce(Action callback, float intervalInMilliseconds)
    {
        var data = new TimerData
        {
            Callback = callback,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + intervalInMilliseconds,
            Repeat = true
        };
        callback();
        timers[nextId] = data;
        return nextId++;
    }
    public int SetInterval(Action<object[]> callback, float intervalInMilliseconds, params object[] args)
    {
        var data = new TimerData
        {
            CallbackWithParams = callback,
            Args = args,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + intervalInMilliseconds,
            Repeat = true
        };
        timers[nextId] = data;
        return nextId++;
    }
    public int SetIntervalAtOnce(Action<object[]> callback, float intervalInMilliseconds, params object[] args)
    {
        var data = new TimerData
        {
            CallbackWithParams = callback,
            Args = args,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + intervalInMilliseconds,
            Repeat = true
        };
        callback(args);
        timers[nextId] = data;
        return nextId++;
    }
    public int SetIntervalAfterDelay(Action callback, float delayInMilliseconds, float intervalInMilliseconds)
    {
        var data = new TimerData
        {
            Callback = callback,
            Delay = delayInMilliseconds,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + delayInMilliseconds,
            Repeat = true
        };
        timers[nextId] = data;
        return nextId++;
    }

    public int SetIntervalAfterDelay(Action<object[]> callback, float delayInMilliseconds, float intervalInMilliseconds, params object[] args)
    {
        var data = new TimerData
        {
            CallbackWithParams = callback,
            Args = args,
            Delay = delayInMilliseconds,
            Interval = intervalInMilliseconds,
            NextTick = elapsedTime + delayInMilliseconds,
            Repeat = true
        };
        timers[nextId] = data;
        return nextId++;
    }

    public void RemoveTimer(int id)
    {
        timers.Remove(id);
    }

    public override void Dispose()
    {
        base.Dispose();
        timers.Clear();
    }

   


}
