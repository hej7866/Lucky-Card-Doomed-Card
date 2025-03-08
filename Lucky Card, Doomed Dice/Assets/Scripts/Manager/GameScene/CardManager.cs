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

    public int drawCount = 0;
    public int cardNumber;

    public event Action<int> OnCardNumberChanged; 


    void Start()
    {
        cardNumber_txt = card.GetComponentInChildren<Text>();
    }

    public void DrawCard()
    {
        if(drawCount >= 3)
        {
            Debug.Log("카드를 더이상 뽑을 수 없습니다.");
            return;
        }

        if(TurnManager.Instance.isScoreSelected)
        {
            Debug.Log("이미 스코어를 결정하셨습니다.");
            return;
        }

        drawCount++; // 카드를 뽑은 횟수

        cardNumber = UnityEngine.Random.Range(1,11);
        
        OnCardNumberChanged?.Invoke(cardNumber);
        cardNumber_txt.text = cardNumber.ToString();
        Debug.Log(cardNumber);
    }

    public void ResetCard()
    {
        drawCount = 0;
        cardNumber = 0;

        OnCardNumberChanged?.Invoke(cardNumber);
        cardNumber_txt.text = cardNumber.ToString();
    }
}
