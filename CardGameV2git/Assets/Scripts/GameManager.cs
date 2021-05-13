using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
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
    private Button mulliganButton;
    private Button startButton;
    private Button keepButton;
    private TextMeshProUGUI waitingPlayerText;
    private TextMeshProUGUI turnText;
    private TextMeshProUGUI manaText;
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
    public int playersReady;
    public GameObject endGamePanel;
    public Material greenFlame;
    public Material redFlame;
    public Material blueFlame;
    public bool DebugMode;

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
        turnText = GameObject.FindWithTag("TurnText").GetComponent<TextMeshProUGUI>();
        manaText = GameObject.FindWithTag("ManaCrystal").GetComponentInChildren<TextMeshProUGUI>();
        playerDeck = GameObject.FindWithTag("PlayerDeck").GetComponent<PlayerDeck>();
        hand = GameObject.FindWithTag("Hand");
        tabletop = GameObject.FindWithTag("Tabletop");
        mulliganPanel = GameObject.FindWithTag("MulliganPanel");
        playerPortrait = GameObject.FindWithTag("Player");
        enemyPortrait = GameObject.FindWithTag("EnemyPlayer");

        currentBattlePhase = BattlePhase.None;
        minionSelected = null;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        
        
            endTurnButton.interactable = false;
            startButton = mulliganPanel.GetComponent<MulliganPanel>().GetStartGameButton();
            mulliganButton = mulliganPanel.GetComponent<MulliganPanel>().GetMulliganButton();
            keepButton = mulliganPanel.GetComponent<MulliganPanel>().GetKeepButton();
            waitingPlayerText = mulliganPanel.GetComponent<MulliganPanel>().GetPlayerText();
        
    }
    
    public void ChangeTurn()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        playerManager.CmdChangeTurn();
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
            turnText.SetText("Opponent's turn");
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
            Debug.Log("2");
            Debug.Log("gameState == GameState.PlayerTurn");
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            playerManager = networkIdentity.GetComponent<PlayerManager>();

            //if (maxMana < 10 && playerManager.isMyTurn)
            //    maxMana++;
            //currentMana = maxMana;
            //ReloadText();

            if (playerManager.isMyTurn)//here do things when my turn or enemies turn starts 
            {
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
                endTurnButton.interactable = false;
                Debug.Log("playerManager.!isMyTurn");
                for (int i = tabletop.transform.childCount - 1; i >= 0; --i)//Set all played minions canAttack to false
                {
                    Transform child = tabletop.transform.GetChild(i);
                    if (child.gameObject.tag == "Card")
                    {
                        child.gameObject.GetComponent<Draggable>().canAttack = false;
                        Color c = child.gameObject.GetComponent<Image>().color;
                        c.a = 0;
                        child.gameObject.GetComponent<Image>().color = c;
                    }
                }
            }
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
            
            startButton.gameObject.SetActive(false);
            
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

    public void StartGame()
    {
        if (DebugMode)
        {
            playerManager.CmdPlayerReady(playersReady);
        }
        else
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            playerManager = networkIdentity.GetComponent<PlayerManager>();

            waitingPlayerText.gameObject.SetActive(true);
            startButton.interactable = false;

            playerManager.CmdPlayerReady(playersReady);

        }
    }

    public void WonGame()
    {
        endGamePanel.SetActive(true);
        endGamePanel.GetComponentInChildren<TextMeshProUGUI>().SetText("Congratulations, You Won!");
    }
    public void LostGame()
    {
        endGamePanel.SetActive(true);
        endGamePanel.GetComponentInChildren<TextMeshProUGUI>().SetText("Unfortunately, You Lost!");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
}
