using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotDelayedJoin : MonoBehaviour
{
    private float timer = 0f;
    public UnityAction onTimerFinished;

    private void Start()
    {
        timer = Random.Range(2.5f, 4f);
#if UNITY_EDITOR
        timer = 0.5f;
#endif
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Destroy(this);
            onTimerFinished.Invoke();
        }
    }
}
