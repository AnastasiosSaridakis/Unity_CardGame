using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Unity;
using Firebase;
using Firebase.Auth;

public class DataBridge : MonoBehaviour
{
    private static DataBridge _instance;
    public static DataBridge Instance { get { return _instance; } }

    public TextMeshProUGUI usernameInput, passwordInput;
    private Player data;
    private List<Deck> deck;
    private List<string> dbd;

    private string DATA_URL = "https://dcardgame-6d190-default-rtdb.europe-west1.firebasedatabase.app/";

    private DatabaseReference databaseReference;

    void Awake()
    {
    #if UNITY_EDITOR
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
    #endif
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
        databaseReference = FirebaseDatabase.GetInstance(DATA_URL).RootReference;
        //LoadData();
    }

    public void SaveData(Deck data, string deckNumber)
    {
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Decks").Child(deckNumber).SetRawJsonValueAsync(jsonData).ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SaveData task is canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.Log("SaveData task is faulted.");
            }
            if (task.IsCompleted)
            {
                print("SaveData is completed");
            }
        }));
    }

    public void SaveDeckName(string deckName)
    {
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Decks").Child(DeckManager.Instance.currentSelectedDeckNum.ToString()).Child("DeckName").SetRawJsonValueAsync(jsonData).ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SaveData task is canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.Log("SaveData task is faulted.");
            }
            if (task.IsCompleted)
            {
                print("SaveData is completed");
            }
        }));
    }

    public void DeleteData(int deckToDelete)
    {
        databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Decks").Child(deckToDelete.ToString()).RemoveValueAsync().ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("DeleteData task is canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.Log("DeleteData task is faulted.");
            }
            if (task.IsCompleted)
            {
                print("DeleteData is completed");
            }
        }));
    }
    


    public void LoadData()
    {
        //FirebaseDatabase.GetInstance(DATA_URL).GetReferenceFromUrl(DATA_URL)
        databaseReference.GetValueAsync().ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("LoadData task is canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.Log("LoadData task is faulted.");
            }
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
               
                string playerData = snapshot.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).GetRawJsonValue();

                Player m = JsonUtility.FromJson<Player>(playerData);
                
                foreach (var child in snapshot.Child("Users").Children)
                {
                    string t = child.GetRawJsonValue();
                    Player extractedData = JsonUtility.FromJson<Player>(t);
                    
                    if (extractedData.Pid == FirebaseAuth.DefaultInstance.CurrentUser.UserId)
                    {
                        //print("Username of p is: " + extractedData.Username);
                        //print("pid of p is: " + extractedData.Pid);

                        DeckManager.Instance.SetUsername(extractedData.Username);

                        //string t2 = child.Child("Decks").GetRawJsonValue();
                        //Deck extractedData2 = JsonUtility.FromJson<Deck>(t2);


                        foreach (var deck in child.Child("Decks").Children)
                        {
                            string t2 = deck.GetRawJsonValue();
                            Deck extractedData2 = JsonUtility.FromJson<Deck>(t2);

                            DeckManager.Instance.PlayerDeckList.Add(extractedData2);
                           // print("deck value is: " + extractedData2.PlayerDeck);
                        }
                    }
                }

                
            }

        }));
    }

    public void QuitGame()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
        Application.Quit();
    }


}
