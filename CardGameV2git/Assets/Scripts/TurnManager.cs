using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : NetworkBehaviour
{
    private List<PlayerManager> players = new List<PlayerManager>();
    public void AddPlayer(PlayerManager _player)
    {
        players.Add(_player);
    }
}
