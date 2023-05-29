using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<int, Delegate> eventTableWithArgs;
    private Dictionary<int, Delegate> eventTableWithoutArgs;

    public delegate void EventCallback(params object[] args);
    public delegate void EventCallbackWithoutArgs();

    public override void Initialize()
    {
        base.Initialize();
        eventTableWithArgs = new Dictionary<int, Delegate>();
        eventTableWithoutArgs = new Dictionary<int, Delegate>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Destroy()
    {
        base.Destroy();
        eventTableWithArgs.Clear();
        eventTableWithoutArgs.Clear();
    }

    public EventManager()
    {

    }

    public void RegisterEvent(int eventId, EventCallback callback)
    {
        if (eventTableWithArgs.ContainsKey(eventId))
        {
            eventTableWithArgs[eventId] = Delegate.Combine(eventTableWithArgs[eventId], callback);
        }
        else
        {
            eventTableWithArgs[eventId] = callback;
        }
    }

    public void RegisterEvent(int eventId, EventCallbackWithoutArgs callback)
    {
        if (eventTableWithoutArgs.ContainsKey(eventId))
        {
            eventTableWithoutArgs[eventId] = Delegate.Combine(eventTableWithoutArgs[eventId], callback);
        }
        else
        {
            eventTableWithoutArgs[eventId] = callback;
        }
    }

    public void UnregisterEvent(int eventId, EventCallback callback)
    {
        if (eventTableWithArgs.ContainsKey(eventId))
        {
            eventTableWithArgs[eventId] = Delegate.Remove(eventTableWithArgs[eventId], callback);
            if (eventTableWithArgs[eventId] == null)
            {
                eventTableWithArgs.Remove(eventId);
            }
        }
    }

    public void UnregisterEvent(int eventId, EventCallbackWithoutArgs callback)
    {
        if (eventTableWithoutArgs.ContainsKey(eventId))
        {
            eventTableWithoutArgs[eventId] = Delegate.Remove(eventTableWithoutArgs[eventId], callback);
            if (eventTableWithoutArgs[eventId] == null)
            {
                eventTableWithoutArgs.Remove(eventId);
            }
        }
    }

    public void TriggerEvent(int eventId, params object[] args)
    {
        if (eventTableWithArgs.ContainsKey(eventId))
        {
            (eventTableWithArgs[eventId] as EventCallback)?.Invoke(args);
        }
    }

    public void TriggerEvent(int eventId)
    {
        if (eventTableWithoutArgs.ContainsKey(eventId))
        {
            (eventTableWithoutArgs[eventId] as EventCallbackWithoutArgs)?.Invoke();
        }
    }
}
