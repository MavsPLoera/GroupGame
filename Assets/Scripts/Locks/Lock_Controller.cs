using System;
using UnityEngine;

public class Lock_Controller : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            AreaLock_Controller.instance.cannotEnterText(gameObject.tag);
    }
}
