using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyControl : MonoBehaviour
{

    public enum EnemyType {
        MELEE,
        RANGED
    }

    NavMeshAgent agent;
    GameObject player;
    EnemySpawnController spawnController;
    EnemyStats stats;
    Rigidbody rigid;

    public EnemyType type = EnemyType.MELEE;

    public bool canMove;
    private bool isDead;
    private bool canDecide;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        spawnController = EnemySpawnController.instance;
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        stats = GetComponent<EnemyStats>();
        Invoke("getPlayer", 0.1f);
        anim = transform.GetChild(0).GetComponent<Animator>();
        canMove = true;
        isDead = true;
        canDecide = true;
    }

    void getPlayer()
    {
        player = PlayerControl.instance.gameObject;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            rigid.isKinematic = true;
            rigid.velocity = Vector3.zero;
            if(agent != null) agent.isStopped = true;
            return;
        }
        Debug.DrawLine(transform.position, agent.pathEndPosition);
        if (!canMove)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            agent.isStopped = false;
            rigid.isKinematic = true;
        }

        if (agent.remainingDistance < 0.1 )
        {
            anim.SetBool("IsRunning", false);
        }
        else
        {
            anim.SetBool("IsRunning", true);
        }

        //If they are meant to be attacking the player
        if (EnemySpawnController.instance.IsAttackingEnemy(gameObject))
        {
            if (type == EnemyType.MELEE)
            {
                //If they can't see the player or are too far away, then move towards them otherwise they will automatically look at the player
                if (!LookAtPlayerClose())
                {
                    Move(player.gameObject.transform.position);
                }
            }
            else if(type == EnemyType.RANGED)
            {
                Vector3 direction = player.transform.position - gameObject.transform.position;
                float distance = direction.magnitude;
                if (distance > 11)
                {
                    MoveNearToPlayer();
                }
            }
        }
        //They aren't attacking the player
        else
        {
            if(CanSeePlayer()) LookAtPlayer();


            if (DistanceFromPlayer() < 2)
            {
                spawnController.MakeNearestEnemyAttack();
            }else if(DistanceFromPlayer() > 6)
            {
                if(DistanceFromPlayer() > 10)
                {
                    Move(player.transform.position);
                }
                else
                {
                    MoveNearToPlayer();
                }
                return;
            }


            if (type == EnemyType.MELEE)
            {
                Vector3 direction = player.transform.position - gameObject.transform.position;
                //Get the distance of the enemy from the player
                float distance = direction.magnitude;

                if (agent.remainingDistance < 3 || distance > 11)
                {
                    if (canDecide) StartCoroutine(DecideToMove());
                }
            }
            else if (type == EnemyType.RANGED)
            {
                if (!CanSeePlayerFromPosition(transform.position))
                {
                    if(canDecide) StartCoroutine(DecideToMove());
                }
            }
        }
    }

    IEnumerator DecideToMove()
    {
        canDecide = false;

        MoveNearToPlayer();

        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));

        canDecide = true;
    }
    
    void MoveNearToPlayer()
    {
        float radius = 6;
        List<Vector3> positionsNearPlayer = PositionsAroundPlayer(radius, 30);
        Random rnd = new Random();
        Vector3 pos = positionsNearPlayer[Random.Range(0, positionsNearPlayer.Count - 1)];
        bool possible = false;
        while (!possible)
        {
            //TODO: make closest position the pos to use
            pos = positionsNearPlayer[Random.Range(0, positionsNearPlayer.Count - 1)];
            //while((positionsNearPlayer.Count > 0) && ((pos - gameObject.transform.position).magnitude < 2.1))
            //{
            //    pos = positionsNearPlayer[Random.Range(0, positionsNearPlayer.Count - 1)];
            //    posss = pos;
            //    positionsNearPlayer.Remove(pos);
            //}
            //RaycastHit hit;
            //possible = Physics.Raycast(pos, player.gameObject.transform.position - pos, out hit);
            //if (!possible)
            //{
            //    positionsNearPlayer.Remove(pos);
            //}
            //Debug.Log(positionsNearPlayer.Count);
            if (CanSeePlayerFromPosition(pos))
            {
                possible = true;
            }
            else
            {
                positionsNearPlayer.Remove(pos);
            }
            if (possible)
            {
                foreach (GameObject enemy in EnemySpawnController.instance.getEnemies())
                {
                    if ((pos - enemy.GetComponent<EnemyControl>().agent.destination).magnitude < 2)
                    {
                        positionsNearPlayer.Remove(pos);
                        possible = false;
                    }
                }
            }
            if (positionsNearPlayer.Count == 0 && possible == false)
            {
                //possible = true;
                //pos = player.gameObject.transform.position;
                radius -= 0.5f;
                positionsNearPlayer = PositionsAroundPlayer(radius, 30);
                //Debug.Log("Smaller " + radius);
            }
        }
        //Debug.Log("Moving");
        Move(pos);
    }

    void MoveAwayFromPlayer()
    {
        //Find a position away from the player to keep the enemy safe but nearby incase they need to attack
        Vector3 normalized = (player.transform.position - transform.position);
        normalized.Normalize();
        NavMeshHit hit;
        if (!NavMesh.Raycast(player.transform.position, normalized * -8, out hit, NavMesh.AllAreas))
        {
            Move(player.transform.position + (normalized * -8));
            Debug.DrawLine(player.transform.position, player.transform.position + (normalized * -8), Color.green);
        }
        else
        {
            Debug.DrawLine(player.transform.position, player.transform.position + (normalized * -8), Color.red);
        }
    }

    void Move(Vector3 target)
    {
        if(canMove) agent.SetDestination(target);
    }

    bool LookAtPlayerClose()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, out hit))
        {
            if (hit.distance < 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position), Time.deltaTime * 30);
                return true;
            }
        }
        return false;
    }

    bool LookAtPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, out hit))
        {
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position), Time.deltaTime * 30);
             return true;
        }
        return false;
    }

    float DistanceFromPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, player.gameObject.transform.position - gameObject.transform.position, out hit))
        {
            return hit.distance;
        }
        return int.MaxValue;
    }

    public void AttackMovement()
    {
        StopAllCoroutines();
        StartCoroutine(AttackDash());
    }

    public void PushBackMovement()
    {
        StopAllCoroutines();
        StartCoroutine(PushBack(player.transform.position));
    }

    public void BossPushBackMovement()
    {
        StopAllCoroutines();
        StartCoroutine(BossPushBack());
    }

    IEnumerator AttackDash()
    {
        canMove = false;
        rigid.isKinematic = false;
        agent.isStopped = true;

        rigid.velocity = transform.forward * 4 / stats.timeBetweenSwings;
        while (rigid.velocity.magnitude >= 3)
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, Vector3.zero, Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }


        //Debug.Log("kinematic");
        rigid.isKinematic = true;
        agent.isStopped = false;
        canMove = true;
    }

    IEnumerator PushBack(Vector3 fromPosition)
    {
        canMove = false;
        rigid.isKinematic = false;
        agent.isStopped = true;

        rigid.velocity = ((transform.position - fromPosition).normalized) * 4 / stats.timeBetweenSwings;
        while(rigid.velocity.magnitude >= 3)
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, Vector3.zero, Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("kinematic");
        rigid.isKinematic = true;
        agent.isStopped = false;
        canMove = true;
    }

    IEnumerator BossPushBack()
    {
        canMove = false;
        rigid.isKinematic = false;
        agent.isStopped = true;

        yield return new WaitForSeconds(2f);

        //Debug.Log("kinematic");
        rigid.isKinematic = true;
        agent.isStopped = false;
        canMove = true;
    }

    List<Vector3> PositionsAroundPlayer(float radius, int numberOfPositions)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numberOfPositions; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPositions;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius + player.transform.position.x, player.transform.position.y, Mathf.Sin(angle) * radius + player.transform.position.z);
            positions.Add(newPos);
        }
        return positions;
    }

    bool CanSeePlayerFromPosition(Vector3 position)
    {
        RaycastHit hit;
        Debug.DrawRay(position, player.gameObject.transform.position - position);
        if(Physics.Raycast(position, player.gameObject.transform.position - position, out hit))
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

    bool CanSeePlayer()
    {
        return CanSeePlayerFromPosition(transform.position);
    }

    public void Die()
    {
        isDead = true;
        canMove = false;
        agent.isStopped = true;
        rigid.velocity = Vector3.zero;
        Destroy(GetComponent<CapsuleCollider>(), 0.5f);
    }

}
