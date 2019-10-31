using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperItem : MonoBehaviour
{

    public Material highlightMat;
    Material originalMat;
    Renderer rend;
    PlayerStats stats;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMat = rend.material;
        stats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseOver()
    {
        if (transform.parent.parent.GetComponent<Shopkeeper>() == null)
            return;
        rend.material = highlightMat;
        if (Input.GetMouseButtonDown(0))
        {
            switch (transform.name)
            {
                case "+1 health(Clone)":
                    if (stats.money > 50)
                    {
                        stats.AddMoney(-50);
                        stats.maxHealth += 5;
                        Destroy(gameObject);
                        Destroy(transform.parent.parent.GetComponent<Shopkeeper>());
                        Destroy(transform.parent.parent.gameObject, 2);
                    }
                    break;
                case "+2 armour(Clone)":
                    if (stats.money > 50)
                    {
                        stats.armour += 5;
                        stats.AddMoney(-50);
                        Destroy(gameObject);
                        Destroy(transform.parent.parent.GetComponent<Shopkeeper>());
                        Destroy(transform.parent.parent.gameObject, 2);
                    }
                    break;
                case "+10% mana gain(Clone)":
                    if (stats.money > 50)
                    {
                        stats.AddMoney(-50);
                        stats.additionallManaGain += 10;
                        Destroy(gameObject);
                        Destroy(transform.parent.parent.GetComponent<Shopkeeper>());
                        Destroy(transform.parent.parent.gameObject, 2);
                    }
                    break;
                case "CoinMagnet(Clone)":
                    if (stats.money > 30)
                    {
                        stats.AddMoney(-30);
                        stats.ExtendCoinPickupCollider();
                        Destroy(gameObject);
                        Destroy(transform.parent.parent.GetComponent<Shopkeeper>());
                        Destroy(transform.parent.parent.gameObject, 2);
                    }
                    break;
                case "SpeedUp(Clone)":
                    if (stats.money > 85)
                    {
                        stats.speed += 0.5f;
                        stats.AddMoney(-85);
                        Destroy(gameObject);
                        Destroy(transform.parent.parent.GetComponent<Shopkeeper>());
                        Destroy(transform.parent.parent.gameObject, 2);
                    }
                    break;
            }
        }
    }

    private void OnMouseExit()
    {
        rend.material = originalMat;
    }

}
