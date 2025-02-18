using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : SingleTon<DiceManager>
{
    [Header("주사위")]
    [SerializeField] private Button dice;
    Text diceNumber_txt;

    public int diceNumber;

    void Start()
    {
        diceNumber_txt = dice.GetComponentInChildren<Text>();
    }

    public void RollDice()
    {
        diceNumber = Random.Range(1,7);
        
        
        diceNumber_txt.text = diceNumber.ToString();
        Debug.Log(diceNumber);
    }
}
