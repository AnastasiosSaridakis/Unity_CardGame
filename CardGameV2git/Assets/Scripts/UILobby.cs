using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private Button joinButton;
    [SerializeField]
    private Button hostButton;
    [SerializeField] 
    private GameObject playersPanel;

    [Header("Lobby")] 
    [SerializeField] 
    private Transform UIPlayerParent;
    [SerializeField] 
    private GameObject UIPlayerPrefab;
    [SerializeField] 
    private TMP_Text matchIDText;
    [SerializeField]
    private Button beginGameButton;
    
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
    
    public void Host()
    {
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        
        PlayerManager.localPlayer.HostGame();
    }
    public void Join()
    {
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        
        PlayerManager.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
    }

    public void HostSuccess(bool success,string matchID)
    {
        if (success)
        {
            //Debug.Log("setting canvas to true here");
            playersPanel.SetActive(true);
            
            SpawnPlayerUIPrefab(PlayerManager.localPlayer);
            matchIDText.text = matchID;
            beginGameButton.interactable = true;
            Debug.Log("spawning UI pref (HOSTSuccess)");
        }
        else
        {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
           // Debug.Log("you joined a game!");
            playersPanel.SetActive(true);
            
            SpawnPlayerUIPrefab(PlayerManager.localPlayer);
            matchIDText.text = matchID;
            Debug.Log("spawning UI pref (JOINSuccess)");
        }
        else
        {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void SpawnPlayerUIPrefab(PlayerManager player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        //newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
    }

    public void BeginGame()
    {
        PlayerManager.localPlayer.BeginGame();
    }
    
    
}
