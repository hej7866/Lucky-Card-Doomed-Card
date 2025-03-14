using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DiceManager : SingleTon<DiceManager>
{
    [Header("주사위")]
    [SerializeField] private Button dice;
    Text diceNumber_txt;

    public int rollCount = 0;
    public int diceNumber;

    public event Action<int> OnDiceNumberChanged; 


    void Start()
    {
        diceNumber_txt = dice.GetComponentInChildren<Text>();
    }

    public void RollDice()
    {
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
        
        OnDiceNumberChanged?.Invoke(diceNumber); 
        diceNumber_txt.text = diceNumber.ToString();
        LogManager.Instance.AddLog($"주사위를 굴려 숫자{diceNumber}가 나왔습니다!!");
    }

    public void ResetDice()
    {
        rollCount = 0;
        diceNumber = 0;

        OnDiceNumberChanged?.Invoke(diceNumber);
        diceNumber_txt.text = diceNumber.ToString();
    }
}
