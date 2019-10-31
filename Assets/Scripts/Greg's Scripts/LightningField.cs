using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningField : MonoBehaviour
{

    List<GameObject> enemiesInside;
    float damage;
    float startTime;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        enemiesInside = new List<GameObject>();
        InvokeRepeating("DealDamage", 1, 0.2f);
        Destroy(gameObject, 7);
        player = GameObject.Find("Player");
    }

    void DealDamage ()
    {
        if (startTime == 0)
            startTime = Time.time;
        if (Time.time - startTime > 5)
            return;

        for (int i = 0; i < enemiesInside.Count; i++)
        {
            if (enemiesInside[i])
                enemiesInside[i].SendMessage("TakeDamage", damage);
            else
            {
                enemiesInside.RemoveAt(i);
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void CastSpell(GameObject targeting)
    {
        transform.rotation = Quaternion.Euler(90,0,0);
        transform.position = targeting.transform.position + Vector3.up * 12f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            enemiesInside.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
            enemiesInside.Remove(other.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        CameraFunctions.AddShake(0.07f / (1 + Vector3.Distance(transform.position, player.transform.position)/10));
    }

}
