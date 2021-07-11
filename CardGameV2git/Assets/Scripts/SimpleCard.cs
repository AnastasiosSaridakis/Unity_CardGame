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
    Card originalCard;
    private Canvas canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
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

        if (UIManager.Instance.zoomCard != null)
            Destroy(UIManager.Instance.zoomCard);

       
        /*Vector3 cardPos = transform.position + Vector3.right * GetComponent<RectTransform>().rect.width;
        cardPos = cardPos + Vector3.down * Screen.height / 2;
        //RectTransformUtility.WorldToScreenPoint(Camera.main, gameObject.transform.position)*/

        Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 pos2 = WorldToScreenSpace(transform.position, Camera.main, transform.parent.parent.GetComponent<RectTransform>());
        pos2 = pos2 + Vector3.right * GetComponent<RectTransform>().rect.width;
        
        UIManager.Instance.zoomCard = Instantiate(UIManager.Instance.defaultCard, pos2, Quaternion.identity);
        UIManager.Instance.zoomCard.transform.SetParent(transform.parent.parent.parent,false);
        UIManager.Instance.zoomCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
        foreach (Card card in CardDatabase.Instance.cardList)
        {
            if (card.id == cardId)
            {
                UIManager.Instance.zoomCard.GetComponent<CardDisplay>().CopyStats(card);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DeckManager.Instance.isDragging)
            return;

        Destroy(UIManager.Instance.zoomCard);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right ||eventData.button == PointerEventData.InputButton.Middle )
            return;
        if (UIManager.Instance.zoomCard != null)
            Destroy(UIManager.Instance.zoomCard);
        
        transform.SetParent(UIManager.Instance.currentDeckPanel.transform.parent.parent);
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        DeckManager.Instance.isDragging = true;
        DeckManager.Instance.currentCardsInDeck--;
        DeckManager.Instance.RefreshText();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right ||eventData.button == PointerEventData.InputButton.Middle )
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.position = new Vector3(ray.origin.x, ray.origin.y, 1);
        
        /*Vector2 pos; /THIS IS AN ALTERNATIVE OF THE TWO ROWS ABOVE
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);*/
        //transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right ||eventData.button == PointerEventData.InputButton.Middle )
            return;
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
    public static Vector3 WorldToScreenSpace(Vector3 worldPos, Camera cam, RectTransform area)
    {
        Vector3 screenPoint = cam.WorldToScreenPoint(worldPos);
        screenPoint.z = 0;
 
        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam, out screenPos))
        {
            return screenPos;
        }
 
        return screenPoint;
    }

}
