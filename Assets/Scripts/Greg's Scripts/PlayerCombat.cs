using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    List<GameObject> inStaffRange, inSlamRange;
    PlayerControl movement;
    public MeshRenderer swingHitboxModel, slamHitboxModel;
    public bool canAttack = true;
    PlayerStats stats;
    bool wantsToRepeatAttack;
    Animator anim;

    public bool attackLocksMovement, attackLocksRotation;

    public delegate void ClickAction();
    public static event ClickAction OnAttack;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.1f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        inStaffRange = new List<GameObject>();
        inSlamRange = new List<GameObject>();
        movement = GetComponent<PlayerControl>();
        stats = GetComponent<PlayerStats>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!movement.canMove)
        //    return;
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StopAllCoroutines();
            StartCoroutine(Attack());
        }else if (Input.GetMouseButtonDown(0) && movement.canJump)
        {
            anim.SetTrigger("TryingToAttack");
            wantsToRepeatAttack = true;
        }
    }

    void TriggerEntered(GameObject[] info)
    {
        if (info[0].name == "Hitbox_StaffSwing" && info[1].tag == "Enemy")
        {
            inStaffRange.Add(info[1]);
        }
    }

    void TriggerExited(GameObject[] info)
    {
        if (info[0].name == "Hitbox_StaffSwing" && info[1].tag == "Enemy")
        {
            inStaffRange.Remove(info[1]);
        }
    }

    //void AttackMovement ()
    //{
    //    wantsToRepeatAttack = false;
    //    movement.canRotate = false;
    //    movement.canMove = false;
    //}

    //void DealDamage ()
    //{
    //    for (int i = 0; i < inStaffRange.Count; i++)
    //    {
    //        inStaffRange[i].SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
    //    }
    //}

    IEnumerator Attack ()
    {
        OnAttack();
        RaycastHit hit; Physics.Raycast(transform.position, -Vector3.up, out hit);
        canAttack = false;
        if (hit.distance < 1.2f)
        {
            anim.SetTrigger("TryingToAttack");
        }
        else
        {
            movement.canMove = false;
            movement.canRotate = false;
            GetComponent<Rigidbody>().velocity = Vector3.up * 5;

            for (int i = 0; i < inStaffRange.Count; i++)
            {
                inStaffRange[i].SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
            }
            RaycastHit hit2; Physics.Raycast(transform.position, -Vector3.up, out hit2);
            slamHitboxModel.transform.position = hit2.point;
            slamHitboxModel.enabled = true;
            slamHitboxModel.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            CameraFunctions.AddShake(0.2f);
            while (!movement.canJump)
            {
                yield return new WaitForEndOfFrame();
            }
            slamHitboxModel.enabled = false;
            movement.canMove = true;
            movement.canRotate = true;
            canAttack = true;
        }
    }

    void SwingEnded()
    {
        if (!wantsToRepeatAttack)
        {
            //anim.SetBool("TryingToAttack", false);
            //movement.canRotate = true;
            //movement.canMove = true;
        }
    }


}
