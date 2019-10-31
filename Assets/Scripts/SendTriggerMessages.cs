using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTriggerMessages : MonoBehaviour
{
    public GameObject receiver;

    private void OnTriggerEnter (Collider col)
    {
        receiver.SendMessage("TriggerEntered", new GameObject[] { gameObject, col.gameObject });
    }

    private void OnTriggerExit(Collider col)
    {
        receiver.SendMessage("TriggerExited", new GameObject[] { gameObject, col.gameObject });
    }

}
