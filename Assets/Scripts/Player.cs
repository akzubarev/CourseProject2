using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject bullet;
    public float attackspeed = 0.4f;
    float velocity, leftborder, rightborder, timer = 0;
    Rigidbody2D r2d;

    public int lifes=4;
    
    void Start()
    {
        r2d = GetComponent<Rigidbody2D>();
        velocity = GameController.step * 10;

        leftborder = GameController.leftborder;
        rightborder = GameController.rightborder;
        UpdateLives();
    }

    void Update()
    {
        if (Input.GetKey("right") && transform.localPosition.x <= rightborder)
            r2d.velocity = new Vector2(velocity, 0);
        else if (Input.GetKey("left") && transform.localPosition.x >= leftborder)
            r2d.velocity = new Vector2(-velocity, 0);
        else
            r2d.velocity = new Vector2(0, 0);

        timer += Time.deltaTime;
        if (timer > attackspeed)
        {
            if (Input.GetKeyDown("space"))
            {
                timer = 0;
                Instantiate(bullet, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        var name = obj.gameObject.name;
        Destroy(obj.gameObject);
        if (name=="EnemyProjectile(Clone)"|| name == "Enemy(Clone)")
        {
            lifes--;
            if (lifes == -1)
               EndGame();
            else
              UpdateLives();

            GameObject.Find("GameController").GetComponent<Bot>().UpdateFleet();
        }
    }
    
   public void UpdateLives()
    {
        GameObject.Find("Lifes").GetComponent<Text>().text = $"Lifes: {lifes}";
    }

    void EndGame()
    {
        GameObject.Find("GameController").GetComponent<Bot>().EndGame(false);
    }
}
