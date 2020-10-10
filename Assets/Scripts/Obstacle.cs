using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    int lifes=5;

    void ChangeImage()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{lifes}lifes");
    }
    
    void OnTriggerEnter2D(Collider2D obj)
    {
        var name = obj.gameObject.name;

        if (name == "PlayerProjectile(Clone)" || name == "EnemyProjectile(Clone)")
        {
            Destroy(obj.gameObject);
            lifes--;
            if (lifes>0)
                ChangeImage();
            else
                Destroy(gameObject);
        }
        else if (name == "Enemy")
           {
            Destroy(obj.gameObject);
            Destroy(gameObject);
            }

    }
}
