using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleFireball : MonoBehaviour
{

    float movSpeed = 2f, rotSpeed = 300f;
    bool isMoving;
    float damage;
    public GameObject[] particles;
    public List<GameObject> damagedEnemies;

    // Start is called before the first frame update
    void Start()
    {
        damagedEnemies = new List<GameObject>();
        transform.position += Vector3.up * 0.75f + transform.forward * 1.6f;
        Invoke("Die", 13.00f / movSpeed);
    }

    void SetDamage(float dmg)
    {
        damage = dmg;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
            return;

        transform.Translate(Vector3.forward * movSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving && other.tag == "Enemy" && !damagedEnemies.Contains(other.gameObject))
        {
            other.SendMessage("TakeDamage", damage);
            damagedEnemies.Add(other.gameObject);
        }
    }

    void StartForward ()
    {
        isMoving = true;
    }

    void Die ()
    {
        for (int i = 0; i < 2; i++)
        {
            if (particles.Length > i)
            particles[i].transform.parent = null;
            particles[i].transform.localScale = Vector3.one * 10;
            particles[i].SetActive(true);
            particles[i].GetComponent<ParticleSystem>().Stop();
        }

        Destroy(gameObject);
    }

}
