using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTriggerMessages : MonoBehaviour
{
    public GameObject receiver;

    private void OnTriggerEnter (Collider col)
    {
        if (col.tag == "Enemy" || col.tag == "Player")
        receiver.SendMessage("TriggerEntered", new GameObject[] { gameObject, col.gameObject }, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider col)
    {
        receiver.SendMessage("TriggerExited", new GameObject[] { gameObject, col.gameObject }, SendMessageOptions.DontRequireReceiver);
    }

}
