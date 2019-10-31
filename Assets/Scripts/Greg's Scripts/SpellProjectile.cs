using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{

    public float stopDelay;
    public Quaternion endRot;
    public Vector3 finalDest;
    public float damage;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
            Invoke("Stop", stopDelay);
        else if (other.tag == "Enemy")
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Enemy")
    //        enemiesInside.Remove(other.gameObject);
    //}

    void Stop ()
    {
        //for (int i = 0; i < enemiesInside.Count; i++)
        //{
        //    enemiesInside[i].SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        //}
        CameraFunctions.AddShake(0.1f);
        Destroy(GetComponent<Rigidbody>());
        transform.rotation = endRot;
        transform.position = new Vector3(finalDest.x, transform.position.y, finalDest.z);
        Invoke("Die", 3);
    }

    void Die ()
    {
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).parent = null;
        Destroy(gameObject);
    }

}
