using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    private static DeckManager _instance;
    public static DeckManager Instance { get { return _instance; } }

    /*public Button previousPageBtn, nextPageBtn;
    public TextMeshProUGUI pageNumberTxt,cardsInDeckTxt,exitDialogTxt;
    public TMP_InputField deckTitle;*/
    string deckName;
    
    public int currentPageNumber, totalNumberOfPages;
    public List<Deck> PlayerDeckList;
    public int currentSelectedDeckNum;
    public Deck currentSelectedDeck;
    Transform[] count;
    Transform[] countOrdered;
    public bool isDragging = false;
    public int currentCardsInDeck;
    bool isNewDeck = false;
    public List<SimpleCard> currentDeckList;
    public string username;
    public bool dataToLoad = false;

    public int SelectedDeckToPlay;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }

    public void Start()
    {
        DataBridge.Instance.LoadData();
    }

    public void Update()
    {
        if (dataToLoad)
        {
            UIManager.Instance.connectedText.SetText("Connected as: " + username);
            dataToLoad = false;
            //Debug.Log("im in update");
        }
    }

    public void SetUsername(string uname)
    {
        username = uname;
        dataToLoad = true;
    }
    public void GoToPreviousPage()
    {
        currentPageNumber--;
        UIManager.Instance.pageNumberTxt.SetText(currentPageNumber + "/" + totalNumberOfPages);
        if (currentPageNumber == 1)
        {
            UIManager.Instance.previousPageBtn.interactable = false;
        }
        if(currentPageNumber == totalNumberOfPages)
        {
            UIManager.Instance.nextPageBtn.interactable = false;
        }
        else
        {
            UIManager.Instance.nextPageBtn.interactable = true;
        }
        LoadPage();
    }

    public void GoToNextPage()
    {
        currentPageNumber++;
        UIManager.Instance.pageNumberTxt.SetText(currentPageNumber + "/" + totalNumberOfPages);
        if (currentPageNumber == totalNumberOfPages)
        {
            UIManager.Instance.nextPageBtn.interactable = false;
        }
        else
        {
            UIManager.Instance.nextPageBtn.interactable = true;
        }
        if(currentPageNumber > 1)
        {
            UIManager.Instance.previousPageBtn.interactable = true;
        }
        LoadPage();
    }

    public void LoadCardManager()
    {
        currentCardsInDeck = currentSelectedDeck.PlayerDeck.Count;
        UIManager.Instance.cardsInDeckTxt.SetText(currentCardsInDeck + "/30");
        UIManager.Instance.cardsInDeckTxt.color = Color.white;
        currentPageNumber = 1;
        totalNumberOfPages = CardDatabase.Instance.cardList.Count / 10;// MAKE THE CARDDATABASE TO START FROM HERE AND BE PRESERVED THROUGH SCENES
        if (CardDatabase.Instance.cardList.Count % 10 != 0)
        {
            totalNumberOfPages++;
        }
        UIManager.Instance.pageNumberTxt.SetText(currentPageNumber + "/" + totalNumberOfPages);
        if(totalNumberOfPages == 1)
            UIManager.Instance.nextPageBtn.interactable = false;
        UIManager.Instance.deckTitle.text = currentSelectedDeck.DeckName;
        //foreach (Card card in CardDatabase.Instance.cardList)
        //foreach (int cardID in PlayerDeckList[currentSelectedDeckNum].PlayerDeck)
        foreach (int cardID in currentSelectedDeck.PlayerDeck)
        {
            foreach(Card card in CardDatabase.Instance.cardList)
            {
                if(card.id == cardID)
                {
                    print("instantiating simple card ");
                    GameObject go = Instantiate(UIManager.Instance.simpleCard, UIManager.Instance.currentDeckPanel.transform);
                    go.GetComponent<SimpleCard>().SetStats(card.id, card.cardname, card.cost);
                }
            }
            
        }
        OrderCurrentDeck();
        LoadPage();
    }

    public void SetSelectedDeck()
    {
        currentSelectedDeck = PlayerDeckList[currentSelectedDeckNum];
    }

    //public void OrderCurrentDeck(GameObject simpleCard, int cost)
    public void OrderCurrentDeck()
    {
        count = new Transform[UIManager.Instance.currentDeckPanel.transform.childCount];
        countOrdered = new Transform[UIManager.Instance.currentDeckPanel.transform.childCount];
        for (int i = 0; i < UIManager.Instance.currentDeckPanel.transform.childCount; i++)
        {
            count[i] = UIManager.Instance.currentDeckPanel.transform.GetChild(i);
        }
        countOrdered = count.OrderBy(go => go.gameObject.GetComponent<SimpleCard>().cost).ThenBy(go => go.gameObject.GetComponent<SimpleCard>().cName).ToArray();
        for (int i = 0; i < countOrdered.Length; i++)
        {
            countOrdered[i].SetSiblingIndex(i);
        }
    }

    public void RefreshText()
    {
        UIManager.Instance.cardsInDeckTxt.SetText(currentCardsInDeck + "/30");
        if (currentCardsInDeck > 30)
            UIManager.Instance.cardsInDeckTxt.color = Color.red;
        else
            UIManager.Instance.cardsInDeckTxt.color = Color.white;
    }

    public void LoadPage()
    {
        for(int i = UIManager.Instance.cardDatabasePanel.transform.childCount-1; i >= 0; i--)
        {
            Destroy(UIManager.Instance.cardDatabasePanel.transform.GetChild(i).gameObject);
        }
        if (currentPageNumber != totalNumberOfPages)
        {
            for (int i = (currentPageNumber - 1) * 10; i < currentPageNumber * 10; i++)
            {
                Card card = CardDatabase.Instance.orderedCardList[i];

                GameObject go = Instantiate(UIManager.Instance.defaultCard, UIManager.Instance.cardDatabasePanel.transform);
                go.GetComponent<CardDefault>().SetStats(card);
                go.GetComponent<CardDefault>().cardID = card.id;
                go.GetComponent<CardDefault>().cost = card.cost;
                go.GetComponent<CardDefault>().cardName = card.name;
                go.GetComponent<CardDefault>().CheckForCopies();
            }
        }
        else//we are in the last page
        {
            for (int i = (currentPageNumber - 1) * 10; i < (currentPageNumber - 1) * 10 + CardDatabase.Instance.cardList.Count % 10 ; i++)
            {
                Card card = CardDatabase.Instance.orderedCardList[i];

                GameObject go = Instantiate(UIManager.Instance.defaultCard, UIManager.Instance.cardDatabasePanel.transform);
                go.GetComponent<CardDefault>().SetStats(card);
                go.GetComponent<CardDefault>().cardID = card.id;
                go.GetComponent<CardDefault>().CheckForCopies();
            }

        }
        
    }

    public void ExitDialog()
    {
        if(currentCardsInDeck > 30)
        {
            UIManager.Instance.exitDialogTxt.SetText("You have more than 30 cards in your current deck. If you exit now, your deck will NOT be saved. Continue?");
        }
        else if(currentCardsInDeck<30)
        {
            UIManager.Instance.exitDialogTxt.SetText("You have less than 30 cards in your current deck. If you exit now, your deck will NOT be saved. Continue?");
        }
        else
        {
            UIManager.Instance.exitDialogTxt.SetText("If you exit now, your deck will be saved. Continue?");
        }
    }

    public void SaveDeck()
    {
        if(currentCardsInDeck == 30)
        {
            print(deckName);
            Deck deck = new Deck(deckName, UIManager.Instance.GetCurrentDeck());
            //UIManager.Instance.listOfDecks.transform.GetChild(currentSelectedDeckNum).gameObject.GetComponent<deckButtons>().SetTitle(deckTitle.text);
            DataBridge.Instance.SaveData(deck, currentSelectedDeckNum.ToString());
            PlayerDeckList[currentSelectedDeckNum] = deck;
            isNewDeck = false;
            UIManager.Instance.LoadDecks();
        }
        else
        {
            Debug.Log("Cards not 30, not saving");
            if (isNewDeck)
            {
                UIManager.Instance.addDeckBtn.GetComponent<Button>().interactable = true;
                PlayerDeckList.RemoveAt(currentSelectedDeckNum);
            }
        }

        for(int i = 0; i<UIManager.Instance.currentDeckPanel.transform.childCount; i++) //Clear currentDeckPanel
        {
            Destroy(UIManager.Instance.currentDeckPanel.transform.GetChild(i).gameObject);
        }
    }

    public void CheckDeckName()
    {
        if(UIManager.Instance.deckTitle.text.Length >= 20)
        {
            UIManager.Instance.deckTitle.text = deckName;
        }
        else
        {
            deckName = UIManager.Instance.deckTitle.text;
        }
    }

    public void SaveDeckName()
    {
        DataBridge.Instance.SaveDeckName(deckName);
        PlayerDeckList[currentSelectedDeckNum].DeckName = deckName;
        Debug.Log("Saving deck name... : " + deckName);
    }

    public void AddDeck()
    {
        if(PlayerDeckList.Count == 7)
        {
            UIManager.Instance.addDeckBtn.GetComponent<Button>().interactable = false;
        }
        List<int> emptyList = new List<int>();
        Deck newDeck = new Deck("New Deck", emptyList);
        PlayerDeckList.Add(newDeck);
        currentSelectedDeckNum = PlayerDeckList.Count-1;
        currentSelectedDeck = PlayerDeckList[currentSelectedDeckNum];
        isNewDeck = true;
        LoadCardManager();
    }

    public void DeleteDeck()
    {
        UIManager.Instance.addDeckBtn.GetComponent<Button>().interactable = true;

        PlayerDeckList.Remove(currentSelectedDeck);
        UIManager.Instance.RemoveDeckFromList(currentSelectedDeckNum);
        print("Before Delete");
        print("After Delete");
        if (PlayerDeckList.Count != 0)//OVERWRITE INDEXES
        {
            for (int i = 0; i < PlayerDeckList.Count; i++)
            {
                print("save i is: " + i);
                DataBridge.Instance.SaveData(PlayerDeckList[i], i.ToString());
            }
        }

        DataBridge.Instance.DeleteData(PlayerDeckList.Count); //delete last value
        print("end of delete AND save");
        if (PlayerDeckList.Count != 0)//OVERWRITE INDEXES
        {
            for (int i = 0; i < PlayerDeckList.Count; i++)
            {
                print("save i is: " + i);
                DataBridge.Instance.SaveData(PlayerDeckList[i], i.ToString());
            }
        }
    }
}
