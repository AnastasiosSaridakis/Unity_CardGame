using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public int health;
    
    void Start()
    {
        healthText.text = health.ToString();
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damage)
    {
        health = health - damage;
        healthText.SetText(health.ToString());
    }


}