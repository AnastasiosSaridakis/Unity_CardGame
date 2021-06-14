using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{

    public int id;
    public string cardname;
    public int cost;
    public int attack;
    public int health;
    public string description;
    public Sprite artworkImage;
    
    public Card()
    {

    }

    public Card(int Id, string Name, int Cost, int Attack, int Health, string Description, Sprite Artwork)
    {
        id = Id;
        cardname = Name;
        cost = Cost;
        attack = Attack;
        health = Health;
        description = Description;
        artworkImage = Artwork;
    }
}
