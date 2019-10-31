using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimationMessages : MonoBehaviour
{

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void AnimationEvent (string funName)
    {
        target.SendMessage(funName);
    }
}
