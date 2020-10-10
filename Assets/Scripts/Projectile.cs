using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int speed=0;

    void Start()
    {
        transform.parent= GameObject.Find("Projectiles").transform;
        var r2d = GetComponent<Rigidbody2D>();
        r2d.velocity = new Vector2(0,speed);
    }
    
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
