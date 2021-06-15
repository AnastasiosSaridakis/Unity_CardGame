using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDefault : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parent;
    private GameObject cloneCard;
    public int cardID, availability = 3, cost;
    public string cardName;
    public bool thingsToUpdate = false;

    private void Start()
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cloneCard = Instantiate(this.gameObject, this.gameObject.transform.parent.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cloneCard == null)
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        cloneCard.transform.position = new Vector3(ray.origin.x, ray.origin.y, 1);
        
        /*Vector2 pos;      //THIS IS AN ALTERNATIVE OF THE TWO ROWS ABOVE
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        cloneCard.transform.position = canvas.transform.TransformPoint(pos);*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cloneCard == null)
            return;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        
        pointer.position = eventData.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        foreach (RaycastResult result in results)
        {
            if(result.gameObject.tag == "CurrentDeckPanel")//spawn a card with the same id as a simpleCard
            {
                GameObject go = Instantiate(UIManager.Instance.simpleCard,UIManager.Instance.currentDeckPanel.transform);
                go.GetComponent<SimpleCard>().SetStats(cardID, this.GetComponent<CardDisplay>().nameText.text, int.Parse(this.GetComponent<CardDisplay>().manaText.text));
                DeckManager.Instance.OrderCurrentDeck();
                DeckManager.Instance.currentCardsInDeck++;
                DeckManager.Instance.RefreshText();
                availability--;
                thingsToUpdate = true;
            }
        }
        Destroy(cloneCard);
    }

    void Update()
    {
        if (thingsToUpdate)
        {
            if (availability > 0)
            {
                gameObject.GetComponent<CanvasGroup>().alpha = 1;
                gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                gameObject.GetComponent<CanvasGroup>().alpha = 0.25f;
                gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            thingsToUpdate = false;
        }
       
    }

    public void SetStats(Card card)
    {
        this.GetComponent<CardDisplay>().CopyStats(card);
    }
    
    public void SetAvailability(int availalble)
    {
        availability = availalble;

        if (availability > 0)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0.25f;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void CheckForCopies()
    {
        for (int i = 0; i < UIManager.Instance.currentDeckPanel.transform.childCount; i++)
        {
            if (UIManager.Instance.currentDeckPanel.transform.GetChild(i).GetComponent<SimpleCard>().cardId == cardID)
            {
                availability--;
                thingsToUpdate = true;
            }
        }
        
    }
}
