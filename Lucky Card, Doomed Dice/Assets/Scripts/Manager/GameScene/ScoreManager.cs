using System;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ScoreManager : SingleTon<ScoreManager>
{
    [SerializeField] GameObject attackBtn;
    [SerializeField] GameObject defenceBtn;

    public event Action<int> OnScoreChanged; // ✅ 점수 변경 이벤트

    private void Start()
    {
        CardManager.Instance.OnCardNumberChanged += _ => CalculateScore();
        DiceManager.Instance.OnDiceNumberChanged += _ => CalculateScore();
    }

    public int card
    {
        get
        {
            return CardManager.Instance.cardNumber;
        } 
    }
    
    public int dice
    {
        get
        {
            return DiceManager.Instance.diceNumber;
        }
    }

    private int currentScore = 0; // ✅ 현재 점수 저장

    public void CalculateScore()
    {
        int newScore = CardManager.Instance.cardNumber * DiceManager.Instance.diceNumber;
        
        if (newScore != currentScore)
        {
            currentScore = newScore;
            OnScoreChanged?.Invoke(currentScore); // ✅ UI 업데이트 이벤트 호출

            // ✅ Photon에 점수 업데이트
            Hashtable props = new Hashtable { { "Score", currentScore } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
    

    public void SelectScore()
    {
        TurnManager.Instance.selectScore = true;
        attackBtn.SetActive(true);
        defenceBtn.SetActive(true);
    }
}
