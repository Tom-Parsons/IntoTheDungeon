using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemySpawnController : MonoBehaviour
{

    public static EnemySpawnController instance;

    public bool paused = false;

    public int wave = 0;
    public int timeBetweenWaves = 5;

    public int attackingEnemiesNumber = 2;
    public int enemiesPerWave = 10;

    public List<GameObject> spawners;

    private GameObject player;
    public GameObject shopkeeper;
    public GameObject boss;
    public List<GameObject> spawnEnemies = new List<GameObject>();
    private List<GameObject> enemies;
    private List<GameObject> attackingEnemies;

    public Text waveText;

    bool waveTimer = false;

    public List<ChildPartileSystemsController> portalEffects = new List<ChildPartileSystemsController>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawners = new List<GameObject>();
        enemies = new List<GameObject>();
        attackingEnemies = new List<GameObject>();
        Invoke("getPlayer", 0.1f);
    }

    void getPlayer()
    {
        player = PlayerControl.instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused || RoomGeneration.instance != null && !RoomGeneration.instance.generated)
            return;
        //Not enough enemies attacking the player at once
        if (attackingEnemies.Count < attackingEnemiesNumber)
        {
            //There are still enemies left on the map
            if (attackingEnemies.Count != 0 && enemies.Count != 0)
            {
                //There are enemies that aren't currently attacking the player
                if (enemies.Count > attackingEnemiesNumber)
                {
                    MakeNearestEnemyAttack();
                }
            }
            else if(attackingEnemies.Count == 0 && enemies.Count > 0 && wave % 5 != 0)
            {
                MakeNearestEnemyAttack();
            }
            else
            {
                //No enemies left
                if (!waveTimer && enemies.Count <= 0)
                {
                    StartCoroutine(NewWave());
                }
            }
        }
    }

    public void MakeNearestEnemyAttack()
    {
        attackingEnemies.Clear();
        GameObject enemy = enemies[0];
        for (int i = 0; i < attackingEnemiesNumber; i++)
        {
            foreach (GameObject go in enemies)
            {
                if (!attackingEnemies.Contains(go) && go.name.Contains("MeleeEnemy"))
                {
                    enemy = go;
                }
            }
            float currentDistance = ((Vector3)player.transform.position - enemy.transform.position).magnitude;
            foreach (GameObject go in enemies)
            {
                if (!attackingEnemies.Contains(go) && go.name.Contains("MeleeEnemy"))
                {
                    Vector3 direction = player.transform.position - go.transform.position;
                    float distance = direction.magnitude;
                    if (distance < currentDistance)
                    {
                        enemy = go;
                        currentDistance = distance;
                    }
                }
            }

            if (!attackingEnemies.Contains(enemy))
            {
                attackingEnemies.Add(enemy);
            }
        }
    }

    public void RemoveFurthestEnemy()
    {
        GameObject enemy = attackingEnemies[0];
        float currentDistance = ((Vector3)player.transform.position - enemy.transform.position).magnitude;
        foreach (GameObject go in attackingEnemies)
        {
            Vector3 direction = player.transform.position - go.transform.position;
            float distance = direction.magnitude;
            if (distance > currentDistance)
            {
                enemy = go;
                currentDistance = distance;
            }
        }
        attackingEnemies.Remove(enemy);
    }

    IEnumerator NewWave()
    {
        Debug.Log("Starting new wave");
        waveTimer = true;

        yield return new WaitForSeconds(timeBetweenWaves - 1);
        foreach (var portalE in portalEffects)
        {
            portalE.PlayChildSystems();
        }
        yield return new WaitForSeconds(1);

        if ((wave + 1) % 5 == 0)
        {
            //Spawn boss
            print("Spawned Boss");
            GameObject spawnedBoss = Instantiate(boss);
            spawnedBoss.transform.position = spawners[Random.Range(0, spawners.Count - 1)].transform.position;
            spawnedBoss.GetComponent<NavMeshAgent>().enabled = true;
            enemies.Add(spawnedBoss);
        }
        else if (wave > 0 && wave % 5 == 0)
        {
            //Generate new level
            RoomGeneration.instance.GenerateLevel();
            yield return new WaitForSeconds(0.5f);
        }

        wave++;
        waveText.text = "Wave: " + wave;
        Debug.Log("Spawning enemies");
        int spawnedEnemyNumber = 0;
        int totalChance = 0;
        foreach (GameObject spawnedEnemy in spawnEnemies)
        {
            totalChance += spawnedEnemy.GetComponent<EnemyStats>().spawnPercentage;
        }

        //TODO: SPAWN ENEMIES
        while (spawnedEnemyNumber < enemiesPerWave && wave % 5 != 0)
        {
            if (spawnedEnemyNumber < enemiesPerWave)
            {
                int percentageChance = Random.Range(0, totalChance);
                GameObject enemyToInstantiate = spawnEnemies[0];
                int checkChance = 0;
                foreach(GameObject spawnedEnemy in spawnEnemies)
                {
                    int chance = spawnedEnemy.GetComponent<EnemyStats>().spawnPercentage;
                    if (checkChance + chance >= percentageChance)
                    {
                        enemyToInstantiate = spawnedEnemy;
                        break;
                    }
                    checkChance += chance;
                    //if(spawnedEnemy.name.Contains("RangedEnemy") && percentageChance <= 5)
                    //{
                    //    en = spawnedEnemy;
                    //}else if (spawnedEnemy.name.Contains("MeleeEnemy") && percentageChance > 5 && percentageChance <= 100)
                    //{
                    //    en = spawnedEnemy;
                    //}
                }
                GameObject en = Instantiate(enemyToInstantiate);
                en.transform.position = spawners[Random.Range(0, spawners.Count - 1)].transform.position;
                en.GetComponent<NavMeshAgent>().enabled = true;
                enemies.Add(en);
                spawnedEnemyNumber++;
                yield return new WaitForSeconds(1);
            }
        }
        foreach (var portalE in portalEffects)
        {
            portalE.StopChildSystems();
        }
        Debug.Log("Spawned Enemies");
        waveTimer = false;
    }

    public bool IsAttackingEnemy(GameObject enemy)
    {
        return attackingEnemies.Contains(enemy);
    }

    public void RemoveEnemy(GameObject go)
    {
        if (attackingEnemies.Contains(go))
        {
            attackingEnemies.Remove(go);
        }
        if (enemies.Contains(go))
        {
            enemies.Remove(go);
        }
        go.GetComponent<EnemyCombat>().Die();
        go.GetComponent<EnemyControl>().Die();
        Destroy(go, 5);
    }

    public List<GameObject> getEnemies()
    {
        return enemies;
    }

}
