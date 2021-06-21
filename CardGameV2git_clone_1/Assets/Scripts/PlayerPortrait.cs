using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TMP_Text nameText;
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
        health -= damage;
        healthText.SetText(health.ToString());
    }

    public void SetUsername(string username)
    {
        nameText.text = username;
    }


}