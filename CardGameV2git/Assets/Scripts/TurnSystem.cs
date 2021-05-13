using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public bool isYourTurn = true;
    public int yourTurn = 1;
    public int yourOpponentTurn = 0;
    public TextMeshProUGUI turnText;

    public int maxMana = 1;
    public int currentMana = 1;
    public TextMeshProUGUI manaText;


    void Start()
    {
        turnText.SetText("Your Turn");
    }


    void Update()
    {
        manaText.SetText(currentMana + "/" + maxMana);
    }

    public void EndYourTurn()
    {
        isYourTurn = false;
        yourOpponentTurn++;
        turnText.SetText("Opponent's Turn");
    }

    public void EndOpponentTurn()
    {
        isYourTurn = true;
        yourTurn++;
        maxMana++;
        currentMana = maxMana;
        turnText.SetText("Your Turn");
    }
}
