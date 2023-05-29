using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<int, Delegate> eventTable;

    public delegate void EventCallback(params object[] args);

    public override void Initialize()
    {
        base.Initialize();
        eventTable = new Dictionary<int, Delegate>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Destroy()
    {
        base.Destroy();
        eventTable.Clear();
    }

    public EventManager()
    {

    }

    public void RegisterEvent(int eventId, EventCallback callback)
    {
        if (eventTable.ContainsKey(eventId))
        {
            eventTable[eventId] = Delegate.Combine(eventTable[eventId], callback);
        }
        else
        {
            eventTable[eventId] = callback;
        }
    }

    public void UnregisterEvent(int eventId, EventCallback callback)
    {
        if (eventTable.ContainsKey(eventId))
        {
            eventTable[eventId] = Delegate.Remove(eventTable[eventId], callback);
            if (eventTable[eventId] == null)
            {
                eventTable.Remove(eventId);
            }
        }
    }

    public void TriggerEvent(int eventId, params object[] args)
    {
        if (eventTable.ContainsKey(eventId))
        {
            (eventTable[eventId] as EventCallback)?.Invoke(args);
        }
    }
}