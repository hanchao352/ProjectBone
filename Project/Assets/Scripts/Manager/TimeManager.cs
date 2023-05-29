using System;
using System.Collections.Generic;
using System.Threading;

public  class TimeManager : Singleton<TimeManager>
{
    
    public TimeManager() { }

    private Dictionary<Guid, Timer> timers = new Dictionary<Guid, Timer>();


    public override void Initialize()
    {
        base.Initialize();
    }

    public Guid InvokeAfterDelay(Action action, int millisecondsDelay, bool shouldRepeat = false)
    {
        return InvokeAfterDelay((Action<object[]>)((object[] args) => action()), millisecondsDelay, shouldRepeat);
    }

    public Guid InvokeAfterDelay(Action<object[]> action, int millisecondsDelay, bool shouldRepeat = false, params object[] args)
    {
        if (action == null || millisecondsDelay < 0) return Guid.Empty;

        var guid = Guid.NewGuid();
        TimerCallback timerCallback = state =>
        {
            try
            {
                action(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in timer callback: {ex}");
            }
            finally
            {
                if (!shouldRepeat)
                {
                    RemoveTimer(guid);
                }
            }
        };
        var timer = new Timer(timerCallback, null, millisecondsDelay, shouldRepeat ? millisecondsDelay : Timeout.Infinite);
        timers.Add(guid, timer);
        
        return guid;
    }

    public Guid InvokeEvery(Action action, int millisecondsInterval)
    {
        return InvokeEvery((Action<object[]>)((object[] args) => action()), millisecondsInterval);
    }

    public Guid InvokeEvery(Action<object[]> action, int millisecondsInterval, params object[] args)
    {
        return InvokeAfterDelay(action, millisecondsInterval, true, args);
    }


    public Guid InvokeWithInitialDelayAndInterval(Action action, int initialDelayMilliseconds, int intervalMilliseconds)
    {
        return InvokeWithInitialDelayAndInterval((Action<object[]>)((object[] args) => action()), initialDelayMilliseconds, intervalMilliseconds);
    }

    public Guid InvokeWithInitialDelayAndInterval(Action<object[]> action, int initialDelayMilliseconds, int intervalMilliseconds, params object[] args)
    {
        if (action == null || initialDelayMilliseconds < 0 || intervalMilliseconds < 0) return Guid.Empty;

        var guid = Guid.NewGuid();
        TimerCallback timerCallback = state =>
        {
            try
            {
                action(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in timer callback: {ex}");
            }
        };
        var timer = new Timer(timerCallback, null, initialDelayMilliseconds, intervalMilliseconds);
        timers.Add(guid, timer);

        return guid;
    }

    public void RemoveTimer(Guid guid)
    {
        if (timers.TryGetValue(guid, out Timer timer))
        {
            timer.Dispose();
            timers.Remove(guid);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Destroy()
    {
        base.Destroy();
        RemoveAllTimers();
    }

    // public void RemoveAllTimers()
    // {
    //     foreach (KeyValuePair<Guid, Timer> kvp in timers)
    //     {
    //         kvp.Value.Dispose();
    //     }
    //
    //     timers.Clear();
    // }
    public void RemoveAllTimers()
    {
        var timerIds = new List<Guid>(timers.Keys);

        for (int i = 0; i < timerIds.Count; i++)
        {
            Guid timerId = timerIds[i];
            Timer timer = timers[timerId];
            timer.Dispose();
            timers.Remove(timerId);
        }
    }
}