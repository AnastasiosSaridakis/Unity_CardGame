using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Registration : MonoBehaviour
{
    public TMP_InputField logginEmail;
    public TMP_InputField logginPassword;
    public TextMeshProUGUI loginErrorLabel;
    public TMP_InputField emailField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPwField;
    public TextMeshProUGUI errorLabel;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public AuthController authC;
    public Button submitRegisterBtn;
    private string username = "";

    
    public void VerifyInput()
    {
        if (usernameField.text.Length >10 || usernameField.text.Length<4)
        {
            errorLabel.SetText("Username must be between 4-10 characters");
            submitRegisterBtn.interactable = false;
        }
        else if(passwordField.text.Length < 8)
        {
            errorLabel.SetText("Password must be more than 8 characters");
            submitRegisterBtn.interactable = false;
        }
        else if (passwordField.text != confirmPwField.text)
        {
            errorLabel.SetText("Passwords must match");
            submitRegisterBtn.interactable = false;
        }
        else
        {
            errorLabel.SetText("");
            username = usernameField.text;
            submitRegisterBtn.interactable = true;
        }
    }

    public void SwitchPanel()
    {
        Debug.Log("Hi im in Switchpanel");
        emailField.text = "";
        authC.SetUsername(username);
        usernameField.text = "";
        passwordField.text = "";
        confirmPwField.text = "";
        errorLabel.text = "";
        logginEmail.text = "";
        logginPassword.text = "";
        loginErrorLabel.text = "";
        gameObject.GetComponent<AuthController>().errorTextLoggin = "";
        gameObject.GetComponent<AuthController>().errorTextRegister = "";
        loginPanel.gameObject.SetActive(!loginPanel.gameObject.activeSelf);
        registerPanel.gameObject.SetActive(!registerPanel.gameObject.activeSelf);
    }

    public void exitApplication()
    {
        Debug.Log("EXITING APP");
        Application.Quit();
    }
}


