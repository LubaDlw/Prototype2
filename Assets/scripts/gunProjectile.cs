using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunProjectile : MonoBehaviour
{
    public float timeToDestroy = 1.1f;

    void Start()
    {
        Invoke(nameof(CheckAndDestroy), timeToDestroy);
    }

    void CheckAndDestroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
