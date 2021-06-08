using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour
{
    [Header("Mulligan Panel")] 
    public GameObject mulliganPanel;
    public Button keepButton;
    public Button mulliganButton;
    public TMP_Text waitingPlayerText;

    [Header("Playing Phase")] 
    public Button endTurnButton;

    public GameObject hand;
    public GameObject tableTop;
    public GameObject enemyHand;
    public GameObject enemyTableTop;
    
    public TMP_Text manaCrystalText;
    public GameObject playerPortrait;
    public GameObject enemyPlayerPortrait;
    
    public TMP_Text turnText;

    [Header("EndGame Phase")] 
    public GameObject endGamePanel;
    public TMP_Text endGameMessage;
    public Button leaveGameButton;
    
    private static UIGame _instance;
    
    public static UIGame Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    _instance = this;
                }
    }

    private void Start()
    {
        hand = GameObject.FindGameObjectWithTag("Hand");
        tableTop = GameObject.FindGameObjectWithTag("Tabletop");
        //enemyHand= GameObject.FindGameObjectWithTag("Hand")
        enemyTableTop = GameObject.FindGameObjectWithTag("EnemyTabletop");
    }
}
