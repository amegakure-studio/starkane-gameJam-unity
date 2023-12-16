using System;
using System.Collections.Generic;

public class EventManager
{
    private Dictionary<GameEvent, Action<Dictionary<string, object>>> eventDictionary;

    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager();
                instance.Initialize();
            }
            return instance;
        }
    }

    private void Initialize()
    {
        eventDictionary = new Dictionary<GameEvent, Action<Dictionary<string, object>>>();
    }

    public void Subscribe(GameEvent eventName, Action<Dictionary<string, object>> listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = null;
        }
        eventDictionary[eventName] += listener;
    }

    public void Unsubscribe(GameEvent eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    public void Publish(GameEvent eventName, Dictionary<string, object> eventData = null)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
    }
}
