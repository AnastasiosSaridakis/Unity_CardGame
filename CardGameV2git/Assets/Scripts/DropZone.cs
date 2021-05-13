using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class DropZone : NetworkBehaviour, IDropHandler, IPointerEnterHandler,IPointerExitHandler
{
    private GameObject enemyHand;
    private GameObject enemytabletop;

    public void Start()
    {
        enemyHand = GameObject.FindWithTag("EnemyHand");
        enemytabletop = GameObject.FindWithTag("EnemyTabletop");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            //Debug.Log("ENTER this.tag = " + this.tag);
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            //Debug.Log("EXIT this.tag = " + this.tag);
            d.placeholderParent = d.parentToReturnTo;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag.name + " was dropped on "+gameObject.name);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            if(d.placeholderParent != (enemyHand.transform) || d.placeholderParent != (enemytabletop.transform))
            {
                d.parentToReturnTo = this.transform;
            }
            else
            {
                //Debug.Log("alliws gurna pisw xwris na kaneis tpt");
            }
        }
    }

}
