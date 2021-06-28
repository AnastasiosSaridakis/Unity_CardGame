using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public PlayerManager playerManager;
    public int currentMana, maxMana;

    public enum GameState { FlipCoin, Mulligan, PlayerTurn, EndGame };
    public GameState currentGameState;

    public enum BattlePhase { None, Selected, Targeted}
    public BattlePhase currentBattlePhase;

    private GameObject mulliganPanel;
    public Button mulliganButton;
    public Button startButton;
    public Button keepButton;
    private TMP_Text waitingPlayerText;
    private TMP_Text turnText;
    private TMP_Text manaText;
    public GameObject playerPortrait;
    public GameObject enemyPortrait;
    public PlayerDeck playerDeck;
    public GameObject hand;
    public GameObject tabletop;
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public List<Card> mulliganList;
    public GameObject minionSelected;
    public int maxCardsInHand;
    public int maxCardsOnBoard;
    public Button endTurnButton;
    public int playersMulliganed;
    public GameObject endGamePanel;
    public Material greenFlame;
    public Material redFlame;
    public Material blueFlame;
    [SerializeField]private GameObject[] playerList;
    [Header("TurnOptions")]
    public int turnNumber;
    [SerializeField] private List<Image> runeList;
    //[SerializeField] private bool isTimeFlagged;
    [SerializeField] private float TotalTurnTimer;
    [SerializeField] private TMP_Text timerTxt;
    [SerializeField] private Animator runesAnimator;
    [SerializeField] private Coroutine currentCoroutine;
    [SerializeField] public bool isFlagged;
    private bool DebugMode; //I AM USELESS PLZ REMOVE ME

    void Awake()
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
        turnText = UIGame.Instance.turnText;
        manaText = UIGame.Instance.manaCrystalText;
        //playerDeck = GameObject.FindWithTag("PlayerDeck").GetComponent<PlayerDeck>();
        hand = UIGame.Instance.hand;
        tabletop = UIGame.Instance.tableTop;
        mulliganPanel = UIGame.Instance.mulliganPanel;
        playerPortrait = UIGame.Instance.playerPortrait;
        enemyPortrait = UIGame.Instance.enemyPlayerPortrait;
        endTurnButton = UIGame.Instance.endTurnButton;
        mulliganButton = UIGame.Instance.mulliganButton;
        keepButton = UIGame.Instance.keepButton;
        //waitingPlayerText = UIGame.Instance.waitingPlayerText;

        currentBattlePhase = BattlePhase.None;
        minionSelected = null;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        endTurnButton.interactable = false;
        playerList = GameObject.FindGameObjectsWithTag("PlayerManager");
    }

    private void Update()
    {
        
    }

    public void ChangeTurn()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        foreach (GameObject pm in playerList)
        {
            if (pm.GetComponent<PlayerManager>() != playerManager) 
            {
                Debug.Log("Im inside foreach");
                playerManager.CmdChangeTurn(pm.GetComponent<PlayerManager>().isTimeFlagged);
            }
        }
    }

    public void ReloadText()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        if (playerManager.isMyTurn == true)
        {
            turnText.SetText("My turn");
            manaText.SetText(currentMana.ToString() + "/" + maxMana.ToString());
        }
        else
        {
            turnText.SetText("Enemy turn");
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        if (gameState == GameState.FlipCoin)
        {
            
            currentGameState = GameState.FlipCoin;
        }
        else if (gameState == GameState.Mulligan)
        {
            Debug.Log("EGine mulligan");
            currentGameState = GameState.Mulligan;
            for (int i = 0; i < 5; i++)
            {
                playerDeck.Draw();
            }
        }
        else if (gameState == GameState.PlayerTurn)
        {
            /*Debug.Log("2");
            Debug.Log("gameState == GameState.PlayerTurn");*/
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            playerManager = networkIdentity.GetComponent<PlayerManager>();

            //if (maxMana < 10 && playerManager.isMyTurn)
            //    maxMana++;
            //currentMana = maxMana;
            //ReloadText();

            if (playerManager.isMyTurn)//here do things when my turn or enemies turn starts 
            {
                if (turnNumber == 0)
                {
                    currentMana++;
                    maxMana++;
                    ReloadText();
                }
                endTurnButton.interactable = true;
               // Debug.Log("playerManager.isMyTurn");
                for (int i = tabletop.transform.childCount - 1; i >= 0; --i)//Set all played minions canAttack to true
                {
                    Transform child = tabletop.transform.GetChild(i);
                    if (child.gameObject.tag == "Card" && !child.gameObject.GetComponent<Draggable>().canAttack)
                    {
                        child.gameObject.GetComponent<Draggable>().canAttack = true;
                        //Color c = child.gameObject.GetComponent<Image>().color = Color.green;
                        //c.a = 1;
                        //child.gameObject.GetComponent<Image>().color = c;
                        child.gameObject.GetComponent<Image>().material = greenFlame;
                    }
                }
                playerDeck.Draw();
            }
            else
            {
                if (turnNumber == 0)
                {
                    playerDeck.Draw();
                    Debug.Log($"Drawing an extra card because im not first!...");
                }
                endTurnButton.interactable = false;
                //Debug.Log("playerManager.!isMyTurn");
                for (int i = tabletop.transform.childCount - 1; i >= 0; --i)//Set all played minions canAttack to false
                {
                    Transform child = tabletop.transform.GetChild(i);
                    if (child.gameObject.tag == "Card")
                    {
                        child.gameObject.GetComponent<Draggable>().canAttack = false;
                        /*Color c = child.gameObject.GetComponent<Image>().color;
                        c.a = 0;
                        child.gameObject.GetComponent<Image>().color = c;*/
                        child.gameObject.GetComponent<Image>().material = null;
                    }
                }
            }

            //hasChangedTurn = true;
            TurnTimer();
            currentGameState = GameState.PlayerTurn;
            
        }
        else if (gameState == GameState.EndGame)
        {
            Debug.Log("GAME ENDED!");
            currentGameState = GameState.EndGame;
        }
        else
        {
            Debug.Log("State not found?");
        }
    }

    public void ChangeGameString(string state)
    {
        if (state == "Mulligan")
        {
            
            //startButton.gameObject.SetActive(false);
            
            mulliganButton.gameObject.SetActive(true);
            
            keepButton.gameObject.SetActive(true);
            ChangeGameState(GameManager.GameState.Mulligan);
        }
        else
        {
            Debug.Log("changegamestring not found");
        }
    }

    public void KeepCards()
    {
        Debug.Log("1");
        for (int i = mulliganPanel.transform.childCount - 1; i >= 0; --i)
        {
            Transform child = mulliganPanel.transform.GetChild(i);
            if (child.gameObject.tag == "Card")
            {
                child.SetParent(hand.transform, false);
                child.transform.localScale = new Vector3(1, 1, 1);
                child.GetComponent<CanvasGroup>().blocksRaycasts = true;
                Debug.Log("Cards are: " + child.name);
            }
        }
        mulliganPanel.SetActive(false);
        currentGameState = GameState.PlayerTurn;
        ChangeGameState(GameManager.GameState.PlayerTurn);
    }

    public void MulliganCards()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        playerManager.EmptyHandList();
        
        for (int i = mulliganPanel.transform.childCount - 1; i >= 0; --i)
        {
            Transform child = mulliganPanel.transform.GetChild(i);
            if (child.gameObject.tag == "Card")
            {
                Card card = child.gameObject.GetComponent<CardDisplay>().card;
                playerDeck.deck.Add(card);
                Destroy(child.gameObject);
            }
        }
        playerDeck.Shuffle();
        playerManager.CmdDestroyFirstDraw();
        mulliganButton.interactable = false;
        ChangeGameState(GameManager.GameState.Mulligan);
    }

    public void ApplyDamage(GameObject attacker, GameObject target)
    {
        if(attacker.tag == "Card")//currently only cards can attack
        {
            if(target.tag == "Card")//if both are cards, both needs to be damaged
            {
                Debug.Log("GAMEMANAGER APPLY DAMAGE?");
                target.GetComponent<CardDisplay>().card.health -= attacker.GetComponent<CardDisplay>().card.attack;
                attacker.GetComponent<CardDisplay>().card.health -= target.GetComponent<CardDisplay>().card.attack;

            }
            else if (target.tag=="Player")//if attacker is card BUT target is player
            {

            }
        }
    }

    public void AcceptedMulligan()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        playerManager.SetAcceptedMulligan();
    }
    public void StartGame()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        //waitingPlayerText.gameObject.SetActive(true);
       // startButton.interactable = false;

        playerManager.SetPlayerReady();

    }

    public void WonGame()
    {
        if(currentCoroutine!=null)
            StopCoroutine(currentCoroutine);
        runesAnimator.enabled = false;
        
        endGamePanel.SetActive(true);
        UIGame.Instance.endGameWinnerPoster.SetActive(true); 
    }
    public void LostGame()
    {
        if(currentCoroutine!=null)
            StopCoroutine(currentCoroutine);
        runesAnimator.enabled = false;
        UIGame.Instance.pauseMenu.SetActive(false);
        endGamePanel.SetActive(true);
        UIGame.Instance.endGameLoserPoster.SetActive(true);
    } 
    public void ForfeitGame()
    {
        playerManager.CmdForfeitGame();
    }
    public void RestartGame()
    {
        UILobby.Instance.DisconnectLobby();
        NetworkManager.singleton.StopClient();
    }

    public void TurnTimer()
    {
        /*for(int i =0; i<runeList.Count;i++)
        {
            runeList[i].color = new Color32(31, 231, 255, 255);
        }*/

        if(currentCoroutine!=null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
        
        int i = 1;
        float counter = 0;
        float maxTurnTimer = 0;
        maxTurnTimer = !isFlagged ? TotalTurnTimer : 18f;
        if (runesAnimator.enabled == false)
        {
            runesAnimator.enabled = true;
        }

        if (!isFlagged)
        {
            runesAnimator.Play("RuneBar",-1,0f);
        }
        else
        {
            runesAnimator.Play("RuneBar",-1,0.7f);
        }
        
        Debug.Log($"Im inside CountDown | Setting animator to true");
        while (counter < maxTurnTimer)
        {
            counter += Time.deltaTime;
            timerTxt.text = counter.ToString("N1");
            
            if (counter >= 6*i)
            {
                Debug.Log("We have waited for: " + counter + " seconds and i value is: "+i);
                i++;
            }
            yield return null;
        }
        
        Debug.Log($"HERE CHANGE TURN AUTOMATICALLY!!!");
        if (playerManager.isMyTurn)
        {
            playerManager.CmdTimeFlagged(true,playerManager);
            ChangeTurn();
        }
    }

    public void ManualButtonPress()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
        
        playerManager.CmdTimeFlagged(false,playerManager);
        Debug.Log($"Button: Setting {playerManager.netIdentity} isTimeFlagged to: {playerManager.isTimeFlagged}");
    }
    
}
