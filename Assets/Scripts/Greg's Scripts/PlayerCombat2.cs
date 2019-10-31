
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat2 : MonoBehaviour
{

    List<GameObject> inStaffRange, inSlamRange;
    PlayerControl movement;
    public MeshRenderer swingHitboxModel, slamHitboxModel;
    public bool canAttack = true;
    PlayerStats stats;
    public bool wantsToRepeatAttack;
    Animator anim;
    float lastAttackEndTime;
    int attackCombo;

    public bool attackLocksMovement, attackLocksRotation;

    public delegate void ClickAction();
    public static event ClickAction OnAttack;

    float lastTimeHit;

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
        if (anim.speed != 1 && Time.time - lastTimeHit > 0.06f)
            anim.speed = 1;
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            canAttack = false;
            StopAllCoroutines();
            StartCoroutine(Attack());
        }else if (Input.GetMouseButtonDown(0) && movement.canJump)
        {            
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

    void AttackMovement ()
    {
        wantsToRepeatAttack = false;
        movement.canRotate = false;
        movement.canMove = false;
    }

    void DealDamage ()
    {
        if (inStaffRange.Count > 0)
        {
            anim.speed = 0.0f;
            lastTimeHit = Time.time;
        }
        for (int i = 0; i < inStaffRange.Count; i++)
        {
            if (inStaffRange[i] == null || inStaffRange[i].GetComponent<Collider>() == null)
            {
                inStaffRange.RemoveAt(i);
                i--;
                continue;
            }
            inStaffRange[i].SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
            PlayerStats.AddMana(5*(1+stats.additionallManaGain));
        }
    }

    IEnumerator Attack ()
    {
        OnAttack();
        RaycastHit hit; Physics.Raycast(transform.position, -Vector3.up, out hit);
        if (hit.distance < 1.2f)
        {
            anim.SetTrigger("TryingToAttack");
        }
        else
        {
            movement.canDash = false;
            anim.SetTrigger("StartSmash");
            movement.canMove = false;
            movement.canRotate = false;
            GetComponent<Rigidbody>().velocity = Vector3.up * 10;

            while (!movement.canJump)
            {
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < inStaffRange.Count; i++)
            {
                inStaffRange[i].SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
            }
            slamHitboxModel.enabled = true;
            slamHitboxModel.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            CameraFunctions.AddShake(0.6f);
            yield return new WaitForSeconds(0.75f);
            anim.SetTrigger("Landed");
            slamHitboxModel.enabled = false;
            movement.canDash = true;
            movement.canMove = true;
            movement.canRotate = true;
            canAttack = true;
        }
    }

    void SwingEnded()
    {
        if (!wantsToRepeatAttack)
        {
            canAttack = true;
            movement.canRotate = true;
            movement.canMove = true;
            Invoke("CheckForResetCombo", 1);
        }else 
        {
            wantsToRepeatAttack = false;
            if (movement.canDash)
                anim.SetTrigger("TryingToAttack");
        }
        lastAttackEndTime = Time.time;

        if (attackCombo == 3)
            attackCombo = 1;
        else attackCombo++;
        anim.SetFloat("AttackCombo", attackCombo);

    }

    void CheckForResetCombo ()
    {
        if (Time.time - lastAttackEndTime > 0.95f)
        {
            attackCombo = 1;
            anim.SetFloat("AttackCombo", attackCombo);
        }
    }


}
