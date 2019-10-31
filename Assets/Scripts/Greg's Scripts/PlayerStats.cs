using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    public float speed, jumpHeight, maxHealth, health, armour, maxMana, mana, additionallManaGain, damage, timeBetweenSwings;
    PlayerCombat2 combat;
    PlayerControl movement;
    PlayerSpells spells;
    Animator anim;
    public Slider hpDisplay, manaDisplay;
    public Text coinCounter;
    public int money;
    public Material originalMaterial, damageMaterial;
    bool isAlive = true;
    public static PlayerStats instance;
    public Image gameOverImage;

    public SphereCollider coinPickupCol;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        combat = GetComponent<PlayerCombat2>();
        movement = GetComponent<PlayerControl>();
        spells = GetComponent<PlayerSpells>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        instance = this;
        AddMana(0);
        AddMoney(0);
    }

    void TakeDamage (float damage)
    {
        if (!isAlive)
            return;


        Debug.Log(1);
        health -= (damage * (1 - armour / 100));
        CameraFunctions.AddShake(0.1f * damage);
        transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = damageMaterial;
        Invoke("ResetPlayerColor", 0.2f);
        hpDisplay.value = health / maxHealth;
        if (health <= 0 && isAlive)
            Die();
    }

    void ResetPlayerColor ()
    {
        transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = originalMaterial;
    }


    void Die ()
    {
        isAlive = false;
        anim.SetTrigger("Die");
        combat.canAttack = false;
        movement.canDash = false;
        movement.canJump = false;
        movement.canMove = false;
        Destroy(combat);
        Destroy(movement);
        spells.CancelCasting();
        spells.isCasting = true;
        movement.canRotate = false;
        StartCoroutine(FadeGameOverScreen());
    }

    IEnumerator FadeGameOverScreen ()
    {
        yield return new WaitForSeconds(2);
        while (gameOverImage.color.a < 1)
        {
            gameOverImage.color = new Color(gameOverImage.color.r, gameOverImage.color.g, gameOverImage.color.b, gameOverImage.color.a + Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public static void AddMana (float newMana)
    {
        instance.mana += newMana;
        if (instance.mana > instance.maxMana) instance.mana = instance.maxMana;
        instance.manaDisplay.value = instance.mana / instance.maxMana;
    }

    public void AddMoney (int newMoney)
    {
        money += newMoney;
        coinCounter.text = "Coins: " + money;
    }

    public void ExtendCoinPickupCollider ()
    {
        coinPickupCol.radius += 1f;
    }

}
