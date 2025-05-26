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

    public int card01DrawCount = 0;
    public int card02DrawCount = 0;
    public int card01Number;
    public int card02Number;

    public event Action<int> OnCard01NumberChanged; 
    public event Action<int> OnCard02NumberChanged; 
    public event Action<int, int> OnCard01DrawCountChanged;
    public event Action<int, int> OnCard02DrawCountChanged;


    void Start()
    {
        card01Number_img = card01.GetComponent<Image>();
        card02Number_img = card02.GetComponent<Image>();
    }

    public void DrawCard01()
    {
        card01DrawCount++;
        DrawCard(card01DrawCount,1);
        card01Number_img.sprite = cardImgs[card01Number];
    }

    public void DrawCard02()
    {
        card02DrawCount++;
        DrawCard(card02DrawCount,2);
        card02Number_img.sprite = cardImgs[card02Number];
    }

    public void DrawCard(int drawCount, int cardNum)
    {
        if (TurnManager.Instance.CurrentPhase == TurnManager.TurnPhase.Battle)
        {
            LogManager.Instance.AddLog("전투 페이즈에는 카드를 뽑을 수 없습니다!");
            return;
        }

        if (drawCount > 3)
        {
            LogManager.Instance.AddLog("카드를 더이상 뽑을 수 없습니다.");
            return;
        }

        if (TurnManager.Instance.isScoreSelected)
        {
            LogManager.Instance.AddLog("이미 스코어를 결정하셨습니다.");
            return;
        }

        if (cardNum == 1)
        {
            card01Number = UnityEngine.Random.Range(1, 14);
            OnCard01DrawCountChanged?.Invoke(drawCount, 1);
            OnCard01NumberChanged?.Invoke(card01Number);

            LogManager.Instance.AddLog($"카드를 뽑아 숫자{card01Number}가 나왔습니다!!");
        }
        else if (cardNum == 2)
        {
            card02Number = UnityEngine.Random.Range(1, 14);
            OnCard02DrawCountChanged?.Invoke(drawCount, 2);
            OnCard02NumberChanged?.Invoke(card02Number);
            LogManager.Instance.AddLog($"카드를 뽑아 숫자{card02Number}가 나왔습니다!!");
        }
    }

    public void ResetCard()
    {
        card01DrawCount = 0;
        card02DrawCount = 0;
        card01Number = 0;
        card02Number = 0;

        OnCard01DrawCountChanged?.Invoke(card01DrawCount, 1);
        OnCard02DrawCountChanged?.Invoke(card02DrawCount, 2);

        OnCard01NumberChanged?.Invoke(card01Number);
        OnCard02NumberChanged?.Invoke(card02Number);

        card01Number_img.sprite = cardImgs[0];
        card02Number_img.sprite = cardImgs[0];
    }
}
