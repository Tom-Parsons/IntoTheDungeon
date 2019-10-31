using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFunctions : MonoBehaviour
{

    float randPosXSeed, randPosYSeed, randPosZSeed;
    float shakeMagnitude = 0.5f;
    float trauma = 0f;
    float shakeSpeed = 80;
    public static CameraFunctions instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        randPosXSeed = Random.Range(0.00f, 1.00f);
        randPosYSeed = Random.Range(0.00f, 1.00f);
        randPosZSeed = Random.Range(0.00f, 1.00f);
    }

    // Update is called once per frame
    void Update()
    {
        if (trauma < 0.01f)
            return;
        trauma = Mathf.Lerp(trauma, 0.00f, Time.deltaTime * 2);
        Vector3 addPos = new Vector3(Mathf.PerlinNoise(randPosXSeed, Time.time * shakeSpeed) - 0.5f, Mathf.PerlinNoise(randPosYSeed, Time.time * shakeSpeed) - 0.5f, Mathf.PerlinNoise(randPosZSeed, Time.time * shakeSpeed) - 0.5f) * shakeMagnitude * trauma;
        transform.position = transform.parent.position + addPos;
    }

    public static void AddShake (float newTrauma)
    {
        instance.trauma += newTrauma;
        if (newTrauma > 0) newTrauma = 1;
    }

}
