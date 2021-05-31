using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class deckPrefab2 : MonoBehaviour
{
    public void SelectDeckToPlay()
    {
        Color c;

        foreach (Transform trans in this.transform.parent.transform)
        {
            c = trans.gameObject.GetComponent<Image>().color;
            c.a = 0;
            trans.gameObject.GetComponent<Image>().color = c;
        }
        c  = GetComponent<Image>().color;
        c.a = 1;
        GetComponent<Image>().color = c;
        DeckManager.Instance.SelectedDeckToPlay = this.transform.GetSiblingIndex();

        UIManager.Instance.PlayButton.GetComponent<Button>().interactable = true;
        UIManager.Instance.PlayButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }
}
