using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBarrage : MonoBehaviour
{

    List<Vector3> targets;
    List<Vector3> icycleOrigins;
    GameObject icycle;
    float damage;

    void CastSpell (GameObject targeting)
    {
        icycle = Resources.Load("Spells/Icycle") as GameObject;
        icycleOrigins = new List<Vector3>();
        targets = new List<Vector3>();
        for (int i = 0; i < 5; i++)
        {
            targets.Add(targeting.transform.GetChild(i).position);
        }
        StartCoroutine(ShootIcycles());
    }

    void SetDamage (float dmg)
    {
        damage = dmg;
    }

    IEnumerator ShootIcycles()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            icycleOrigins.Add(transform.GetChild(0).GetChild(0).GetChild(i).position);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            GameObject GO = Instantiate(icycle, icycleOrigins[i], Quaternion.identity);
            GO.transform.LookAt(targets[i]);
            SpellProjectile proj = GO.GetComponent<SpellProjectile>();
            proj.endRot = transform.rotation * Quaternion.Euler(60 - i*8 + Random.Range(-10,10), Random.Range(-5, 5), Random.Range(-5, 5));
            proj.finalDest = targets[i];
            proj.damage = damage;
            GO.GetComponent<Rigidbody>().AddForce(GO.transform.forward * 1500);
            yield return new WaitForSeconds(0.05f);
        }
    }

}
