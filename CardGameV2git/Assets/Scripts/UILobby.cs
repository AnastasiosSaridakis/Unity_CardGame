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
        
        PlayerConnection.localPlayer.HostGame();
    }
    public void Join()
    {
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
    }

    public void HostSuccess(bool success)
    {
        if (success)
        {
            Debug.Log("setting canvas to true here");
        }
        else
        {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void JoinSuccess(bool success)
    {
        if (success)
        {
        }
        else
        {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    } 
    
    
}
