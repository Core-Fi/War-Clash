using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{
    public float DelayTime;
    private float liveTime;
    void OnEnable()
    {
        liveTime = 0;
    }

    void Update()
    {
        liveTime += Time.deltaTime;
        if (DelayTime < liveTime)
        {
            Object.Destroy(gameObject);
        }
    }
}
