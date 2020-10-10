using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bot : MonoBehaviour
{
    public GameObject enemy, player;

    List<List<GameObject>> fleet = new List<List<GameObject>>();

    int leftcolumn = 0, rightcolumn = 11;
    int bottomrow = 4;
    List<int> bottoms = new List<int>();

    float leftborder, rightborder, speed, shootingspeed;
    float step;

    int level=0;

    System.Random rnd = new System.Random();
    private readonly object _locker = new object();

    WhereToMove currentlyMoving = WhereToMove.right;

    enum WhereToMove
    {
        left,
        right,
        down
    }

    void Start()
    {
        step = GameController.step;
        leftborder = GameController.leftborder;
        rightborder = GameController.rightborder;
        speed = GameController.speed;
        shootingspeed=speed;

        SpawnShips();
        StartCoroutines();
    }
    
   public void StartCoroutines()
    {
        StartCoroutine("MoveFleet");
        StartCoroutine("FireAtPlayer");
        StartCoroutine("UpdateFleetCoroutine");
    }

    public void LevelUp()
    { 
        level++;
        speed=1-0.1f*level;
        shootingspeed= 1 - 0.1f * level;
    }

    public void ResetToStart()
    {
        level=0;
        speed = 1;
        shootingspeed = 1;
    }

    public void SpawnShips()
    {
        fleet= new List<List<GameObject>>();
       
        GameObject enemies = GameObject.Find("Enemies");
        if (enemies)
            Destroy(enemies);
        enemies = new GameObject();
        enemies.name = "Enemies";
        

        for (int i = 0; i < 12; i++)
        {
            fleet.Add(new List<GameObject>());
            for (int j = 0; j < 5; j++)
            {
                GameObject ship = Instantiate(enemy,
                    new Vector2(transform.position.x + step * i,
                    transform.position.y - step * j),
                    Quaternion.identity);

                ship.transform.parent = enemies.transform;
                fleet[i].Add(ship);
            }
            bottoms.Add(4);
        }

    }

    IEnumerator MoveFleet()
    {
        while (true)
        { 
            yield return new WaitForSeconds(speed);
            Vector3 movement = new Vector3(0, 0, 0);
            switch (currentlyMoving)
            {

                case WhereToMove.right:
                    movement = new Vector3(step, 0, 0);
                    break;
                case WhereToMove.left:
                    movement = new Vector3(-step, 0, 0);
                    break;
                case WhereToMove.down:
                    movement = new Vector3(0, -step, 0);
                    break;

            }

            for (int i = fleet[0].Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < fleet.Count; j++)
                    Move(j, i, movement);

                yield return new WaitForSeconds(speed / fleet[0].Count);

            }
            ChangeState();
        }
    }

    void ChangeState()
    {
        UpdateFleet();
        try
        {
            lock (_locker)
            {
                switch (currentlyMoving)
                {
                    case WhereToMove.right:
                        if (fleet[rightcolumn][bottoms[rightcolumn]].transform.localPosition.x >= rightborder - step + 0.1)
                            currentlyMoving = WhereToMove.down;
                        break;
                    case WhereToMove.left:
                        if (fleet[leftcolumn][bottoms[leftcolumn]].transform.localPosition.x <= leftborder + step - 0.1)
                            currentlyMoving = WhereToMove.down;
                        break;
                    case WhereToMove.down:
                        if (fleet[rightcolumn][bottoms[rightcolumn]].transform.localPosition.x >= rightborder - step + 0.1)
                            currentlyMoving = WhereToMove.left;
                        else currentlyMoving = WhereToMove.right;
                        break;
                }
                if (rightcolumn==leftcolumn)
                    GoRapid();
                else
                for (int i = 0; i < fleet.Count; i++)
                    if (bottoms[i] >= 0)
                        if (fleet[i][bottoms[i]].transform.localPosition.y <= -0.5)
                            GoRapid();

            }
        }
        catch (System.Exception e) { }
    }

    void Move(int rowindex, int columnindex, Vector3 movement)
    {
        try
        {
            fleet[rowindex][columnindex].transform.localPosition += movement;
        }
        catch (System.Exception e) { }
    }

    IEnumerator UpdateFleetCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateFleet();
    }

    public void UpdateFleet()
    {
        lock (_locker)
        {
            rightcolumn = -1;
            leftcolumn = -1;
            bottomrow = -1;
            bottoms = new List<int>();
            for (int i = 0; i < 12; i++)
            {
                bottoms.Add(-1);
            }

            for (int i = 0; i < fleet.Count; i++)
                for (int j = 0; j < fleet[0].Count; j++)
                    if (fleet[i][j])
                        bottoms[i] = j;

            for (int i = 0; i < fleet.Count; i++)
                for (int j = 0; j < fleet[0].Count; j++)
                    if (fleet[i][j])
                    {
                        rightcolumn = i;
                        break;
                    }

            for (int i = rightcolumn; i >= 0; i--)
                for (int j = 0; j < fleet[0].Count; j++)
                    if (fleet[i][j])
                    {
                        leftcolumn = i;
                        break;
                    }
            if (rightcolumn == -1)
                EndGame(true);
        }
    }

    IEnumerator FireAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingspeed - 0.1f);
            UpdateFleet();
            int firing = FindClosest();//-1
            int count = 0;

            while (firing == -1)
                firing = FindClosest();

            while (bottoms[firing] != -1 && bottoms[firing] - count >= 0 && !fleet[firing][bottoms[firing] - count])
                count++;
            try
            {
                fleet[firing][bottoms[firing] - count].GetComponent<Enemy>().Shoot();
            }
            catch (System.ArgumentOutOfRangeException e) { }
            catch (MissingReferenceException e) { }
            catch (System.Exception e) { }

            count = 0;
            yield return new WaitForSeconds(0.1f);

            firing += 1 - 2 * rnd.Next(0, 2);
            if (firing == -1) firing = 1;
            if (firing == 12) firing = 10;

            while (bottoms[firing] != -1 && bottoms[firing] - count >= 0 && !fleet[firing][bottoms[firing] - count])
            {
                count++;
            }
            try
            {
                fleet[firing][bottoms[firing] - 1 - count].GetComponent<Enemy>().Shoot();
            }
            catch (System.ArgumentOutOfRangeException e) { }
            catch (MissingReferenceException e) { }
            catch (System.Exception e) { }

            count = 0;
            firing = rnd.Next(leftcolumn, rightcolumn + 1);

            while (bottoms[firing] != -1 && !fleet[firing][bottoms[firing]] && count < 10)
            {
                firing = rnd.Next(leftcolumn, rightcolumn);
                count++;
            }
            try
            {
                fleet[firing][bottoms[firing]].GetComponent<Enemy>().Shoot();
            }
            catch (System.ArgumentOutOfRangeException e) { }
            catch (MissingReferenceException e) { }
            catch (System.Exception e) { }

        }
    }

    int FindClosest()
    {
        float min = 10;
        int minnum = -1;

        for (int i = leftcolumn; i <= rightcolumn; i++)
        {
            if (bottoms[i] != -1)
                try
                {
                    if (System.Math.Abs(fleet[i][bottoms[i]].transform.localPosition.x - player.transform.localPosition.x) < min)
                    {
                        min = System.Math.Abs(fleet[i][bottoms[i]].transform.localPosition.x - player.transform.localPosition.x);
                        minnum = i;
                    }
                }
                catch (System.Exception e) { }
        }
        return minnum;
    }

    void GoRapid()
    {
        speed = 0.1f;
    }

    public void EndGame(bool win)
    {
        StopCoroutine("MoveFleet");
        StopCoroutine("FireAtPlayer");
        StopCoroutine("UpdateFleetCoroutine");
        GetComponent<GameController>().EndGame(win);
    }
}
