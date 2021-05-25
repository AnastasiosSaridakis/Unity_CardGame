using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tutorialOLD
{public class PlayerConnection : NetworkBehaviour
{
    public static PlayerConnection localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    private NetworkMatchChecker networkMatchChecker;
    private void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        if (isLocalPlayer)
        {
            localPlayer = this;
            Debug.Log(("IT IS LOCAL PLAYER"));
        }
        else
        {
            //UILobby.Instance.SpawnPlayerUIPrefab(this);
            Debug.Log(("NOT LOCAL PLAYER"));
        }
    }

    #region Host
    /*Host game commands and RPC's*/
    public void HostGame()
    {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID)
    {
        matchID = _matchID;
        if(MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex))
        {
            Debug.Log("<color = green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetHostGame(true,_matchID);
        }
        else
        {
            Debug.Log("<color = red>Game hosted failed</color>");
            TargetHostGame(false,_matchID);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success,string _matchID)
    {
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
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
            Debug.Log("<color = green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true,_matchID);
        }
        else
        {
            Debug.Log("<color = red>Game hosted failed</color>");
            TargetJoinGame(false,_matchID);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success,string _matchID)
    {
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.Instance.JoinSuccess(success,_matchID);
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
        Debug.Log("<color = red>Game Beginning failed</color>");
    }

    public void StartGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    void TargetBeginGame()
    {
        Debug.Log($"MatchID: {matchID} | Beginning");
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }
    

    #endregion
}
    
}

