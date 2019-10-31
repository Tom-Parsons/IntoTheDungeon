using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    public GameObject cameraHorPivot;
    Rigidbody rigid;
    ConstantForce cf;
    PlayerStats stats;
    PlayerCombat2 combat;
    public Vector3 targetDir;
    public ParticleSystem dashSmoke;
    Animator anim;

    string lastDirPressed;
    float lastDirPressedTime;

    public bool canJump = true, canMove = true, canRotate = true, canDash = true;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        cf = GetComponent<ConstantForce>();
        rigid = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        combat = GetComponent<PlayerCombat2>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDir = (cameraHorPivot.transform.forward * Input.GetAxisRaw("Vertical") + cameraHorPivot.transform.right * Input.GetAxisRaw("Horizontal")).normalized;
        if (canMove)
        {
            if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
                Move();
            else
                rigid.velocity = Vector3.Lerp(rigid.velocity, new Vector3(0, rigid.velocity.y, 0), Time.deltaTime * 10);

            if (Input.GetButtonDown("Jump") && canJump)
            {
                Jump();
                cf.force = -Vector3.up * 5;
            }
            else if (Input.GetButtonUp("Jump") || rigid.velocity.y < 0)
                cf.force = -Vector3.up * 10;


            if (Input.GetKeyDown(KeyCode.W)) DirectionPressed("W");
            if (Input.GetKeyDown(KeyCode.S)) DirectionPressed("S");
            if (Input.GetKeyDown(KeyCode.A)) DirectionPressed("A");
            if (Input.GetKeyDown(KeyCode.D)) DirectionPressed("D");

        }

        if (canDash && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButton(1)) && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Dash")
        {
            canDash = false;
            StopAllCoroutines();
            StartCoroutine(Dash(targetDir));
        }


        if (canRotate)
            LookAtMouse();

    }

    void Move ()
    {
        rigid.velocity = Vector3.Lerp(rigid.velocity, new Vector3(targetDir.x * stats.speed, rigid.velocity.y, targetDir.z * stats.speed), Time.deltaTime * 20);
    }

    void LookAtMouse ()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.LookRotation(new Vector3(hit.point.x, transform.GetChild(0).position.y, hit.point.z) - transform.GetChild(0).position), Time.deltaTime * 30);
        }
    }

    void Jump ()
    {
        canDash = false;
        canJump = false;
        rigid.velocity = new Vector3(rigid.velocity.x, stats.jumpHeight, rigid.velocity.z);
        anim.ResetTrigger("Landed");
        anim.SetTrigger("StartJump");
    }

    void OnCollisionEnter (Collision col)
    {
        if (canJump)
            return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.distance < 1.1f)
            {
                if (canRotate)
                canDash = true;
                canJump = true;
                anim.SetTrigger("Landed");
            }
        }
    }

    IEnumerator Dash (Vector3 dashDir)
    {
        Debug.Log("DashStarted");
        if (dashDir.magnitude == 0)
            dashDir = transform.GetChild(0).forward ;

        transform.GetChild(0).LookAt(transform.GetChild(0).position + dashDir);
        anim.SetTrigger("StartDash");

        rigid.velocity = dashDir * 30;

        combat.canAttack = false;
        canRotate = false;
        canMove = false;
        canJump = false;
        dashSmoke.Play();
        CameraFunctions.AddShake(0.2f);
        while (rigid.velocity.magnitude > 3f)
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, Vector3.zero, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }
        canRotate = true;
        dashSmoke.Stop();
        canJump = true;
        canMove = true;
        combat.canAttack = true;
        canDash = true;

    }

    public void AttackMovement ()
    {
        if (!canDash)
            return;

        StopAllCoroutines();
        StartCoroutine(AttackDash());
    }

    IEnumerator AttackDash()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            transform.GetChild(0).rotation = Quaternion.LookRotation(new Vector3(hit.point.x, transform.GetChild(0).position.y, hit.point.z) - transform.GetChild(0).position);
        }

        rigid.velocity = transform.GetChild(0).forward * 10 / stats.timeBetweenSwings;
        while (rigid.velocity.magnitude > 0.1f)
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, Vector3.zero, Time.deltaTime / stats.timeBetweenSwings * 5);
            yield return new WaitForEndOfFrame();
        }
    }

    void DirectionPressed (string key)
    {
        if (key == lastDirPressed && Time.time - lastDirPressedTime < 0.2f && canMove && canJump)
            switch(key)
            {
                case "W":
                    StartCoroutine(Dash(transform.forward));
                    break;
                case "S":
                    StartCoroutine(Dash(-transform.forward));
                    break;
                case "A":
                    StartCoroutine(Dash(-transform.right));
                    break;
                case "D":
                    StartCoroutine(Dash(transform.right));
                    break;
            }
        else if (key != lastDirPressed)
        {
            lastDirPressed = key;
        }
        lastDirPressedTime = Time.time;
    }

}
