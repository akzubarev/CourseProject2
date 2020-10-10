using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UFO : MonoBehaviour
{
    public GameObject explosion;
    public int pointsforkill = 50;

    void OnTriggerEnter2D(Collider2D obj)
    {
        var name = obj.gameObject.name;

        if (name == "PlayerProjectile(Clone)")
        {
            Destroy(obj.gameObject);
            Destroy(GetComponent<Collider>());
            Enemy.score += pointsforkill;
            UpdateScore();
            StartCoroutine("Die");
        }
    }

    public void UpdateScore()
    {
        GameObject.Find("Score").GetComponent<Text>().text = $"Score: {Enemy.score}";
    }

    IEnumerator Die()
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        expl.transform.parent = gameObject.transform;
        Debug.Log("Waiting");
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Dying");
        Destroy(gameObject);
     }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
