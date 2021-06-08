using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


//[System.Serializable]
//public class SyncListCard : SyncList<Card>{}
public class TurnManager : NetworkBehaviour
{
    public List<PlayerManager> players = new List<PlayerManager>();  // FIX: this should be private, is should get the players from the servers List.

    public void AddPlayer(PlayerManager _player)
    {
        players.Add(_player);
    }
}
