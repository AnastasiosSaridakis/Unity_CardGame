using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Additive;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerManager : NetworkBehaviour
{
    public GameObject hand;
    public GameObject tabletop;
    public GameObject enemyHand;
    public GameObject enemytabletop;
    public GameObject mulliganPanel;
    public GameObject cardPrefab;
    Card tempCard;
    private GameObject zoomCard;
    public GameObject placeHolder;

    [SyncVar]
    public int playersReady;

    [Header("PlayerConnection")]
    public static PlayerManager localPlayer;
    public static TurnManager myTurnManager;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;
    [SyncVar] public Match currentMatch;
    [SyncVar] public string username;
    
    [Header("In-match")] 
    [SyncVar] public bool isMyTurn = false;
    [SyncVar] public int diceRoll;
    [SyncVar] public int turn;
    [SyncVar] public bool isTimeFlagged = false;
    [SerializeField] private List<GameObject> handList;
    [SerializeField] private List<GameObject> tabletopList;
    
    
    private NetworkMatchChecker networkMatchChecker;

    [SerializeField] private GameObject playerLobbyUI;

    private void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
            localPlayer.username = DeckManager.Instance.username;
            CmdSendUname(localPlayer,localPlayer.username);
            Debug.Log(("IT IS LOCAL PLAYER"));
        }
        else
        { 
           playerLobbyUI = UILobby.Instance.SpawnPlayerUIPrefab(this);
           Debug.Log(("NOT LOCAL PLAYER"));
        }
    }

    [Command]
    public void CmdSendUname(PlayerManager player , string uname)
    {
        player.username = uname;
    }
    public override void OnStopClient()
    {
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        ServerDisconnect();
    }

    private void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 4)
        {
            Debug.Log($"<color=red> 2 game scene finised loading</color>");
           // CmdSpawnObjects();
            GameManager.Instance.StartGame();
            if (isClientOnly)
            {
                hand = UIGame.Instance.hand;
                enemyHand = UIGame.Instance.enemyHand;
                tabletop = UIGame.Instance.tableTop;
                enemytabletop = UIGame.Instance.enemyTableTop;
                mulliganPanel = UIGame.Instance.mulliganPanel;
                if (hasAuthority)
                {
                    UIGame.Instance.playerPortrait.GetComponent<PlayerPortrait>().SetUsername(username);
                }
                else
                {
                    UIGame.Instance.enemyPlayerPortrait.GetComponent<PlayerPortrait>().SetUsername(username);
                }
                //isMyTurn = true;
                //CmdChangeTurn();
            }
        }
    }
    public void SetPlayerReady()
    {
        CmdPlayerReady();
    }
    
    public void SetAcceptedMulligan()
    {
        CmdAcceptedMulligan(GameManager.Instance.playersMulliganed);
    }

    public void DealCards(Card card)
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.Mulligan)
        {
            CmdMulliganCards(card.id);
        }
        else if (GameManager.Instance.currentGameState == GameManager.GameState.PlayerTurn)
        {
            CmdDealCards(card.id);
        }
        else
        {
            Debug.Log("CmdDealCards Unknown GameState");
        }
    }



    public void EmptyHandList()
    {
        handList.Clear();
    }

    public void PlayCard( GameObject cardObject, string placeholderParent, int index)
    {
        GameManager.Instance.ReloadText();
        CardInfo info = new CardInfo(cardObject.GetComponent<CardDisplay>().card.id);
        CmdPlayCard(info, placeholderParent, index);
    }

    public void PreviewCard(int id)
    {
        //Debug.Log("PreviewCard cardToPreview is: " + cardToPreview);
        CmdSpawnPreview(id);
    }

    [Server]
    public override void OnStartServer()
    {

    }

    #region Commands

    [Command]
    public void CmdPlayerReady()
    {
        playersReady++;
        RpcPlayerReady(playersReady);
    }

    [ClientRpc]
    public void RpcPlayerReady(int currentPlayersReady)
    {
        if (currentPlayersReady == 2 && isLocalPlayer)
        {
            Debug.Log($"Players are {currentPlayersReady} and starting mulligan");
            GameManager.Instance.ChangeGameString("Mulligan");
        }
        else
        {
            Debug.Log($"Players are {currentPlayersReady} and waiting OR not local");
        }
    }    
    
    [Command]
    public void CmdAcceptedMulligan(int acceptedM)
    {
        acceptedM++;
        RpcAcceptedMulligan(acceptedM);
    }

    [ClientRpc]
    public void RpcAcceptedMulligan(int _acceptedMulligan)
    {
        GameManager.Instance.playersMulliganed = _acceptedMulligan;
        if (_acceptedMulligan == 2)
        {
            //Start Game
            Debug.Log($"Players are {_acceptedMulligan} and starting Game");
            GameManager.Instance.KeepCards();
        }
        else
        {
            Debug.Log($"Players are {_acceptedMulligan} and waiting");
            //UIGame.Instance.waitingPlayerText.enabled = true;
        }
        
    }

    [Command]
    public void CmdForfeitGame()
    {
        RpcForfeitGame();
    }

    [ClientRpc]
    public void RpcForfeitGame()
    {
        if (hasAuthority)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.EndGame);
            GameManager.Instance.LostGame();
        }
        else
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.EndGame);
            GameManager.Instance.WonGame();
        }
    }
    
    [Command]
    public void CmdMulliganCards(int id)
    {
        /*foreach (Card card in CardDatabase.Instance.cardList)
        {
            if (card.id == id)
            {
                GameObject go;
                go = Instantiate(cardPrefab);
                NetworkServer.Spawn(go, connectionToClient);
                go.GetComponent<CardDisplay>().card = card;

                //Debug.Log("I've drawn a " + card);
                RpcShowCard(go, id, "Mulligan");
            }
        }*/
        RpcShowCard(id, "Mulligan");
    }

    [Command]
    public void CmdDealCards(int id)
    {
        Debug.Log("Inside CMD DEAL CARDS");
        /*foreach (Card card in CardDatabase.Instance.cardList)
        {
            if (card.id == id)
            {
                GameObject go;
                go = Instantiate(cardPrefab);
                NetworkServer.Spawn(go, connectionToClient);
                go.GetComponent<CardDisplay>().card = card;
                Debug.Log("Im in Dealt");
                //Debug.Log("I've drawn a " + card);
                RpcShowCard(go, id, "Dealt");
            }
        }*/
        RpcShowCard(id, "Dealt");
    }

    [Command]
    public void CmdPlayCard(CardInfo info,string placeholderParent,int index)
    {
        RpcPlayCard(info, placeholderParent, index);
    }

    [Command]
    public void CmdSpawnPreview(int id)
    {
        Debug.Log("Before authority check, also id is: " + id);
        if (hasAuthority)
        {
            Debug.Log("CmdSpawnPreview id is: " + id);
            placeHolder = Instantiate(cardPrefab);
            NetworkServer.Spawn(placeHolder, connectionToClient);

            foreach (Card card in CardDatabase.Instance.cardList)
            {
                if (card.id == id)
                {
                    placeHolder.GetComponent<CardDisplay>().card = card;
                }
            }
        }
        else
        {
            Debug.Log("den exw authority , also id is: " + id);
        }
    }

    [Command]
    public void CmdChangeTurn(bool flagged)
    {
        turn++;
        RpcChangeTurn(turn,flagged);
    }

    [Command]
    public void CmdTimeFlagged(bool flagged,PlayerManager player)
    {
        player.isTimeFlagged = flagged;
    }

    [Command]
    public void CmdApplyDamage(CardInfo attackerInfo, CardInfo targetInfo, int attackerIndex, int targetIndex, string targetType)
    {
        RpcApplyDamage(attackerInfo, targetInfo,attackerIndex,targetIndex,targetType);
    }

    [Command]
    public void CmdDestroyFirstDraw()
    {
        RpcDestroyFirstDraw();
    }
    #endregion

    #region Client

    public void TargetZoomCard(NetworkConnection target, GameObject card)
    {
        if (hasAuthority)
        {
            //Debug.Log("TargetZoomCard " + this.netId + hasAuthority);
            //card.GetComponent<Image>().color = Color.red;
            card.GetComponent<CanvasGroup>().blocksRaycasts = false;
            card.GetComponent<Draggable>().enabled = false;
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(card.GetComponent<RectTransform>().sizeDelta.x, card.GetComponent<RectTransform>().sizeDelta.y);
            rect.localScale = new Vector3(2, 2, 2);
        }
    }

    [ClientRpc]
    void RpcPlayCard(CardInfo info, string placeholderParentString, int index)
    {
        Transform placeholderParent;
        if (placeholderParentString.Equals("tabletop"))
        {
            placeholderParent = UIGame.Instance.tableTop.transform;
        }
        else if (placeholderParentString.Equals("hand"))
        {
            placeholderParent = UIGame.Instance.hand.transform;
        }
        else
        {
            placeholderParent=null;
            Debug.Log("placeholderParent string is wrong");
        }
        if (hasAuthority)
        { 
            
            //card.transform.SetParent(placeholderParent);
           // card.transform.SetSiblingIndex(index);
        }
        else
        {
            if(placeholderParent == UIGame.Instance.hand.transform)
            {
                Debug.Log("Eimai sto RPCplaycard, NO authority, enemyhand");
                /*card.transform.SetParent(enemyHand.transform, false);
                card.transform.SetSiblingIndex(index);*/
            }
            else if(placeholderParent == UIGame.Instance.tableTop.transform)
            {
                Debug.Log("Eimai sto RPCplaycard, NO authority, enemytabletop");
                GameObject card = Instantiate(cardPrefab, enemytabletop.transform, false);
                card.GetComponent<CardDisplay>().InitializeStats(info);
                card.GetComponent<Draggable>().isDraggable = false;
                card.GetComponent<CanvasGroup>().blocksRaycasts = true;
               // card.transform.Rotate(0f, 0f, 180f);
                card.transform.SetSiblingIndex(index);
                //card.GetComponent<CardDisplay>().FlipCard();
                if(enemyHand.transform.GetChild(0).gameObject != null)
                    Destroy(enemyHand.transform.GetChild(0).gameObject);
            }
        }
    }

    [ClientRpc]
    void RpcShowCard(int id, string Type)
    {
        if (Type == "Dealt")
        {
            if (hasAuthority)
            {
                GameObject go;
                go = Instantiate(cardPrefab);
                handList.Add(cardPrefab);
                foreach (Card card in CardDatabase.Instance.cardList)
                {
                    if (card.id == id)
                    {
                        go.GetComponent<CardDisplay>().card = card;
                    }
                }
                if (hand.transform.childCount < GameManager.Instance.maxCardsInHand)
                {
                    go.transform.SetParent(hand.transform, false);
                }
                else
                {//Here i can burn the card, because my hand is full
                    Debug.Log("HAND IS FULL!");
                    Destroy(go);
                } 

            }
            else
            {
                GameObject go;
                go = Instantiate(cardPrefab);
                
                foreach (Card card in CardDatabase.Instance.cardList)
                {
                    if (card.id == id)
                    {
                        go.GetComponent<CardDisplay>().card = card;

                    }
                }
                if (enemyHand.transform.childCount < GameManager.Instance.maxCardsInHand)
                {
                    go.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    go.transform.SetParent(enemyHand.transform, false);
                    go.GetComponent<CardDisplay>().FlipCard();
                }
                else
                {//Here i can burn the card, because my opponents hand is full
                    Debug.Log("ENEMY HAND IS FULL!");
                    Destroy(go);
                }
            }
        }
        else if (Type == "Mulligan")
        {
            if (hasAuthority)
            {
                Debug.Log("RPCshowcard Mulligan HAS authority");
                GameObject go;
                go = Instantiate(cardPrefab);
                handList.Add(go);
                foreach (Card card in CardDatabase.Instance.cardList)
                {
                    if (card.id == id)
                    {
                        go.GetComponent<CardDisplay>().card = card; //does it really do something here?
                    }
                }
                go.transform.SetParent(mulliganPanel.transform, true);
                go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                go.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else
            {
                GameObject go;
                go = Instantiate(cardPrefab);
                Debug.Log("RPCshowcard Mulligan NO authority Destroy");
                //Destroy(go);
                foreach (Card card in CardDatabase.Instance.cardList)
                {
                    if (card.id == id)
                    {
                        go.GetComponent<CardDisplay>().card = card; //does it really do something here?
                    }
                }
                go.transform.SetParent(enemyHand.transform, false);
                go.transform.SetSiblingIndex(0);
                go.GetComponent<CanvasGroup>().blocksRaycasts = false;
                go.GetComponent<CardDisplay>().FlipCard();
            }
        }
    }

    [ClientRpc]
    void RpcChangeTurn(int turn,bool flagged)
    {
        GameManager.Instance.turnNumber++;
        PlayerManager pm = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        
        pm.isMyTurn = !(pm.isMyTurn);
        
        GameManager.Instance.isFlagged = flagged;
        
        GameManager.Instance.ChangeGameState(GameManager.GameState.PlayerTurn);
        if (GameManager.Instance.maxMana < 10 && pm.isMyTurn)
            GameManager.Instance.maxMana++;
       // Debug.Log($"max mana is {GameManager.Instance.maxMana}");
        GameManager.Instance.currentBattlePhase = GameManager.BattlePhase.None;
        
        GameManager.Instance.currentMana = GameManager.Instance.maxMana;
        GameManager.Instance.ReloadText();
    }

    [ClientRpc]
    public void RpcApplyDamage(CardInfo attackerInfo, CardInfo targetInfo, int firstIndex, int secondIndex, string targetType)
    {
        GameObject attackerGO, targetGO = null;
        if (hasAuthority)
        {Debug.Log("FIrst if yes authority");
            attackerGO = tabletop.transform.GetChild(firstIndex).gameObject;
            if(attackerGO == null)
                return;
            attackerGO.GetComponent<MyFeedbacks>().AttackFeedback();
            
            if (targetType.Equals("Card"))
            {
                targetGO = enemytabletop.transform.GetChild(secondIndex).gameObject;
                if(targetGO == null)
                    return;
                targetGO.GetComponent<MyFeedbacks>().GetAttackedFeedback();
            }
            else
            {
                targetGO = GameManager.Instance.enemyPortrait;
            }
        }
        else
        {
            Debug.Log("FIrst if no authority");
             attackerGO = enemytabletop.transform.GetChild(firstIndex).gameObject;
             if(attackerGO == null)
                 return;
             attackerGO.GetComponent<MyFeedbacks>().GetAttackedFeedback();
             if (targetType.Equals("Card"))
             {
                 targetGO = tabletop.transform.GetChild(secondIndex).gameObject;
                 if(targetGO == null)
                     return;
                 targetGO.GetComponent<MyFeedbacks>().AttackFeedback();
             }
             else
             {
                 targetGO = GameManager.Instance.playerPortrait;
             }
        }

        if (attackerGO == null || targetGO == null)
        {
            Debug.Log($"attacker or target is null, returning...");
            return;
        }
        
        //if (!attackerGO.CompareTag("Card")) return;
        //attackerGO.GetComponent<MyFeedbacks>().AttackFeedback();
        if (targetGO.CompareTag("Card") )//if both are cards, both needs to be damaged
        {
            //    Debug.Log("attacker damage is: " + attacker.GetComponent<CardDisplay>().GetAttack());
            //    Debug.Log("target damage is: " + target.GetComponent<CardDisplay>().GetAttack());
            //    Debug.Log("attacker health is is: " + attacker.GetComponent<CardDisplay>().GetHealth());
            //    Debug.Log("target health is is: " + target.GetComponent<CardDisplay>().GetHealth());
            Debug.Log($"<color=yellow>{targetGO.GetComponent<CardDisplay>().nameText.text} remaining health is {targetInfo.health - attackerInfo.attack}</color>");    
            targetGO.GetComponent<CardDisplay>().setHealth(targetInfo.health - attackerInfo.attack);
            targetInfo.health = targetGO.GetComponent<CardDisplay>().GetHealth();
            //targetGO.GetComponent<MyFeedbacks>().GetAttackedFeedback();
            Debug.Log($"<color=yellow>{attackerGO.GetComponent<CardDisplay>().nameText.text} remaining health is {attackerInfo.health - targetInfo.attack}</color>");    
            attackerGO.GetComponent<CardDisplay>().setHealth(attackerInfo.health - targetInfo.attack);
            attackerInfo.health= attackerGO.GetComponent<CardDisplay>().GetHealth();
                

                
            if (attackerGO.GetComponent<CardDisplay>().GetHealth() <= 0)
            {
                attackerGO.GetComponent<CanvasGroup>().blocksRaycasts = false;
                attackerGO.GetComponent<Dissolve>().StartDissolving(); //attacker life is 0
                //Destroy(attacker);

            } 
            else
            {
                if(attackerGO.GetComponent<CardZoom>().zoomCard!=null)
                    attackerGO.GetComponent<CardZoom>().zoomCard.GetComponent<CardDisplay>().setHealth(attackerInfo.health);
                
            }
            if (targetGO.GetComponent<CardDisplay>().GetHealth() <= 0)
            {
                targetGO.GetComponent<CanvasGroup>().blocksRaycasts = false;
                targetGO.GetComponent<Image>().material = null;
                targetGO.GetComponent<Dissolve>().StartDissolving(); //target life is 0
                //Destroy(target);
                //Destroy(zoomCard);
            }
            else
            {
                if(targetGO.GetComponent<CardZoom>().zoomCard!=null)
                    targetGO.GetComponent<CardZoom>().zoomCard.GetComponent<CardDisplay>().setHealth(targetInfo.health);
            }
        }
        else if (targetGO.CompareTag("EnemyPlayer") || targetGO.CompareTag("Player"))//if attacker is card BUT target is player
        {
            if (hasAuthority)//I am doing the attack so my opponent takes damage
            {
                GameManager.Instance.enemyPortrait.GetComponent<PlayerPortrait>().TakeDamage(attackerInfo.attack);
                //target.GetComponent<PlayerPortrait>().SetHealth(target.GetComponent<PlayerPortrait>().GetHealth() - attacker.GetComponent<CardDisplay>().GetAttack());
                if (targetGO.GetComponent<PlayerPortrait>().GetHealth() > 0) return;
                Debug.Log("I WON");
                GameManager.Instance.ChangeGameState(GameManager.GameState.EndGame);
                GameManager.Instance.WonGame();
            }
            else//I am receiving the attack so I have to take damage
            {
                GameManager.Instance.playerPortrait.GetComponent<PlayerPortrait>().TakeDamage(attackerInfo.attack);
                attackerGO.GetComponent<MyFeedbacks>().GetAttackedFeedback();
                //GameManager.Instance.playerPortrait.GetComponent<PlayerPortrait>().SetHealth(GameManager.Instance.playerPortrait.GetComponent<PlayerPortrait>().GetHealth()- attacker.GetComponent<CardDisplay>().GetAttack());

                if (targetGO.GetComponent<PlayerPortrait>().GetHealth() > 0) return;
                Debug.Log("I Lost :(");
                GameManager.Instance.ChangeGameState(GameManager.GameState.EndGame);
                GameManager.Instance.LostGame();
            }
        }
    }

    [ClientRpc]
    public void RpcDestroyFirstDraw()
    {//if i muligan, for the other player destroy the cards in his enemy hand (which is me)
        if (!hasAuthority) 
        {
            for (int i = enemyHand.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = enemyHand.transform.GetChild(i);
                if (child.gameObject.tag == "Card")
                {
                    Destroy(child.gameObject);
                }
            }

        }
    }

    #endregion
    
    #region Host
    /*Host game commands and RPC's*/
    public void HostGame(bool publicMatch)
    {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID,publicMatch);
    }

    [Command]
    void CmdHostGame(string _matchID, bool publicMatch)
    {
        matchID = _matchID;
        if(MatchMaker.instance.HostGame(_matchID, gameObject,publicMatch , out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully | ID: {_matchID}</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetHostGame(true,_matchID, playerIndex);
        }
        else
        {
            Debug.Log("<color=red>Game host failed | ID already exists!</color>");
            TargetHostGame(false,_matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success,string _matchID, int _playerIndex)
    {
        this.playerIndex = _playerIndex;
        matchID = _matchID;
        //Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.Instance.HostSuccess(success, _matchID);
    }
    #endregion
    
    #region Join 
    /*Join game commands and RPC's*/
    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        matchID = _matchID;
        if(MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
        {
            //Debug.Log("<color=green>Game joined successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true,_matchID, playerIndex);
        }
        else
        {
            //Debug.Log("<color=red>Game join failed</color>");
            TargetJoinGame(false,_matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success,string _matchID,int _playerIndex)
    {
        this.playerIndex = _playerIndex;
        matchID = _matchID;
        //Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.Instance.JoinSuccess(success,_matchID);
    }
    #endregion

    #region SearhMatch

    public void SearchGame()
    {
        CmdSearchGame();
    }

    [Command]
    void CmdSearchGame()
    {
        if(MatchMaker.instance.SearchGame(gameObject, out playerIndex,out matchID))
        {
            //Debug.Log("<color=green>Game found</color>");
            networkMatchChecker.matchId = matchID.ToGuid();
            TargetSearchGame(true,matchID, playerIndex);
        }
        else
        {
            //Debug.Log("<color=red>Game not found</color>");
            TargetSearchGame(false,matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetSearchGame(bool success,string _matchID,int _playerIndex)
    {
        this.playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.Instance.SearchSuccess(success,_matchID);
    }

    #endregion
    
    #region BeginGame
    /*Begin game commands and RPC's*/
    public void BeginGame()
    {
        CmdBeginGame();
    }
    
    [Command]
    void CmdBeginGame()
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log("<color=red>Game Beginning failed</color>");
    }

    public void StartGame(List<PlayerManager> _players, TurnManager _turnManager)
    {
        if(isServer)
        {
            diceRoll = Random.Range(0, 99);
        }
        TargetBeginGame(_players,_turnManager, _players[0].diceRoll);
    }

    /*[Command]
    public void CmdSetPlayersNotReady()
    {
        NetworkServer.SetClientNotReady(connectionToClient);
    }*/
    [TargetRpc]
    void TargetBeginGame(List<PlayerManager> _players,TurnManager _turnManager, int roll)
    {
        Debug.Log($"MatchID: {matchID} | Beginning");

        myTurnManager = _turnManager;
        _turnManager.players = _players;
        if (roll < 50)
        {
            _players[0].isMyTurn = true;
            _players[1].isMyTurn = false;
            Debug.Log($"<color=yellow> Host plays first </color>");
        }
        else
        {
            _players[0].isMyTurn = false;
            _players[1].isMyTurn = true;
            Debug.Log($"<color=yellow> Client plays first </color>");
        }

        if (isLocalPlayer)
        {
            Debug.Log("Setting client not ready");
            //CmdSetPlayersNotReady();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Additive);
    }
    #endregion

    #region DisconnectGame

    public void DisconnectGame()
    {
        CmdDisconnectGame();
    }
    [Command]
    void CmdDisconnectGame()
    {
        ServerDisconnect();
    }

    void ServerDisconnect()
    {
        try
        {
            MatchMaker.instance.PlayerDisconnected(this,matchID);
            RpcDisconnectGame();
            networkMatchChecker.matchId = string.Empty.ToGuid();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [ClientRpc]
    void RpcDisconnectGame()
    {
        ClientDisconnect();
    }

    void ClientDisconnect()
    {
        if (playerLobbyUI != null)
        {
            Destroy(playerLobbyUI);
        }
    }
    #endregion

}

