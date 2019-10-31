using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "Enemy";
    }

    void TakeDamage (float damage)
    {
        Instantiate(EffectsDatabase.GetEffect("BarrelExplosion"), transform.position, EffectsDatabase.GetEffect("BarrelExplosion").transform.rotation);
        Destroy(gameObject);
    }
}
