using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBack : MonoBehaviour
{
    public GameObject cardBack;
    public GameObject manaImage;

    public bool isFlipped;

    void Start()
    {
        if (isFlipped)
        {
            cardBack.SetActive(true);
            manaImage.SetActive(false);
            isFlipped = true;
        }
    }

    private void Update()
    {
        if (isFlipped)
        {
            TurnCard();
        }
    }

    public void TurnCard()
    {
        if (isFlipped)
        {
            cardBack.SetActive(true);
            manaImage.SetActive(false);
            isFlipped = true;
        }
        else
        {
            cardBack.SetActive(false);
            manaImage.SetActive(true);
            isFlipped = false;
        }
    }
}
