using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : NetworkBehaviour
{
    private List<PlayerConnection> players = new List<PlayerConnection>();
    public void AddPlayer(PlayerConnection _player)
    {
        players.Add(_player);
    }
}
