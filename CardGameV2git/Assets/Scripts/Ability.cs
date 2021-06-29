using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;

    public Ability()
    {
        
    }

    public Ability(string name)
    {
        abilityName = name;
    }
}
    
