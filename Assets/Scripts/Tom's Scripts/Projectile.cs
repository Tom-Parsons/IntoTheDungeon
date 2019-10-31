using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody rb;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        Vector3 dir = player.transform.position - rb.transform.position;
        dir += Vector3.up * 5;

        rb.velocity = (dir);
        Destroy(gameObject, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TriggerEntered(GameObject[] info)
    {
        if (info[0].name == "Hitbox_Projectile" && (info[1].tag == "Enemy" || info[1].tag == "Player"))
        {
            transform.parent = info[1].transform.GetChild(0);
            rb.isKinematic = true;
            GetComponent<SphereCollider>().isTrigger = true;
            if (info[0].name == "Hitbox_Projectile" && info[1].tag == "Player")
            {
                info[1].SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject, 2);
            }
            Destroy(gameObject, Random.Range(10, 15));
        }
    }

    void TriggerExited(GameObject[] info)
    {
    }

}
