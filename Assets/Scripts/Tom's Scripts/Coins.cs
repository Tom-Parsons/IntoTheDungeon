using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float jumpForce = 3f;
    
    private bool animate;

    private float animateSpeed = 2f;
    private float animateHeight = 0.5f;
    private float timeAlive;

    Vector3 originalPos;

    // Start is called before the first frame update
    private void Start()
    {
        originalPos = transform.position;
        rb = GetComponent<Rigidbody>();

        rb.velocity = new Vector3(Random.Range(-jumpForce, jumpForce), jumpForce, Random.Range(-jumpForce, jumpForce));

        timeAlive = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (animate)
        {
            float offset = Mathf.Sin(timeAlive * animateSpeed) * animateHeight / 2f;
            transform.position = new Vector3(transform.position.x, originalPos.y + offset + (animateHeight + 1f), transform.position.z);
            float xoffset = Random.Range(0f, 180f) * Time.deltaTime;
            float yoffset = Random.Range(0f, 90f) * Time.deltaTime;
            float zoffset = Random.Range(0f, 90f) * Time.deltaTime;
            transform.Rotate(new Vector3(xoffset, yoffset, zoffset));
        }
        timeAlive += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Floor"))
        {
            //Collided with the floor - begin animation
            animate = true;
            rb.velocity = Vector3.zero;
        }else if(other.tag == "Player" && animate)
        {
            other.SendMessage("AddMoney",1);
            //PLAY SOUND
            Destroy(gameObject, 0.2f);
        }
    }

}
