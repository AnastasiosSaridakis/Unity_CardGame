using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public Material material;

    bool isDissolving = false;
    float fade = 1f;

    public Image[] cardArtworkList;
    public TextMeshProUGUI[] textList;

    public AnimationClip attackingAnimation;

    void Start()
    {
        fade = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDissolving)
        {
            fade -= Time.deltaTime;

            if(fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
                Destroy(this.gameObject);
            }

            foreach (TextMeshProUGUI child in textList)
            {
                child.alpha = fade;
            }

            material.SetFloat("_Fade", fade);
        }
    }

    public void StartDissolving()
    {
        wait();
        material.SetFloat("_Fade", fade);
        foreach (Image child in cardArtworkList)
        {
            child.material = material;
        }
        isDissolving = true;
    }

    IEnumerator wait()
    {
        Debug.Log("before yield");
        yield return new WaitForSeconds(5);
        Debug.Log("animation clip time is: " + attackingAnimation.length);
        //yield return new WaitForSeconds(attackingAnimation.length);
    }
}
