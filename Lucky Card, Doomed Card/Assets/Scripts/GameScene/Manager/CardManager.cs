using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardManager : SingleTon<CardManager>
{
    [Header("카드")]
    [SerializeField] private Sprite[] cardImgs;
    [SerializeField] private Button card01;
    [SerializeField] private Button card02;
    Image card01Number_img;
    Image card02Number_img;

    public int drawCount = 0;
    public int cardNumber;

    public event Action<int> OnCardNumberChanged; 
    public event Action<int> OnDrawCountChanged;


    void Start()
    {
        card01Number_img = card01.GetComponent<Image>();
        card02Number_img = card02.GetComponent<Image>();
    }

    public void DrawCard01()
    {
        DrawCard();
        card01Number_img.sprite = cardImgs[cardNumber];
    }

    public void DrawCard02()
    {
        DrawCard();
        card02Number_img.sprite = cardImgs[cardNumber];
    }

    public void DrawCard()
    {
        if (TurnManager.Instance.CurrentPhase == TurnManager.TurnPhase.Battle)
        {
            LogManager.Instance.AddLog("전투 페이즈에는 카드를 뽑을 수 없습니다!");
            return;
        }

        if (drawCount >= 3)
        {
            LogManager.Instance.AddLog("카드를 더이상 뽑을 수 없습니다.");
            return;
        }


        if (TurnManager.Instance.isScoreSelected)
        {
            LogManager.Instance.AddLog("이미 스코어를 결정하셨습니다.");
            return;
        }

        drawCount++; // 카드를 뽑은 횟수

        cardNumber = UnityEngine.Random.Range(1, 14);

        OnDrawCountChanged?.Invoke(drawCount);
        OnCardNumberChanged?.Invoke(cardNumber);
        
        LogManager.Instance.AddLog($"카드를 뽑아 숫자{cardNumber}가 나왔습니다!!");
    }

    public void ResetCard()
    {
        drawCount = 0;
        cardNumber = 0;

        OnDrawCountChanged?.Invoke(drawCount);
        OnCardNumberChanged?.Invoke(cardNumber);
        card01Number_img.sprite = cardImgs[0];
        card02Number_img.sprite = cardImgs[0];
    }
}
