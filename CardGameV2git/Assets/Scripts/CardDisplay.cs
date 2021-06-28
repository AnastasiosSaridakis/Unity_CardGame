using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardInfo
{
    public int id;
    public int cost;
    public int attack;
    public int health;
    public string name;
    public string description;

    public void SetStats(CardDisplay cd)
    {
        cost = int.Parse(cd.manaText.text);
        attack = int.Parse(cd.attackText.text);
        health =int.Parse(cd.healthText.text);
        name = cd.nameText.text;
        description = cd.descriptionText.text;
    }
    public CardInfo(int id)
    {
        foreach (var card in CardDatabase.Instance.cardList.Where(card => card.id == id)) //When i first Instantiate the card throught the network
        {
            this.id = card.id;
            cost = card.cost;
            attack = card.attack;
            health = card.health;
            name = card.cardname;
            //description = card.description;
            description = card.GetDescription();
        }
    }

    public CardInfo(){}
}
public class CardDisplay : MonoBehaviour
{
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;
    public List<Ability> abilities;
    public GameObject cardBackImage;
    public bool isCardBackActive;




    void Start()
    {
        if (card != null) // used when Instantiating a card for the first time
        {
            nameText.text = card.cardname;
            manaText.text = card.cost.ToString();
            healthText.text = card.health.ToString();
            attackText.text = card.attack.ToString();
            //descriptionText.text = card.description;
            artworkImage.sprite = card.artworkImage;
            abilities = card.abilities;
            descriptionText.text = card.GetDescription();
        }
    }

    public void setHealth(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    public void setAttack(int newAttack)
    {
        attackText.text = newAttack.ToString();
    }
    public int GetHealth()
    {
        return int.Parse(healthText.text);
    }
    public int GetAttack()
    {
        return int.Parse(attackText.text);
    }

    public void CopyStats(Card cardToCopy) //Used in ZoomCard
    {
        nameText.text = cardToCopy.cardname;
        manaText.text = cardToCopy.cost.ToString();
        healthText.text = cardToCopy.health.ToString();
        attackText.text = cardToCopy.attack.ToString();
        //descriptionText.text = cardToCopy.description;
        artworkImage.sprite = cardToCopy.artworkImage;
        descriptionText.text = cardToCopy.GetDescription();
    }
    public void InitializeStats(CardInfo info)
    {
        manaText.text = info.cost.ToString();
        healthText.text = info.health.ToString();
        attackText.text = info.attack.ToString();
        nameText.text = info.name;
        descriptionText.text = info.description;
        foreach (var _card in CardDatabase.Instance.cardList.Where(_card => _card.id == info.id))
        {
            artworkImage.sprite = _card.artworkImage;
            abilities = _card.abilities;
            this.card = _card;
        }
    }

    public void FlipCard()
    {
        isCardBackActive = !isCardBackActive;
        if (isCardBackActive)
        {
            gameObject.GetComponent<CardDisplay>().cardBackImage.SetActive(true);
        }
        else
        {
            gameObject.GetComponent<CardDisplay>().cardBackImage.SetActive(false);
        }
    }
}
