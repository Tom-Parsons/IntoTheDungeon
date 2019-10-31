using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectsDatabase
{
    public static List<GameObject> availableEffect = new List<GameObject>()
    {
        Resources.Load("Effects/Hit") as GameObject,
        Resources.Load("Effects/BarrelExplosion") as GameObject
    };

    public static GameObject GetEffect (string name)
    {
        GameObject toReturn = availableEffect.Find(x => x.name == name);
        if (toReturn)
        {
            return toReturn;
        }
        else
            return null;
    }

}
