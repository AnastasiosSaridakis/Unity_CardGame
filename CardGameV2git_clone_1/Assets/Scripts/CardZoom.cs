using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : MonoBehaviour
{
    public PlayerManager PlayerManager;
    
    public GameObject hand;
    private GameObject zoomCard;

    public void Awake()
    {
        hand = GameObject.FindWithTag("Hand");
    }
    

    public void OnHoverEnter()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        if (gameObject.transform.position.y+this.GetComponent<RectTransform>().sizeDelta.y * 2>Screen.height)
        {
            float i = Screen.height - this.GetComponent<RectTransform>().sizeDelta.y;
            PlayerManager.CmdZoomCard(this.gameObject, gameObject.transform.position.x, i, Quaternion.identity);
        }
        else
        {
            float i = this.GetComponent<RectTransform>().sizeDelta.y + this.GetComponent<RectTransform>().sizeDelta.y/2;
            PlayerManager.CmdZoomCard(this.gameObject, gameObject.transform.position.x, gameObject.transform.position.y + i + 10, Quaternion.identity);
        }

        //zoomCard = Instantiate(gameObject, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 200), Quaternion.identity, Canvas.transform);
        //zoomCard.layer = LayerMask.NameToLayer("Zoom");
        //zoomCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //zoomCard.GetComponent<Draggable>().enabled = false;
        //RectTransform rect = zoomCard.GetComponent<RectTransform>();

        //rect.sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().sizeDelta.y);
        //rect.localScale = new Vector3(2, 2, 2);
    }


    public void OnHoverExit()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdDestroyZoomCard();
    }
}
