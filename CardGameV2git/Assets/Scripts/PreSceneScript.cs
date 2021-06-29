using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreSceneScript : MonoBehaviour
{
    public MMFeedbacks textAppearance;

    public GameObject spaceText;
    // Start is called before the first frame update
    void Start()
    {
        textAppearance.PlayFeedbacks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && spaceText.activeSelf)
        {
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
