using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    private static UILobby _instance;
    
    public static UILobby Instance { get { return _instance; } }
    
    [Header("Host/Join Elements")] 
    [SerializeField]
    private TMP_InputField joinMatchInput;

    [SerializeField] private List<Selectable> lobbySelectables = new List<Selectable>();
    [SerializeField] private GameObject mainPanel;
    [SerializeField] 
    private GameObject playersPanel;
    [SerializeField] 
    private GameObject searchCanvas;

    [Header("Lobby")] 
    [SerializeField] 
    private Transform UIPlayerParent;
    [SerializeField] 
    private GameObject UIPlayerPrefab;
    [SerializeField] 
    private TMP_Text matchIDText;
    [SerializeField]
    private Button beginGameButton;
    [SerializeField]
    private GameObject playerLobbyUI;
    [SerializeField]
    private GameObject LobbyList;

    private bool searching = false;
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

    private void Update()
    {
        if (playersPanel.activeSelf && LobbyList.transform.childCount < 2)
        {
            if (beginGameButton.interactable)
            {
                beginGameButton.interactable = false;
            }
        }
        else if (playersPanel.activeSelf && LobbyList.transform.childCount == 2)
        {
            if (!beginGameButton.interactable)
            {
                beginGameButton.interactable = true;
            }

        }
    }

    public void HostPrivate()
    {
        joinMatchInput.interactable = false;
        
        lobbySelectables.ForEach(x => x.interactable = false);
        
        PlayerManager.localPlayer.HostGame(false);
    }
    public void HostPublic()
    {
        joinMatchInput.interactable = false;
        
        lobbySelectables.ForEach(x => x.interactable = false);
        
        PlayerManager.localPlayer.HostGame(true);
    }
    public void Join()
    {
        joinMatchInput.interactable = false;
        lobbySelectables.ForEach(x => x.interactable = false);
        
        PlayerManager.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
    }

    public void HostSuccess(bool success,string matchID)
    {
        if (success)
        {
            mainPanel.SetActive(false);
            playersPanel.SetActive(true);
            
            if(playerLobbyUI != null) Destroy(playerLobbyUI);
            playerLobbyUI = SpawnPlayerUIPrefab(PlayerManager.localPlayer);
            matchIDText.text = matchID;
            beginGameButton.gameObject.SetActive(true);
            //beginGameButton.interactable = false;
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
           // Debug.Log("you joined a game!");
            mainPanel.SetActive(false);
            playersPanel.SetActive(true);
            
            if(playerLobbyUI != null) Destroy(playerLobbyUI);
            playerLobbyUI = SpawnPlayerUIPrefab(PlayerManager.localPlayer);
            //PlayerManager.localPlayer.showPlayersUI();
            matchIDText.text = matchID;
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public GameObject SpawnPlayerUIPrefab(PlayerManager player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        //newUIPlayer.transform.SetSiblingIndex(player.playerIndex-1);  //Uncomment it if you want all clients to be syncronized in the lobby panel and show in the same order.
        return newUIPlayer;
    }

    public void BeginGame()
    {
        PlayerManager.localPlayer.BeginGame();
    }

    public void SearchGame()
    {
        //Debug.Log("Searching for game!..");
        mainPanel.SetActive(false);
        searchCanvas.SetActive(true);
        StartCoroutine(SearchingForGame());
    }

    IEnumerator SearchingForGame()
    {
        // searching = true; // an example of a coroutine with wait for seconds.
        // WaitForSeconds chechEveryFewSeconds = new WaitForSeconds(1);
        // while (searching)
        // {
        //     yield return chechEveryFewSeconds;
        //     if (searching)
        //     {
        //         PlayerManager.localPlayer.SearchGame();
        //     }
        // }
        
        searching = true; // an example of a coroutine with time -= time.deltatime.
        float currentTime = 1;
        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 1;
                PlayerManager.localPlayer.SearchGame();
            }
            yield return null;
        }

    }
    public void SearchSuccess(bool success, string matchID)
    {
        
        if (success)
        {
            searchCanvas.SetActive(false);
            JoinSuccess(success,matchID);
            searching = false;
        }
    }

    public void SearchCancel()
    {
        mainPanel.SetActive(true);
        searchCanvas.SetActive(false);
        searching = false;
        lobbySelectables.ForEach(x => x.interactable = true);
    }

    public void DisconnectLobby()
    {
        if(playerLobbyUI != null) Destroy(playerLobbyUI);
        PlayerManager.localPlayer.DisconnectGame();

        mainPanel.SetActive(true);
        playersPanel.SetActive(false);
        lobbySelectables.ForEach(x=>x.interactable = true);
        beginGameButton.gameObject.SetActive(false);
    }

    public void ButtonClickedSound()
    {
        AudioManager.instance.Play("button");
    }

    public void DisconnectClient()
    {
        NetworkManager.singleton.StopClient();
    }
}
