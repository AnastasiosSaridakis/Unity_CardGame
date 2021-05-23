using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour
{
    public static PlayerConnection localPlayer;
    [SyncVar] public string matchID;

    private NetworkMatchChecker networkMatchChecker;
    private void Start()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public void HostGame()
    {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID)
    {
        matchID = _matchID;
        if(MatchMaker.instance.HostGame(_matchID, gameObject))
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
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.Instance.HostSuccess((success));
    }

}
