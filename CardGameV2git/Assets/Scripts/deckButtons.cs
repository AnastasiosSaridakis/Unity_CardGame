using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class deckButtons : MonoBehaviour
{
    public Button deleteDeckButton;
    public Button editDeckBitton;
    public TextMeshProUGUI title;

    private void Start()
    {
        for (int i = 0; i < UIManager.Instance.listOfDecks.transform.childCount; i++)
        {
            if (UIManager.Instance.listOfDecks.transform.GetChild(i).gameObject == this.gameObject)
            {
                title.SetText(DeckManager.Instance.PlayerDeckList[i].DeckName);
            }
        }
    }

    public void ShowButtons()
    {
        for (int i = 0; i < UIManager.Instance.listOfDecks.transform.childCount; i++)
        {
            UIManager.Instance.listOfDecks.transform.GetChild(i).GetComponent<deckButtons>().HideButtons();
        }
        deleteDeckButton.gameObject.SetActive(true);
        editDeckBitton.gameObject.SetActive(true);
    }

    public void HideButtons()
    {
        deleteDeckButton.gameObject.SetActive(false);
        editDeckBitton.gameObject.SetActive(false);
    }

    public void SetCurrentSelectedDeck() // CurrentSelectedDeck starts with 0 so if no deck is selected then the first deck will be selected because indexes start with 0. Maybe start the for from 1;
    {
        for (int i = 0; i < UIManager.Instance.listOfDecks.transform.childCount; i++)
        {
            if(UIManager.Instance.listOfDecks.transform.GetChild(i).gameObject == this.gameObject)
            {
                DeckManager.Instance.currentSelectedDeckNum = i;
                DeckManager.Instance.SetSelectedDeck();
            }
        }
    }

    public void EditDeck()
    {
        UIManager.Instance.DeckPanel.SetActive(true);
        DeckManager.Instance.LoadCardManager();
    }
    public void DeleteDeck()
    {
        DeckManager.Instance.DeleteDeck();
    }
    public void SetTitle(string deckTitle)
    {
        title.SetText(deckTitle);
    }
}
