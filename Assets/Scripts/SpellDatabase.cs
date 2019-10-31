using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellDatabase
{

    public static List<Spell> avaliableSpells = new List<Spell>
    {
        new Spell("IceBarrage")
    };

}

public class Spell
{
    public string name;
    public GameObject targetingObject;
    public GameObject spellObject;

    public Spell(string sName)
    {
        name = sName;

    }

}
