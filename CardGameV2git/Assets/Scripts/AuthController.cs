using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Database;
public class AuthController : MonoBehaviour
{
    public TextMeshProUGUI emailInput, emailRegInput, LoginLabel, RegisterLabel;
    public TMP_InputField passwordInput, passwordRegInput;
    public string errorTextLoggin, errorTextRegister;
    public string username;
    public string password;
    private bool LoadNextScene = false;
    public List<Deck> decks= new List<Deck>();
    public List<int> deck= new List<int>();
    public List<int> deck2= new List<int>();
    public Deck deckItem;
    public Deck deckItem2;

    private string DATA_URL = "https://dcardgame-6d190-default-rtdb.europe-west1.firebasedatabase.app/";
    private DatabaseReference databaseReference;
    

    private void Start()
    {
        #if UNITY_EDITOR
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        #endif
        
      //  databaseReference = FirebaseDatabase.GetInstance(DATA_URL).RootReference;
      databaseReference = FirebaseDatabase.DefaultInstance.RootReference; 
        if (databaseReference == null)
        {
            Debug.Log("database referens is null");
            errorTextLoggin = "null";
        }
        else
        {
            Debug.Log("database reference: "+databaseReference.ToString());
            errorTextLoggin = databaseReference.ToString();
        }
        
        for (int i = 0; i< 30; i++)
        {
            deck.Add(i % 3);
            deck2.Add(3);
        }
        deckItem.DeckName = "Starter Deck";
        deckItem2.DeckName = "Second Deck";
        deckItem.PlayerDeck = deck;
        deckItem2.PlayerDeck = deck2;
        decks.Add(deckItem);
        decks.Add(deckItem2);
    }
    public void Login()
    {
        //string pass = passwordInput.text;
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text).ContinueWith((task =>
        {
            Debug.Log("Password Log is: " + passwordInput.text);
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode, "Loggin");
                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode, "Loggin");
                return;
            }
            if (task.IsCompleted)
            {
                print("User logged in");
                LoadNextScene = true;
            }
        }));
    }

    private void Update()
    {
        if(errorTextLoggin != "")
        {
            LoginLabel.SetText(errorTextLoggin);
        }

        if (errorTextLoggin != "")
        {
            LoginLabel.SetText(errorTextLoggin);
        }

        if (LoadNextScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Logout()
    {
        if(FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
    }

    public void Register()
    {
        if(emailRegInput.text.Equals("") && passwordRegInput.text.Equals(""))
        {
            print("Please enter an email and password to register");
            return;
        }
        Logout();
        password = passwordRegInput.text;
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailRegInput.text, password).ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode, "Register");
                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode, "Register");
                return;
            }
            if (task.IsCompleted)
            {
                print("Registration Completed"); //from here try to save into firebase, username Pid and deck.
                Player data = new Player(username, FirebaseAuth.DefaultInstance.CurrentUser.UserId, password);
                Deck data2 = new Deck(decks[0].DeckName, decks[0].PlayerDeck);
                Deck data3 = new Deck(decks[1].DeckName, decks[1].PlayerDeck);
                string jsonData = JsonUtility.ToJson(data);
                string jsonData2 = JsonUtility.ToJson(data2);
                string jsonData3 = JsonUtility.ToJson(data3);
                //databaseReference.Child("Users").Child(username).SetRawJsonValueAsync(jsonData);
                databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(jsonData);
                databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Decks").Child("0").SetRawJsonValueAsync(jsonData2);
                databaseReference.Child("Users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Decks").Child("1").SetRawJsonValueAsync(jsonData3);

                gameObject.GetComponent<Registration>().SwitchPanel();
            }
        }));
    }

    void GetErrorMessage(AuthError errorCode, string type)
    {
        string msg = "";
        msg = errorCode.ToString();
        print(msg);
        if (type == "Loggin")
        {
            switch (errorCode)
            {
                case AuthError.UserNotFound:
                    errorTextLoggin = "UserNotFound";
                    print(msg);
                    
                    break;
                case AuthError.MissingPassword:
                    errorTextLoggin = "Email or Password is wrong";
                    print(msg);
                    
                    break;
                case AuthError.WrongPassword:
                    errorTextLoggin = "Email or Password is wrong";
                    print(msg);
                    
                    break;
                case AuthError.InvalidEmail:
                    errorTextLoggin = "InvalidEmail";
                    print(msg);
                    
                    break;
            }
        }
        else if (type == "Register")
        {
            switch (errorCode)
            {
                case AuthError.AccountExistsWithDifferentCredentials:
                    errorTextRegister = "Account already exists";
                    gameObject.GetComponent<Registration>().errorLabel.SetText(errorTextRegister);
                    Debug.Log("AccountExistsWithDifferentCredentials");
                    break;
                case AuthError.InvalidEmail:
                    errorTextRegister = "Please input a correct email";
                    gameObject.GetComponent<Registration>().errorLabel.SetText(errorTextRegister);
                    Debug.Log("InvalidEmail");
                    break;
                case AuthError.EmailAlreadyInUse:
                    errorTextRegister = "Email Already In Use";
                    gameObject.GetComponent<Registration>().errorLabel.SetText(errorTextRegister);
                    Debug.Log("EmailAlreadyInUse");
                    break;
            }
        }
        else
        {
            Debug.Log("Neither Loggin nor Register type");
        }
    }

    public void SetUsername(string uname)
    {
        username = uname;
    }
}
