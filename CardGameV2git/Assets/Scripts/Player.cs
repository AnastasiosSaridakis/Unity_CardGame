using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public string Username,Pid,Password;
    //public List<int>[] decks = new List<int>[5];
    //public List<Deck> DeckList = new List<Deck>();

    public Player() { }

    public Player(string name, string id, string password)
    {
        Username = name;
        Pid = id;
        Password = password;
    }
}

[Serializable]
public class Deck
{
    public string DeckName;
    public List<int> PlayerDeck;

    public Deck(string name, List<int> deck)
    {
        DeckName = name;
        PlayerDeck = deck;
    }
}
