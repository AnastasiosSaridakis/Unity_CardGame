using System;
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
    public List<Ability> abilities;


    public string GetDescription()
    {
        string str = "";
        if (abilities == null)
            return "";
        foreach (Ability ability in abilities)
        {
            if(str.Equals(""))
                str = str + ability.abilityName;
            else
                str = str +"\n" +ability.abilityName;
        }

        return str;
    }

    public Card()
    {

    }

    public Card(int Id, string Name, int Cost, int Attack, int Health, string Description, Sprite Artwork, List<Ability> Abilities)
    {
        id = Id;
        cardname = Name;
        cost = Cost;
        attack = Attack;
        health = Health;
        description = Description;
        artworkImage = Artwork;
        abilities = Abilities;
    }
}


