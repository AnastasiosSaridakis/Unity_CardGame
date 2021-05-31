using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerDeck : MonoBehaviour
{
    public PlayerManager PlayerManager;

    public List<Card> deck = new List<Card>();
    public List<Card> debugDeck = new List<Card>();
    public int initialDeckSize;
    public List<GameObject> cardsInDeck;  //To contain it so it can only hold 5 items. Is a list the best solution?
    private GameObject hand;

    void Awake()
    {
        Debug.Log($"Deck Manager has {DeckManager.Instance.PlayerDeckList[DeckManager.Instance.SelectedDeckToPlay].PlayerDeck.Count} cards");
        foreach (int id in DeckManager.Instance.PlayerDeckList[DeckManager.Instance.SelectedDeckToPlay].PlayerDeck)
        {
            foreach (Card card in CardDatabase.Instance.cardList)
            {
                if (id == card.id)
                {
                    deck.Add(card);
                }
            }
        }
        
        Shuffle();
        //for (int i = 0; i < initialDeckSize; i++)
        //{
        //    int x = Random.Range(0, CardDatabase.Instance.cardList.Count);
        //    deck.Add(CardDatabase.Instance.cardList[x]);
        //}

        //hand = GameObject.FindWithTag("Hand");  
    }

    private void Update() //I shall move the cardsInDeck code into a seperate function that triggers whenever a card is drawn OR added.
    {                     //I shall add cards in cardsInDeck when the player ADDS cards in the deck!
        //if (deck.Count < initialDeckSize - 5)
        //{
        //    cardsInDeck[4].gameObject.SetActive(false);
        //}
        //if (deck.Count < initialDeckSize / 2)
        //{
        //    cardsInDeck[3].gameObject.SetActive(false);
        //}
        //if (deck.Count <= initialDeckSize / 4)
        //{
        //    cardsInDeck[2].gameObject.SetActive(false);
        //}
        //if (deck.Count <= initialDeckSize / 10)
        //{
        //    cardsInDeck[1].gameObject.SetActive(false);
        //}
        //if (deck.Count <= 0)
        //{
        //    cardsInDeck[0].gameObject.SetActive(false);
        //}
    }

    void RefreshDeckPanel()
    {
        if (deck.Count < initialDeckSize - 7)
        {
            cardsInDeck[4].gameObject.SetActive(false);
        }
        if (deck.Count < initialDeckSize / 2)
        {
            cardsInDeck[3].gameObject.SetActive(false);
        }
        if (deck.Count <= initialDeckSize / 4)
        {
            cardsInDeck[2].gameObject.SetActive(false);
        }
        if (deck.Count <= initialDeckSize / 10)
        {
            cardsInDeck[1].gameObject.SetActive(false);
        }
        if (deck.Count <= 0)
        {
            cardsInDeck[0].gameObject.SetActive(false);
        }

    }

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            Card tempCard = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = tempCard;
        }
    }
    public void Draw()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        
        if (deck.Count > 0)
        {
            Debug.Log("5");
            Card card = deck[0];
            deck.RemoveAt(0);
            //Debug.Log("I've drawn a " + card.name);
            PlayerManager.DealCards(card.id);
            RefreshDeckPanel();
        }
        else
        {
            Debug.Log("out of cards!");
        }
        
    }
}
