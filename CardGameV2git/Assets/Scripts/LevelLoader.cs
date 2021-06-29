using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public List<GameObject> transitionList;
    public NetworkManager networkManager;
    public float transitionTime = 1f;

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0) networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    public void LoadNextLevel(string scene)
    {
        if(scene.Equals("MainMenu"))
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex+1));
        else if (scene.Equals("Lobby"))
            StartCoroutine(LoadLobby());
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transitionList[0].SetActive(true);
        transitionList[1].SetActive(false);
        transitionList[0].GetComponent<Animator>().SetTrigger("Start");
        

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    IEnumerator LoadLobby()
    {
        transitionList[0].SetActive(false);
        transitionList[1].SetActive(true);
        transitionList[1].GetComponent<Animator>().SetTrigger("Start");
        
        yield return new WaitForSeconds(transitionTime);
        if (networkManager != null)
        {
            networkManager.StartClient();
        }
        
    }
}
