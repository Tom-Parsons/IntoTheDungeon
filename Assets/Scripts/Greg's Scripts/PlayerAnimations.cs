using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    Animator anim;
    Rigidbody rigid;
    PlayerControl control;
    PlayerCombat combat;
    int attackCombo = 1;

    // Start is called before the first frame update
    void Start()
    {
        control = GetComponent<PlayerControl>();
        combat = GetComponent<PlayerCombat>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        PlayerCombat2.OnAttack += StartAttackAnim;
    }

    // Update is called once per frame
    void Update()
    {
        float rA = anim.GetFloat("RunAngle");
        float targetRA = Vector3.SignedAngle(control.targetDir, transform.GetChild(0).forward, Vector3.up);
        float newRA = Mathf.LerpAngle(rA, targetRA, Time.deltaTime * 20);
        if (newRA > 180)
            newRA -= 360;
        if (newRA < -180)
            newRA += 360;
        anim.SetFloat("RunAngle", newRA);
        if (control.targetDir.magnitude > 0.5f)
            anim.SetBool("isMoving", true);
        else
            anim.SetBool("isMoving", false);
    }

    void StartAttackAnim ()
    {
        //anim.SetBool("TryingToAttack", true);
        //Invoke("AttackStarted",0.1f);
    }

    void AttackStarted ()
    {
        //anim.SetBool("TryingToAttack", false);
    }

    void DealDamage ()
    {

    }
}
