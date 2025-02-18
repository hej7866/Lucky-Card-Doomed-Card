using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : SingleTon<CardManager>
{
    [Header("카드")]
    [SerializeField] private Button card;
    Text cardNumber_txt;

    public int cardNumber;

    void Start()
    {
        cardNumber_txt = card.GetComponentInChildren<Text>();
    }

    public void DrawCard()
    {
        cardNumber = Random.Range(1,11);
        
        
        cardNumber_txt.text = cardNumber.ToString();
        Debug.Log(cardNumber);
    }
}
