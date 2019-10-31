using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellDatabase
{

    public static List<Spell> avaliableSpells = new List<Spell>
    {
        new Spell("IceBarrage", 50, 2.5f, false),
        new Spell("DoubleFireball", 75, 5, false),
        new Spell("LightningField", 100, .5f, true)

    };

}

public class Spell
{
    public string name;
    public float manaCost;
    public float damage;
    public bool detachableTargeting;
    public GameObject targetingObject;
    public GameObject spellObject;

    public Spell(string sName, float sManaCost, float sDamage, bool sDetachableTargeting)
    {
        name = sName;
        manaCost = sManaCost;
        damage = sDamage;
        detachableTargeting = sDetachableTargeting;
        targetingObject = Resources.Load("Spells/" + name + "Targeting") as GameObject;
        spellObject = Resources.Load("Spells/" + name + "Object") as GameObject;
    }

}
