using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardManager : SingleTon<CardManager>
{
    [Header("카드")]
    [SerializeField] private Button card;
    Text cardNumber_txt;

    public event Action<int> OnCardNumberChanged; 

    public int cardNumber;

    void Start()
    {
        cardNumber_txt = card.GetComponentInChildren<Text>();
    }

    public void DrawCard()
    {
        cardNumber = UnityEngine.Random.Range(1,11);
        
        OnCardNumberChanged?.Invoke(cardNumber);
        cardNumber_txt.text = cardNumber.ToString();
        Debug.Log(cardNumber);
    }
}
