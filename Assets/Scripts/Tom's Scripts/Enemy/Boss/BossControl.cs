using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{

    private GameObject player;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator animator;
    public bool canAttack;
    private bool isAlive;
    public float maxHealth = 500;
    private float health;
    private bool lookAtPlayer;
    private Vector3 target;

    private GameObject fireCircleParticleObject;
    private GameObject fireDirectionParticleObject;

    private bool charging;
    public List<GameObject> inRadius = new List<GameObject>();

    [SerializeField] private Material originalMaterial, damageMaterial;

    [SerializeField] private GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerControl.instance.gameObject;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fireCircleParticleObject = transform.GetChild(3).gameObject;
        fireDirectionParticleObject = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        target = player.transform.position;
        canAttack = true;
        charging = false;
        health = maxHealth;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;
        List<GameObject> removeRadius = new List<GameObject>();
        foreach(GameObject go in inRadius)
        {
            if ((go.transform.position - transform.position).magnitude > 3)
                removeRadius.Add(go);
        }
        foreach (GameObject go in removeRadius)
            inRadius.Remove(go);
        if (agent.remainingDistance > 0.1f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        if (lookAtPlayer)
        {
            target = player.transform.position;
            if((transform.position - player.transform.position).magnitude > 7)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
        if(charging)
        {
            foreach(GameObject go in inRadius)
            {
                if (go != null)
                {
                    if (go.tag == "Player" || go.tag == "Enemy")
                    {
                        Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                        if (rigidbody != null)
                        {
                            if (go.tag == "Enemy")
                                go.GetComponent<EnemyControl>().BossPushBackMovement();
                            rigidbody.velocity = transform.forward * 15;
                            go.SendMessage("TakeDamage", 0.2f, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }
        if (canAttack)
        {
            StartCoroutine(Attack());
        }
        LookAtTarget(target);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive)
            return;

        Debug.Log(damage);
        health -= damage;
        transform.GetChild(1).GetComponent<Renderer>().material = damageMaterial;
        transform.GetChild(2).GetComponent<Renderer>().material = damageMaterial;
        Invoke("ResetColour", 0.2f);
        if (health <= 0 && isAlive)
            Die();
    }

    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        rb.isKinematic = true;
        Destroy(gameObject, 4f);
        EnemySpawnController.instance.RemoveEnemy(gameObject);
    }

    private void ResetColour()
    {
        transform.GetChild(1).GetComponent<Renderer>().material = originalMaterial;
        transform.GetChild(2).GetComponent<Renderer>().material = originalMaterial;
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        int attackChoice = Random.Range(0, 3);
        switch (attackChoice)
        {
            case 0:
                StartCoroutine(Charge((player.transform.position - transform.position).normalized));
                break;
            case 1:
                StartCoroutine(FireCircle());
                break;
            case 2:
                StartCoroutine(FireDirection());
                break;
        }

        yield return new WaitForSeconds(Random.Range(6f, 7.5f));

        canAttack = true;
    }

    private IEnumerator Charge(Vector3 direction)
    {
        charging = true;
        lookAtPlayer = false;
        animator.SetTrigger("Charge");
        agent.isStopped = true;
        yield return new WaitForSeconds(1.25f);
        GetComponent<BoxCollider>().enabled = true;
        target = direction * 400;

        rb.isKinematic = false;
        rb.velocity = direction * 50;
        while (rb.velocity.magnitude > 3f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }

        agent.isStopped = false;
        rb.isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        lookAtPlayer = true;
        charging = false;
    }

    private IEnumerator FireCircle()
    {
        lookAtPlayer = false;
        agent.isStopped = true;
        //animator.SetTrigger("Fire");
        fireCircleParticleObject.GetComponent<ChildPartileSystemsController>().PlayChildSystems();
        yield return new WaitForSeconds(2.8f);
        List<Vector3> positions = PositionsAroundTarget(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 2, 20);
        foreach(Vector3 pos in positions)
        {
            //target = pos;
            GameObject proj = Instantiate(projectile, pos, transform.rotation);
            proj.transform.LookAt((new Vector3(proj.transform.position.x, proj.transform.position.y + 1, proj.transform.position.z) - transform.position) * 20);
            yield return new WaitForSeconds(0.05f);
        }
        //animator.SetTrigger("FireOver");
        agent.isStopped = false;
        lookAtPlayer = true;
    }

    private IEnumerator FireDirection()
    {
        lookAtPlayer = false;
        animator.SetTrigger("Fire");
        fireDirectionParticleObject.GetComponent<ChildPartileSystemsController>().PlayChildSystems();

        Vector3 targetPos = player.transform.position;

        yield return new WaitForSeconds(2f);
        int fireAmount = 5;

        List<Vector3> positions = PositionsAroundTarget(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 2, 40);
        Vector3 midPos = positions[0];
        int newIndex = 0;
        int index = 0;
        foreach(Vector3 pos in positions)
        {
            if ((pos - targetPos).magnitude < (midPos - targetPos).magnitude)
            {
                midPos = pos;
                newIndex = index;
            }
            index++;
        }

        int fireIndex = newIndex - (fireAmount / 2);
        for(int i = 0; i < 5; i++)
        {
            target = positions[fireIndex];
            GameObject proj = Instantiate(projectile, positions[fireIndex], transform.rotation);
            proj.transform.LookAt((new Vector3(proj.transform.position.x, proj.transform.position.y + 1, proj.transform.position.z) - transform.position) * 20);
            fireIndex++;
            if (fireIndex == positions.Count) fireIndex = 0;
            if (fireIndex < 0) fireIndex = 0;
        }

        animator.SetTrigger("FireOver");
        yield return new WaitForSeconds(1);

        lookAtPlayer = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") && !inRadius.Contains(other.gameObject))
        {
            inRadius.Add(other.gameObject);
            return;
        }
        if(other.gameObject.transform.parent != null)
        {
            if ((other.transform.parent.gameObject.tag == "Player" || other.transform.parent.gameObject.tag == "Enemy") && !inRadius.Contains(other.transform.parent.gameObject))
            {
                inRadius.Add(other.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRadius.Remove(other.gameObject);
            return;
        }
        if (other.gameObject.transform.parent != null)
        {
            if (other.transform.parent.gameObject.tag == "Player")
            {
                inRadius.Remove(other.transform.parent.gameObject);
            }
        }
    }

    bool LookAtTarget(Vector3 target)
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, target - gameObject.transform.position, out hit))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position), Time.deltaTime * 15);
            return true;
        }
        return false;
    }

    List<Vector3> PositionsAroundTarget(Vector3 target, float radius, int numberOfPositions)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numberOfPositions; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPositions;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius + target.x, target.y, Mathf.Sin(angle) * radius + target.z);
            positions.Add(newPos);
        }
        return positions;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            if(charging) Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

}
