using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{

    Light light;
    float flickerSpeed = 5;
    float maxIntensity;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        maxIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float targetIntensity = Mathf.PerlinNoise(0, Time.time * flickerSpeed) * maxIntensity * .1f + maxIntensity * .9f;
        if (targetIntensity < maxIntensity*.95f)
            light.intensity = targetIntensity;
        else
            light.intensity = maxIntensity;

    }
}
