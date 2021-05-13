using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardDatabase : MonoBehaviour
{
    private static CardDatabase _instance;

    public static CardDatabase Instance { get { return _instance; } }

    public List<Card> cardList = new List<Card>();
    public List<Card> orderedCardList = new List<Card>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }

    private void Start()
    {
        orderedCardList = cardList.OrderBy(go => go.cost).ThenBy(go => go.cardname).ToList();
    }
}




