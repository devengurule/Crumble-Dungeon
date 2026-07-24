using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<EventType, Action<object>> eventDict = new();

    public void Subscribe(EventType eventType, Action<object> action)
    {
        if (GameController.instance.eventManager != null)
        {
            try
            {
                // If event type has not been added to the event dictionary
                //      add to the event dictionary and assign and empty delegate list to that key
                if (!eventDict.ContainsKey(eventType))
                {
                    eventDict[eventType] = delegate { };
                }
            }
            catch
            {
                Debug.Log($"Failed to Create Empty Delegate List with EventType: {eventType}");
            }

            try
            {
                // Subscribe the action to the delegate list
                eventDict[eventType] += action;
            }
            catch
            {
                Debug.Log($"Failed to Subscribe Action: {action} to EventType: {eventType}");
            }
        }
    }

    public void Unsubscribe(EventType eventType, Action<object> action)
    {
        if (GameController.instance.eventManager != null)
        {
            // Remove action from delegate list so long as the event type does exist in the event dictionary
            if (eventDict.ContainsKey(eventType))
            {
                eventDict[eventType] -= action;
            }
        }
    }

    public void Publish(EventType eventType, object value = default)
    {
        if (GameController.instance.eventManager != null)
        {
            //try
            //{
                // Do a multicast invokation on the delegate list so long as the event type exists in the event dictionary
                if (eventDict.ContainsKey(eventType))
                {
                    eventDict[eventType]?.Invoke(value);
                }
            //}
            //catch (Exception e)
            //{
            //    Debug.Log($"Failed to Publish EventType: {eventType}, Error Message:{e}");
            //}
        }
    }
}
