using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;
    public GameObject cardBackImage;
    public bool isCardBackActive;




    void Start()
    {
        if (card != null)
        {
            nameText.text = card.cardname;
            manaText.text = card.cost.ToString();
            healthText.text = card.health.ToString();
            attackText.text = card.attack.ToString();
            descriptionText.text = card.description;
            artworkImage.sprite = card.artworkImage;
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

    public void CopyStats(Card cardToCopy)
    {
        nameText.text = cardToCopy.cardname;
        manaText.text = cardToCopy.cost.ToString();
        healthText.text = cardToCopy.health.ToString();
        attackText.text = cardToCopy.attack.ToString();
        descriptionText.text = cardToCopy.description;
        artworkImage.sprite = cardToCopy.artworkImage;
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
