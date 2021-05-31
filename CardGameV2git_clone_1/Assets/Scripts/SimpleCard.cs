using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SimpleCard : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler 
{
    public int cardId,cost;
    public string cName;
    public TextMeshProUGUI cardName, cardMana;
    GameObject zoomCard;
    Card originalCard;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStats(int id, string name,int mana)
    {
        cardId = id;
        cardName.SetText(name);
        cardMana.SetText(mana.ToString());
        cost = mana;
        cName = name;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DeckManager.Instance.isDragging)
            return;

        if (zoomCard != null)
            Destroy(zoomCard);

        Vector3 cardPos = transform.position + Vector3.right * GetComponent<RectTransform>().rect.width;
        zoomCard = Instantiate(UIManager.Instance.defaultCard, cardPos, Quaternion.identity);
        zoomCard.transform.SetParent(transform.parent.parent.parent);
        zoomCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
        foreach (Card card in CardDatabase.Instance.cardList)
        {
            if (card.id == cardId)
            {
                zoomCard.GetComponent<CardDisplay>().CopyStats(card);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DeckManager.Instance.isDragging)
            return;

        Destroy(zoomCard);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (zoomCard != null)
            Destroy(zoomCard);

        transform.SetParent(UIManager.Instance.currentDeckPanel.transform.parent.parent);
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        DeckManager.Instance.isDragging = true;
        DeckManager.Instance.currentCardsInDeck--;
        DeckManager.Instance.RefreshText();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        pointer.position = eventData.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        bool toBeDestroyed = true;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "CurrentDeckPanel")//drag the card anywhere except the current deck panel
            {
                toBeDestroyed = false;
            }
        }

        if(toBeDestroyed)
        {
            for (int i = 0; i < UIManager.Instance.cardDatabasePanel.transform.childCount; i++)
            {
                if (UIManager.Instance.cardDatabasePanel.transform.GetChild(i).gameObject.GetComponent<CardDefault>().cardID == cardId)
                {
                    UIManager.Instance.cardDatabasePanel.transform.GetChild(i).gameObject.GetComponent<CardDefault>().availability++;
                    UIManager.Instance.cardDatabasePanel.transform.GetChild(i).gameObject.GetComponent<CardDefault>().thingsToUpdate = true;
                }
            }
            Destroy(gameObject);
        }
        else
        {
            transform.SetParent(UIManager.Instance.currentDeckPanel.transform);
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            DeckManager.Instance.currentCardsInDeck++;
            DeckManager.Instance.RefreshText();
        }
        DeckManager.Instance.OrderCurrentDeck();
        DeckManager.Instance.isDragging = false;
    }

}
