using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject obstacle, player, victorydefeat, tryagain, next,quit, playerprefab, ufo;
    public float _step = 0.6f, _leftborder = -5.7f, _rightborder = 5.7f, _speed = 1;
    public static float step, leftborder, rightborder, speed;
    System.Random rnd = new System.Random();

    void Awake()
    {
        step = _step;
        leftborder = _leftborder;
        rightborder = _rightborder;
        speed = _speed;

        SpawnObstacles();
        StartCoroutine("UFOSpawner");
    }

    public void Quit()
    { 
        Application.Quit();    
    }

    IEnumerator UFOSpawner()
    {
        yield return new WaitForSeconds(16f);
        bool lefttoright = rnd.Next(2) == 1;
        Vector2 position = lefttoright ? new Vector2(-5.7f, 4) : new Vector2(5.7f, 4);
        Instantiate(ufo, position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity =
            lefttoright ? new Vector2(5, 0) : new Vector2(-5, 0);
    }
   
    void SpawnObstacles()
    {
        GameObject obstacles = GameObject.Find("Obstacles");
        if (obstacles)
            Destroy(obstacles);

        obstacles = new GameObject();
        obstacles.name = "Obstacles";


        GameObject projectiles = GameObject.Find("Projectiles");
        if (projectiles)
            Destroy(projectiles);

        projectiles = new GameObject();
        projectiles.name = "Projectiles";


        for (int i = 0; i < 6; i++)
        {
            Instantiate(obstacle, new Vector2(transform.position.x + 0.7f + 2 * i,
                          transform.position.y - 7),
                          Quaternion.identity).transform.parent
                  = obstacles.transform;

        }
    }

    public void TryAgain()
    {
        Enemy.score = 0;
        victorydefeat.SetActive(false);
        tryagain.SetActive(false);
        quit.SetActive(false);
        GetComponent<Bot>().SpawnShips();
        GetComponent<Bot>().ResetToStart();
        SpawnObstacles();
        player = Instantiate(playerprefab, new Vector3(0, -4, -1), Quaternion.identity);
        GetComponent<Bot>().player = player;
        player.GetComponent<Player>().lifes = 4;
        player.GetComponent<Player>().UpdateLives();
        GetComponent<Bot>().StartCoroutines();
    }

    public void NextLevel()
    {

        victorydefeat.SetActive(false);
        next.SetActive(false);
        quit.SetActive(false);
        GetComponent<Bot>().SpawnShips();
        GetComponent<Bot>().LevelUp();
        SpawnObstacles();
        player.GetComponent<Player>().lifes++;
        player.GetComponent<Player>().UpdateLives();
        GetComponent<Bot>().StartCoroutines();
    }

    public void EndGame(bool win)
    {
        StopCoroutine("UFOSpawner");
        if (win)
        {
            victorydefeat.GetComponent<Text>().text = "Victory!";
            next.SetActive(true);
        }
        else
        {
            Destroy(player);
            victorydefeat.GetComponent<Text>().text = "Defeat!";
            tryagain.SetActive(true);
        }
        victorydefeat.SetActive(true);
        quit.SetActive(true);
    }
}
