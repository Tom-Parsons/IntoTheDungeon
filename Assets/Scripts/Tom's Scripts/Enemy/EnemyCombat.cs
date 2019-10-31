using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    List<GameObject> inStaffRange;
    List<GameObject> inBowRange;
    EnemyControl movement;
    public MeshRenderer swingHitboxModel;
    public MeshRenderer bowHitboxModel;
    public bool canAttack = true;
    EnemyStats stats;
    private GameObject player;
    private float reFireTimer;

    public GameObject projectile;

    private Animator anim;

    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        inStaffRange = new List<GameObject>();
        inBowRange = new List<GameObject>();
        movement = GetComponent<EnemyControl>();
        stats = GetComponent<EnemyStats>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        player = PlayerControl.instance.gameObject;
        reFireTimer  = Random.Range(1.2f, 2.3f);
        isDead = false;
        canAttack = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
        if (!movement.canMove)
            return;
        if(movement.type == EnemyControl.EnemyType.MELEE && DistanceFromPlayer() < 2.5 && canAttack)
        {
            StartCoroutine(Attack());
        }else if(movement.type == EnemyControl.EnemyType.RANGED && DistanceFromPlayer() < 10 && canAttack && CanSeePlayerFromPosition(transform.position))
        {
            StartCoroutine(Attack());
        }
    }

    void TriggerEntered(GameObject[] info)
    {
        if (info[0].name == "Hitbox_StaffSwing" && info[1].tag == "Player")
        {
            inStaffRange.Add(info[1]);
        }
        else if (info[0].name == "Hitbox_BowFire" && info[1].tag == "Player")
        {
            inBowRange.Add(info[1]);
        }
    }

    void TriggerExited(GameObject[] info)
    {
        if (info[0].name == "Hitbox_StaffSwing" && info[1].tag == "Player")
        {
            inStaffRange.Remove(info[1]);
        }
        else if (info[0].name == "Hitbox_BowFire" && info[1].tag == "Player")
        {
            inBowRange.Remove(info[1]);
        }
    }

    //I have used the same system for attacking as used for player attacking, I just removed unecessary parts
    IEnumerator Attack()
    {
        if (movement.type == EnemyControl.EnemyType.MELEE)
        {
            canAttack = false;
            anim.SetTrigger("Attack");
            movement.AttackMovement();

            //swingHitboxModel.enabled = true;
            yield return new WaitForSeconds(stats.timeBetweenSwings);
            //swingHitboxModel.enabled = false;
            canAttack = true;
        }
        else if (movement.type == EnemyControl.EnemyType.RANGED)
        {
            if(reFireTimer > 0)
            {
                reFireTimer -= Time.deltaTime;
                if(reFireTimer <= 0)
                {
                    anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(1.5f);
                    GameObject pro = Instantiate(projectile);
                    pro.transform.rotation = transform.rotation;
                    pro.transform.position = gameObject.transform.position + gameObject.transform.forward * 2;

                    reFireTimer = Random.Range(1.8f, 2.3f);
                }
            }
        }
        //movement.canRotate = true;
        canAttack = true;
    }

    void AttackWithDamage()
    {
        for (int i = 0; i < inStaffRange.Count; i++)
        {
            inStaffRange[i].SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    //Get the distance from the player
    float DistanceFromPlayer()
    {
        return (player.transform.position - gameObject.transform.position).magnitude;
    }

    bool CanSeePlayerFromPosition(Vector3 position)
    {
        RaycastHit hit;
        Debug.DrawRay(position, player.gameObject.transform.position - position);
        if (Physics.Raycast(position, player.gameObject.transform.position - position, out hit))
        {
            if (hit.collider.transform.parent != null)
            {
                if (hit.collider.transform.parent.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Die()
    {
        isDead = true;
    }

    public bool isEnemyDead()
    {
        return isDead;
    }

}
