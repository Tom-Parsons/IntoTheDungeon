using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public float speed, jumpHeight, maxHealth, health, maxMana, mana, damage, timeBetweenSwings;

    public int spawnPercentage = 10; // Make sure that all these percentages add up to 100

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    private EnemySpawnController spawnController;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        spawnController = EnemySpawnController.instance;
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void TakeDamage(float damage)
    {
        health -= damage;

        CombatEffects.GotHit(gameObject);

        if (health <= 0)
        {
            if (!gameObject.GetComponent<EnemyCombat>().isEnemyDead())
            {
                anim.SetTrigger("Die");
                EnemySpawnController.instance.RemoveEnemy(gameObject);

                GetComponent<EnemyControl>().Die();

                int coinsAmount = Random.Range(minCoins, maxCoins + 1);
                for(int i = 0; i < coinsAmount; i++)
                    Instantiate(coinPrefab, gameObject.transform.position, gameObject.transform.rotation);
            }
        }
        else
        {
            anim.SetTrigger("Damaged");
            GetComponent<EnemyControl>().PushBackMovement();
            anim.speed = 0;
            Invoke("ResetAnimatorSpeed", 0.06f);
        }
    }

    void ResetAnimatorSpeed ()
    {
        anim.speed = 1;
    }

    public void DeathAnim()
    {
        StartCoroutine(DisappearAnim());
    }

    private IEnumerator DisappearAnim()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));

        if (gameObject.GetComponent<CapsuleCollider>() != null) gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Destroy(gameObject.GetComponent<NavMeshAgent>());

        float timer = 4;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
            timer -= Time.deltaTime;
        }

        Destroy(gameObject);
    }

}
