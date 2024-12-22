using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OneTriggerEvent
{
    private UnityAction callbacks;
    private bool hasBeenTriggered;

    public void Invoke()
    {
        if(hasBeenTriggered)
        {
            Debug.LogError("A one time event has been triggered more than once!");
            return;
        }    
        hasBeenTriggered = true;
        callbacks?.Invoke();
        callbacks = null;
    }

    public void SubscribeAndInvokeIfTriggered(UnityAction callback)
    {
        if (hasBeenTriggered)
            callback.Invoke();
        else
            callbacks += callback;
    }

    public void Unsubscribe(UnityAction callback)
    {
        callbacks -= callback;
    }
}

public class OneTriggerEvent<T>
{
    private UnityAction<T> callbacks;
    private bool hasBeenTriggered;
    private T var1;

    public void Invoke(T var1)
    {
        if(hasBeenTriggered)
        {
            Debug.LogError("A one time event has been triggered more than once!");
            return;
        }
        this.var1 = var1;
        hasBeenTriggered = true;
        callbacks?.Invoke(var1);
        callbacks = null;
    }

    public void SubscribeAndInvokeIfTriggered(UnityAction<T> callback)
    {
        if (hasBeenTriggered)
            callback.Invoke(var1);
        else
            callbacks += callback;
    }

    public void Unsubscribe(UnityAction<T> callback)
    {
        callbacks -= callback;
    }
}

public class OneTriggerEvent<T, E>
{
    private UnityAction<T, E> callbacks;
    private bool hasBeenTriggered;
    private T var1;
    private E var2;

    public void Invoke(T var1, E var2)
    {
        if(hasBeenTriggered)
        {
            Debug.LogError("A one time event has been triggered more than once!");
            return;
        }
        this.var1 = var1;
        this.var2 = var2;
        hasBeenTriggered = true;
        callbacks?.Invoke(var1, var2);
        callbacks = null;
    }

    public void SubscribeAndInvokeIfTriggered(UnityAction<T, E> callback)
    {
        if (hasBeenTriggered)
            callback.Invoke(var1, var2);
        else
            callbacks += callback;
    }

    public void Unsubscribe(UnityAction<T, E> callback)
    {
        callbacks -= callback;
    }
}
