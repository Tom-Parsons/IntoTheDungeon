using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMenuRun : MonoBehaviour
{

    public GameObject point1, point2;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("IsRunning", true);
        StartCoroutine(RunAround());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RunAround ()
    {
        float speed = Random.Range(2.0f,4.0f);
        while (true)
        {
            transform.LookAt(point1.transform.position);
            yield return new WaitForSeconds(Random.Range(1, 6));
            while (Vector3.Distance(transform.position, point1.transform.position) > 0.5f)
            {
                transform.position += ((point1.transform.position - transform.position).normalized * Time.deltaTime * speed);
                yield return new WaitForEndOfFrame();
            }
            transform.LookAt(point2.transform.position);
            yield return new WaitForSeconds(Random.Range(1, 6));
            while (Vector3.Distance(transform.position, point2.transform.position) > 0.5f)
            {
                transform.position += ((point2.transform.position - transform.position).normalized * Time.deltaTime * speed);
                yield return new WaitForEndOfFrame();
            }


        }
    }

}
