using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool canAttack;
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;
    public GameObject placeholderItem;
    public GameObject placeholder = null;
    public bool isDragging = false;
    public GameObject enemyHighlighted;
    public bool isDraggable = true;
    private GameObject enemyHand;
    private GameObject enemytabletop;
    private GameObject tabletop;
    private GameObject hand;
    private int newSiblingIndex;


    public PlayerManager PlayerManager;

    private void Start()
    {
        enemyHand = UIGame.Instance.enemyHand;
        enemytabletop = UIGame.Instance.enemyTableTop;
        tabletop = UIGame.Instance.tableTop;
        hand = UIGame.Instance.hand;
        canAttack = false;

        enemyHand.transform.GetComponent<Image>().raycastTarget = false;
        enemytabletop.transform.GetComponent<Image>().raycastTarget = false;

        /*if (!hasAuthority)  //FIX. Can i drag opponents cards?
        {
            isDraggable = false;

            //Debug.Log("Den exw authority ara draggable: " + isDraggable);
        }*/
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        placeholder = Instantiate(PlayerManager.cardPrefab);
        placeholder.GetComponent<CardDisplay>().card = this.gameObject.GetComponent<CardDisplay>().card;//This needs to change cause if i move a buffed card it wont show properly
        //Debug.Log("THE ARTWORK IS: " + placeholder.GetComponent<CardDisplay>().card.artworkImage);
        placeholder.transform.SetParent(this.transform.parent, false);
        placeholder.GetComponent<CanvasGroup>().alpha = 0.33f;
        placeholder.GetComponent<CanvasGroup>().blocksRaycasts = false;
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.flexibleHeight = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        //Debug.Log("placeholder name in draggable is " + placeholder.name);
        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        //isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        //Debug.Log("OnDrag, isDraggable is " + isDraggable);
        // Debug.Log("OnDrag");
        this.transform.position = eventData.position;

        if (!PlayerManager.isMyTurn)
        {
            tabletop.transform.GetComponent<Image>().raycastTarget = false;
        }
        else
        {
            tabletop.transform.GetComponent<Image>().raycastTarget = true;
        }

        if (gameObject.transform.parent != placeholderParent && placeholderParent
            != enemyHand.transform && placeholderParent != enemytabletop.transform && PlayerManager.isMyTurn)
        {
            placeholder.transform.SetParent(placeholderParent);
        }

        newSiblingIndex = placeholderParent.childCount;
        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        //isDragging = false;

        //PlayerManager.CmdSpawnPreview(gameObject.GetComponent<CardDisplay>().card.id, placeholderParent);
        if (placeholder.transform.parent == tabletop.transform && GameManager.Instance.currentMana >= gameObject.GetComponent<CardDisplay>().card.cost)
        { //here i want to play the card
            if (tabletop.transform.childCount < GameManager.Instance.maxCardsOnBoard) //if board ISNT full then play it
            {
                isDraggable = false;
                GameManager.Instance.currentMana -= gameObject.GetComponent<CardDisplay>().card.cost;
                this.transform.SetParent(parentToReturnTo);
                this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
                PlayerManager.PlayCard(gameObject, placeholder.transform.parent, newSiblingIndex);
            }
            else //if board IS full, return it to my hand
            {
                this.transform.SetParent(hand.transform);
                this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
                Debug.Log("not enough space on board");
            }
        }
        else if (placeholder.transform.parent == hand.transform)
        { //here i move the card in my hand
            this.transform.SetParent(parentToReturnTo);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            PlayerManager.CmdPlayCard(gameObject, placeholder.transform.parent, newSiblingIndex);
        }
        else //here i return the card to my hand (THOUGHT: possibly because out of mana?)
        {
            this.transform.SetParent(hand.transform);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Debug.Log("not enough mana");
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeholder);
        //Debug.Log("end of drag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        if (GameManager.Instance.currentBattlePhase == GameManager.BattlePhase.None) //if you click anywhere without a selected
        {                                                                            //minion                   
            if (gameObject.transform.parent == tabletop.transform && PlayerManager.isMyTurn && canAttack)//and if that click is on my turn and
            {                                                                               //its on my minion then select it                
                //Color c = gameObject.GetComponent<Image>().color;
                ////c.a = 1;
                //c = new Color(255,150,0);
                //gameObject.GetComponent<Image>().color = c;

                gameObject.GetComponent<Image>().material = GameManager.Instance.blueFlame;

                GameManager.Instance.minionSelected = gameObject;
                GameManager.Instance.currentBattlePhase = GameManager.BattlePhase.Selected;

            }
            else if (gameObject.tag == "Player") //if i click on my heroe (anytime)
            {
                Debug.Log("My heroe says HI!");
            }
        }
        else if (GameManager.Instance.currentBattlePhase == GameManager.BattlePhase.Selected)
        {
            if ((gameObject.transform.parent == enemytabletop.transform || gameObject.tag == "EnemyPlayer") && PlayerManager.isMyTurn)
            {//if u have already selected your minion AND you select an enemy minion OR heroe, you attack here             
                Debug.Log("BOOOM");

                //Color c = GameManager.Instance.minionSelected.GetComponent<Image>().color;//de-highlight your minion
                //c.a = 0;
                //GameManager.Instance.minionSelected.GetComponent<Image>().color = c;
                GameManager.Instance.minionSelected.GetComponent<Image>().material = null;

                //c = enemyHighlighted.GetComponent<Image>().color; //de-highlight enemy minion
                //c.a = 0;
                //enemyHighlighted.GetComponent<Image>().color = c;
                enemyHighlighted.GetComponent<Image>().material = null;

                GameManager.Instance.minionSelected.GetComponent<Draggable>().canAttack = false;

                PlayerManager.CmdApplyDamage(GameManager.Instance.minionSelected, gameObject);

                enemyHighlighted = null;
                GameManager.Instance.minionSelected = null;
                GameManager.Instance.currentBattlePhase = GameManager.BattlePhase.None;
                Cursor.SetCursor(GameManager.Instance.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
            }
            else if (gameObject.transform.parent == tabletop.transform && PlayerManager.isMyTurn && GameManager.Instance.minionSelected != gameObject)
            {//if u have already a selected minion but you click another one (yours)
                if (canAttack) //if new minion CAN attack
                {
                    //Color c = GameManager.Instance.minionSelected.GetComponent<Image>().color; //de-highlight old minion (make it green if its ready to attack)
                    ////c.a = 0;
                    //c = Color.green;
                    //GameManager.Instance.minionSelected.GetComponent<Image>().color = c;
                    GameManager.Instance.minionSelected.GetComponent<Image>().material = GameManager.Instance.greenFlame;

                    //c = gameObject.GetComponent<Image>().color; //highlight new minion
                    ////c.a = 1;
                    //c = new Color(255, 150, 0);//orange
                    //gameObject.GetComponent<Image>().color = c;
                    gameObject.GetComponent<Image>().material = GameManager.Instance.blueFlame;

                    GameManager.Instance.minionSelected = gameObject; //select new minion
                }
                else//if new minion CANT attack
                {
                    //Color c = GameManager.Instance.minionSelected.GetComponent<Image>().color;//de-highlight already selected minion (even if new minion cant attack)
                    ////c.a = 0;
                    //c = Color.green;
                    //GameManager.Instance.minionSelected.GetComponent<Image>().color = c;
                    GameManager.Instance.minionSelected.GetComponent<Image>().material = GameManager.Instance.greenFlame;

                    GameManager.Instance.minionSelected = null;
                    GameManager.Instance.currentBattlePhase = GameManager.BattlePhase.None;
                }

            }
            else
            {//if i have something selected and i click my hand de-select it
                //Color c = GameManager.Instance.minionSelected.GetComponent<Image>().color;
                ////c.a = 0;
                //c = Color.green;
                //GameManager.Instance.minionSelected.GetComponent<Image>().color = c;
                GameManager.Instance.minionSelected.GetComponent<Image>().material = GameManager.Instance.greenFlame;

                GameManager.Instance.minionSelected = null;
                GameManager.Instance.currentBattlePhase = GameManager.BattlePhase.None;

            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((gameObject.transform.parent == enemytabletop.transform || gameObject.tag == "EnemyPlayer") && GameManager.Instance.currentBattlePhase == GameManager.BattlePhase.Selected)
        {//already selected minion and hovering over enemy monsters OR heroe, to attack
            enemyHighlighted = gameObject;
            //Color c = enemyHighlighted.GetComponent<Image>().color; //highlight enemy minion OR heroe
            //c.a = 1;
            //c = Color.red;
            //enemyHighlighted.GetComponent<Image>().color = c;
            enemyHighlighted.GetComponent<Image>().material = GameManager.Instance.redFlame;

            Cursor.SetCursor(GameManager.Instance.attackCursor, Vector2.zero, CursorMode.ForceSoftware);
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject == enemyHighlighted)
        {
            //Color c = enemyHighlighted.GetComponent<Image>().color; //de-highlight enemy minion OR heroe
            //c.a = 0;
            //enemyHighlighted.GetComponent<Image>().color = c;
            enemyHighlighted.GetComponent<Image>().material = null;

            enemyHighlighted = null;
            Cursor.SetCursor(GameManager.Instance.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}


