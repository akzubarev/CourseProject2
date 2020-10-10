using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject bullet, explosion;
    public static int score=0;
    public int pointsforkill=10;

    public void Shoot()
    {   
        Instantiate(bullet, new Vector2(transform.position.x,transform.position.y-1), Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        var name = obj.gameObject.name;

        if (name == "PlayerProjectile(Clone)")
        {
            Destroy(obj.gameObject);
            Destroy(GetComponent<Collider>());
            score +=pointsforkill;
            UpdateScore();
            StartCoroutine("Die");
        }
    }

    public void UpdateScore()
    {
        GameObject.Find("Score").GetComponent<Text>().text = $"Score: {score}";
    }

    IEnumerator Die()
    {

        GameObject expl=  Instantiate(explosion,transform.position, Quaternion.identity);
         expl.transform.parent=gameObject.transform;
        gameObject.GetComponent<SpriteRenderer>().enabled=false;
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
        GameObject.Find("GameController").GetComponent<Bot>().UpdateFleet();
    }
}
