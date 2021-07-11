using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Rendering;

public class CardZoom : MonoBehaviour
{
    public PlayerManager PlayerManager;
    
    public GameObject hand;
    public GameObject zoomCard;

    public void Awake()
    {
        hand = GameObject.FindWithTag("Hand");
    }
    

    public void OnHoverEnter()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        if (PlayerManager == null)
            return;
        if (UIGame.Instance.zoomCard!=null)
            Destroy(UIGame.Instance.zoomCard);
        
        if (gameObject.transform.position.y+this.GetComponent<RectTransform>().sizeDelta.y * 2>Screen.height)
        {
            float i = Screen.height - this.GetComponent<RectTransform>().sizeDelta.y;
            UIGame.Instance.zoomCard = Instantiate(PlayerManager.cardPrefab, new Vector3(gameObject.transform.position.x, i), Quaternion.identity, gameObject.transform);
        }
        else
        {
            float i = this.GetComponent<RectTransform>().sizeDelta.y + this.GetComponent<RectTransform>().sizeDelta.y/2;
            var position = gameObject.transform.position;
            UIGame.Instance.zoomCard = Instantiate(PlayerManager.cardPrefab, new Vector3(position.x, position.y + i + 10), Quaternion.identity, gameObject.transform);
        }
        UIGame.Instance.zoomCard.GetComponent<CardDisplay>().CopyStats(gameObject.GetComponent<CardDisplay>().card);
        UIGame.Instance.zoomCard.GetComponent<CardDisplay>().setHealth(gameObject.GetComponent<CardDisplay>().GetHealth());
        UIGame.Instance.zoomCard.GetComponent<SortingGroup>().sortingOrder = 1;
        UIGame.Instance.zoomCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
        UIGame.Instance.zoomCard.GetComponent<Draggable>().enabled = false;
        RectTransform rect = UIGame.Instance.zoomCard.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(UIGame.Instance.zoomCard.GetComponent<RectTransform>().sizeDelta.x, UIGame.Instance.zoomCard.GetComponent<RectTransform>().sizeDelta.y);
        rect.localScale = new Vector3(2, 2, 2);

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
        if (UIGame.Instance.zoomCard!=null)
            Destroy(UIGame.Instance.zoomCard);
    }
}
