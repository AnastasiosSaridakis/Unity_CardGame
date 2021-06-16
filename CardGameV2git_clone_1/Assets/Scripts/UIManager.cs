using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    public GameObject MainMenuPanel;
    public GameObject DeckPanel;
    public GameObject OptionsPanel;
    public GameObject currentDeckPanel;
    public TextMeshProUGUI connectedText;
    public GameObject deckPrefab;
    public GameObject deckPrefab2;
    public GameObject listOfDecks;
    public GameObject simpleCard;
    public GameObject defaultCard;
    public GameObject cardDatabasePanel;
    public GameObject addDeckBtn;
    public GameObject LoadingPanel;
    public GameObject ReadToPlayPanel;
    public GameObject ReadToPlayList;
    public GameObject PlayButton;
    [Header("DeckManager")]
     public Button previousPageBtn, nextPageBtn;
     public TextMeshProUGUI pageNumberTxt,cardsInDeckTxt,exitDialogTxt;
     public TMP_InputField deckTitle;

    
    

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
       // FindObjectOfType<AudioManager>().Play("Title Music");
       DeckManager.Instance.dataToLoad = true;
    }

    private void Update()
    {
        
    }

    public void LoadDecks()
    {
        if (listOfDecks.transform.childCount != 0)//Clear the decks so i can re-instantiate them 
        {
            print("Im inside !=0");
            if(listOfDecks.transform.childCount< DeckManager.Instance.PlayerDeckList.Count)
            {
                print("Im inside listOfDecks.transform.childCount< DeckManager.Instance.PlayerDeckList.Count");
                for (int i = listOfDecks.transform.childCount - 1; i < DeckManager.Instance.PlayerDeckList.Count-1; i++)
                {
                    GameObject go = Instantiate(deckPrefab, listOfDecks.transform);
                    go.GetComponent<deckButtons>().SetTitle(DeckManager.Instance.PlayerDeckList[DeckManager.Instance.currentSelectedDeckNum].DeckName);
                }
            }
            else //just edited the name of a previous deck
            {
                listOfDecks.transform.GetChild(DeckManager.Instance.currentSelectedDeckNum).gameObject.GetComponent<deckButtons>().SetTitle(DeckManager.Instance.PlayerDeckList[DeckManager.Instance.currentSelectedDeckNum].DeckName);
            }
        }
        else
        {
            print("Im inside ==0");
            foreach (var val in DeckManager.Instance.PlayerDeckList)
            {
                GameObject go = Instantiate(deckPrefab, listOfDecks.transform);
            }
        }
        if (DeckManager.Instance.PlayerDeckList.Count == 8)
        {
            addDeckBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void LoadReadyToPlayPanel()
    {
        if(ReadToPlayList.transform.childCount == 0)
        {
            foreach (var child in DeckManager.Instance.PlayerDeckList)
            {
                GameObject deck = Instantiate(deckPrefab2, ReadToPlayList.transform);
                int index = deck.transform.GetSiblingIndex();
                deck.transform.GetComponentInChildren<TextMeshProUGUI>().SetText(DeckManager.Instance.PlayerDeckList[index].DeckName);
            }
        }
    }


    public void DestroyReadyToPlayChildren()
    {
        foreach (Transform transform in ReadToPlayList.transform)
        {
            Destroy(transform.gameObject);
        }
        PlayButton.GetComponent<Button>().interactable = false;
        PlayButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(87, 87, 87,255);
    }

    public void RemoveDeckFromList(int selectedDeckNum)
    {
        Destroy(listOfDecks.transform.GetChild(selectedDeckNum).gameObject);
    }

    public List<int> GetCurrentDeck()
    {
        List<int> deck = new List<int>();
        for(int i=0; i<UIManager.Instance.currentDeckPanel.transform.childCount; i++)
        {
            deck.Add(UIManager.Instance.currentDeckPanel.transform.GetChild(i).GetComponent<SimpleCard>().cardId);
        }
        return deck;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ButtonClickedSound()
    {
        AudioManager.instance.Play("button");
    }

    public void SaveDeck()
    {
        DeckManager.Instance.SaveDeck();
    }
    public void AddDeck()
    {
        DeckManager.Instance.AddDeck();
    }

    public void ExitDialog()
    {
        DeckManager.Instance.ExitDialog();
    }

    public void CheckDeckName()
    {
        DeckManager.Instance.CheckDeckName();
    }   
    public void GoToNextPage()
    {
        DeckManager.Instance.GoToNextPage();
    }   
    public void GoToPreviousPage()
    {
        DeckManager.Instance.GoToPreviousPage();
    }
}
