using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DiceManager : SingleTon<DiceManager>
{
    [Header("주사위")]
    [SerializeField] private Sprite[] diceImgs;
    [SerializeField] private Button dice;
    Image diceNumber_img;


    public int rollCount = 0;
    public int diceNumber = 1;

    public event Action<int> OnDiceNumberChanged; 
    public event Action<int> OnRollCountChanged; 


    void Start()
    {
        diceNumber_img = dice.GetComponent<Image>();
    }

    public void RollDice()
    {
        if (TurnManager.Instance.CurrentPhase == TurnManager.TurnPhase.Battle)
        {
            LogManager.Instance.AddLog("전투 페이즈에는 주사위를 굴릴 수 없습니다!");
            return;
        }

        if(rollCount >= 3)
        {
            LogManager.Instance.AddLog("주사위를 더이상 던질 수 없습니다.");
            return;
        }

        if(TurnManager.Instance.isScoreSelected)
        {
            LogManager.Instance.AddLog("이미 스코어를 결정하셨습니다.");
            return;
        }

        rollCount++; // 주사위를 던진 횟수

        diceNumber = UnityEngine.Random.Range(1,7);
        
        OnRollCountChanged?.Invoke(rollCount); 
        OnDiceNumberChanged?.Invoke(diceNumber); 

        diceNumber_img.sprite = diceImgs[diceNumber];
        LogManager.Instance.AddLog($"주사위를 굴려 숫자{diceNumber}가 나왔습니다!!");
    }

    public void ResetDice()
    {
        rollCount = 0;
        diceNumber = 1;

        OnRollCountChanged?.Invoke(rollCount); 
        OnDiceNumberChanged?.Invoke(diceNumber);
        diceNumber_img.sprite = diceImgs[0];
    }
}
