using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private Text Phase;
    [SerializeField] private Text turnTime;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyHealthText;

    public int maxTurns = 15;
    private int currTurn = 1;
    public float thinkingTime = 30f;
    public float battleTime = 15f;
    private bool isTurnActive = false;
    private double turnEndTime;

    public bool isScoreSelected = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TurnStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(TurnRoutine());
        }
    }

    private IEnumerator TurnRoutine()
    {
        while (currTurn <= maxTurns)
        {
            isTurnActive = true;
            Debug.Log($"턴 {currTurn} 시작!");

            // 불 변수들 초기화
            isScoreSelected = false;
            StrategyManager.Instance.isAttackSelected = false;
            StrategyManager.Instance.isDefenceSelected = false;

            photonView.RPC("SyncTurn", RpcTarget.All, currTurn);

            // **① 전략 페이즈 (30초) - 점수 숨기기**
            StartTurnTimer(thinkingTime, "전략 페이즈", false);
            yield return new WaitUntil(() => !isTurnActive);

            // **② 전투 페이즈 (15초) - 점수 공개 & 데미지 계산**
            StartTurnTimer(battleTime, "전투 페이즈", true);
            photonView.RPC("CalculateBattle", RpcTarget.MasterClient);
            yield return new WaitUntil(() => !isTurnActive);


            currTurn++;
        }

        Debug.Log("게임 종료!");
        photonView.RPC("GameOver", RpcTarget.All);
    }

    private void StartTurnTimer(float duration, string phase, bool showScore)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turnEndTime = PhotonNetwork.Time + duration;
            photonView.RPC("SyncTurnTimer", RpcTarget.All, turnEndTime, phase, showScore);
        }
    }

    [PunRPC]
    private void SyncTurn(int turn)
    {
        currTurn = turn;
        Debug.Log($"턴 {currTurn} 동기화 완료!");
    }

    [PunRPC]
    private void SyncTurnTimer(double endTime, string phase, bool showScore)
    {
        turnEndTime = endTime;
        isTurnActive = true;
        Phase.text = phase;
        UIManager.Instance.ToggleScoreVisibility(showScore);
    }

    [PunRPC]
    private void CalculateBattle()
    {
        int playerScore = StrategyManager.Instance.card * StrategyManager.Instance.dice;
        bool playerAttack = StrategyManager.Instance.isAttackSelected;

        // 상대방의 정보 가져오기
        object enemyScoreObj, enemyAttackObj;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("EnemyScore", out enemyScoreObj) ||
            !PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("EnemyAttack", out enemyAttackObj))
        {
            Debug.LogError("EnemyScore 또는 EnemyAttack 값을 찾을 수 없습니다! 전투 계산 중단");
            return;
        }

        int enemyScore = (int)enemyScoreObj;
        bool enemyAttack = (bool)enemyAttackObj;

        Debug.Log($"[⚔️ 전투 시작] 플레이어 점수: {playerScore}, 상대 점수: {enemyScore}");
        Debug.Log($"플레이어 공격 여부: {playerAttack}, 상대 공격 여부: {enemyAttack}");

        // 플레이어와 상대방의 ActorNumber 가져오기
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int enemyActorNumber = PhotonNetwork.PlayerListOthers[0].ActorNumber;

        PlayerManager myPlayer = PlayerManager.GetPlayer(myActorNumber);
        PlayerManager enemyPlayer = PlayerManager.GetPlayer(enemyActorNumber);

        if (myPlayer == null || enemyPlayer == null)
        {
            Debug.LogError("PlayerManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        // 전투 로직
        if (playerAttack && enemyAttack)
        {
            if (playerScore > enemyScore)
            {
                enemyPlayer.TakeDamage(playerScore * 2);
            }
            else if (enemyScore > playerScore)
            {
                myPlayer.TakeDamage(enemyScore * 2);
            }
        }
        else if (playerAttack && !enemyAttack)
        {
            if (playerScore > enemyScore)
            {
                enemyPlayer.TakeDamage(playerScore - enemyScore);
            }
        }
        else if (!playerAttack && !enemyAttack)
        {
            myPlayer.TakeDamage(playerScore);
            enemyPlayer.TakeDamage(enemyScore);
        }

        // ✅ 체력 동기화
        photonView.RPC("SyncHealth", RpcTarget.All, myActorNumber, enemyActorNumber, myPlayer.playerHealth, enemyPlayer.playerHealth);
    }


    [PunRPC]
    private void SyncHealth(int myActorNumber,int enemyActorNumber, int newPlayerHealth, int newEnemyHealth)
    {
        PlayerManager myPlayer = PlayerManager.GetPlayer(myActorNumber);
        PlayerManager enemyPlayer = PlayerManager.GetPlayer(enemyActorNumber);

        if (myPlayer != null)
        {
            myPlayer.playerHealth = newPlayerHealth;
        }

        if (enemyPlayer != null)
        {
            enemyPlayer.playerHealth = newEnemyHealth;
        }

        playerHealthText.text = $"HP: {newPlayerHealth}";
        enemyHealthText.text = $"HP: {newEnemyHealth}";
    }


    [PunRPC]
    private void GameOver()
    {
        Debug.Log("게임이 끝났습니다! 승패를 결정하세요.");
    }

    private void Update()
    {
        if (isTurnActive)
        {
            double timeRemaining = turnEndTime - PhotonNetwork.Time;
            turnTime.text = Mathf.FloorToInt((float)timeRemaining).ToString();
            
            if (timeRemaining <= 0)
            {
                isTurnActive = false;
            }
        }
    }
}
