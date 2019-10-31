using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffects : MonoBehaviour
{

    float lastTimeHit;
    public static CombatEffects instance;

    private void Start()
    {
        instance = this;
    }

    public static void GotHit (GameObject damaged)
    {
        instance.lastTimeHit = Time.realtimeSinceStartup;
        CameraFunctions.AddShake(0.05f);
        GameObject GO = EffectsDatabase.GetEffect("Hit");
        GameObject GO2 = Instantiate(GO, damaged.transform.position, Quaternion.identity);
        Destroy(GO2, 1);
        //instance.StopAllCoroutines();
        //instance.StartCoroutine(instance.SlowTime());
    }

    IEnumerator SlowTime ()
    {
        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

    }

}
