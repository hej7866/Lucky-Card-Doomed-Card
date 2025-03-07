using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; 
using ExitGames.Client.Photon;


public class StrategyManager : MonoBehaviourPunCallbacks
{
    public static StrategyManager Instance { get; private set; }

    [SerializeField] GameObject attackBtn;
    [SerializeField] GameObject defenceBtn;

    public bool isAttackSelected = false;
    public bool isDefenceSelected = false;

    public event Action<int> OnScoreChanged; // ✅ 점수 변경 이벤트

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

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
        TurnManager.Instance.isScoreSelected = true;
        attackBtn.SetActive(true);
        defenceBtn.SetActive(true);
    }


    private System.Collections.IEnumerator WaitForEnemyScoreAndSet()
    {
        Debug.Log("상대방 점수를 기다리는 중...");

        // 상대방이 점수를 선택할 때까지 기다림
        yield return new WaitUntil(() =>
            PhotonNetwork.PlayerListOthers.Length > 0 &&
            PhotonNetwork.PlayerListOthers[0].CustomProperties.ContainsKey("Score") &&
            PhotonNetwork.PlayerListOthers[0].CustomProperties.ContainsKey("isAttackSelected")
        );

        // 상대방의 점수를 가져와서 `EnemyScore`로 저장
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            object enemyScoreObj, enemyAttackObj;
            if (player.CustomProperties.TryGetValue("Score", out enemyScoreObj) &&
                player.CustomProperties.TryGetValue("isAttackSelected", out enemyAttackObj))
            {
                Hashtable roomProps = new Hashtable
                {
                    { "EnemyScore", enemyScoreObj },
                    { "EnemyAttack", enemyAttackObj }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

                Debug.Log($"상대방 점수 저장 완료! EnemyScore: {enemyScoreObj}, EnemyAttack: {enemyAttackObj}");
            }
        }
    }


    public void Attack()
    {
        isAttackSelected = true;
        isDefenceSelected = false;
        attackBtn.SetActive(false);
        defenceBtn.SetActive(false);
        
        SaveProps(); // ✅ 공격/수비 선택 후 네트워크에 저장
    }

    public void Defence()
    {
        isAttackSelected = false;
        isDefenceSelected = true;
        attackBtn.SetActive(false);
        defenceBtn.SetActive(false);

        SaveProps(); // ✅ 공격/수비 선택 후 네트워크에 저장
    }

    private void SaveProps()
    {
        Hashtable props = new Hashtable
        {
            { "Score", card * dice },
            { "isAttackSelected", isAttackSelected }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log($"공격/수비 선택 완료! 네트워크에 저장: Score = {card * dice}, isAttackSelected = {isAttackSelected}");

        // ✅ 마스터 클라이언트가 상대방 점수를 가져와서 `EnemyScore`를 설정
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitForEnemyScoreAndSet());
        }
    }

}
