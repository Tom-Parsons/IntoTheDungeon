using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildPartileSystemsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayChildSystems ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ParticleSystem sys = transform.GetChild(i).GetComponent<ParticleSystem>();
            if (sys != null)
                sys.Play();
        }
    }


    public void StopChildSystems()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ParticleSystem sys = transform.GetChild(i).GetComponent<ParticleSystem>();
            if (sys != null)
                sys.Stop();
        }
    }

}
