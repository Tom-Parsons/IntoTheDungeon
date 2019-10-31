using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpells : MonoBehaviour
{

    Spell currentSpell;
    GameObject targetingObject;
    Animator anim;
    PlayerCombat2 combat;
    PlayerControl movement;
    PlayerStats stats;
    public bool isCasting;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        combat = GetComponent<PlayerCombat2>();
        movement = GetComponent<PlayerControl>();
        stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetingObject && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)))
            CancelCasting();

        if (!movement.canJump)
            return;

        if (isCasting)
        {

            if (targetingObject)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    targetingObject.transform.position = transform.position - Vector3.up * 0.75f;
                    if (currentSpell.detachableTargeting)
                    targetingObject.transform.position = hit.point + Vector3.up/10;
                    Vector3 newTargetingPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    targetingObject.transform.rotation = Quaternion.LookRotation(newTargetingPos - transform.position);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (stats.mana < currentSpell.manaCost)
                    {
                        CancelCasting();
                        Debug.Log("Insufficient Mana");
                        CancelCasting();
                    }else
                    {
                        PlayerStats.AddMana(-currentSpell.manaCost);
                        transform.GetChild(0).rotation = Quaternion.LookRotation(new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position);
                        GameObject GO = Instantiate(currentSpell.spellObject, transform.position, transform.GetChild(0).rotation);
                        GO.SendMessage("SetDamage", currentSpell.damage * stats.damage, SendMessageOptions.DontRequireReceiver);
                        anim.SetTrigger("CastSpell");
                        GO.SendMessage("CastSpell", targetingObject, SendMessageOptions.DontRequireReceiver);
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        movement.canMove = false;
                        movement.canDash = false;
                        movement.canRotate = false;
                        Destroy(targetingObject);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartSpell(0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                StartSpell(1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                StartSpell(2);
        }        
    }

    void StartSpell (int spell)
    {
        isCasting = true;
        combat.canAttack = false;
        currentSpell = SpellDatabase.avaliableSpells[spell];
        targetingObject = Instantiate(currentSpell.targetingObject);
    }

    public void CancelCasting ()
    {
        Destroy(targetingObject);
        StoppedCasting();
    }

    void StoppedCasting ()
    {
        isCasting = false;
        combat.canAttack = true;
        movement.canMove = true;
        movement.canDash = true;
        movement.canRotate = true;
    }

}
