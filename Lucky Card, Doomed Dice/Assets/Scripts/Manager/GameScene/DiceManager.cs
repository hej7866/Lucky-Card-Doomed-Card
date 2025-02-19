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

    public event Action<int> OnDiceNumberChanged; 

    public int diceNumber;

    void Start()
    {
        diceNumber_txt = dice.GetComponentInChildren<Text>();
    }

    public void RollDice()
    {
        diceNumber = UnityEngine.Random.Range(1,7);
        
        OnDiceNumberChanged?.Invoke(diceNumber); 
        diceNumber_txt.text = diceNumber.ToString();
        Debug.Log(diceNumber);
    }
}
