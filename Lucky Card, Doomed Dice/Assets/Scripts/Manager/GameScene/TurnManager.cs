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

            photonView.RPC("SyncTurn", RpcTarget.All, currTurn);

            // **전략 페이즈 (30초) - 점수 숨기기**
            StartTurnTimer(thinkingTime, $"전략 {currTurn}페이즈", false);
            yield return new WaitUntil(() => !isTurnActive);

            // **전투 페이즈 (15초) - 점수 공개 & 데미지 계산**
            StartTurnTimer(battleTime, $"전투 {currTurn}페이즈", true);
            photonView.RPC("CalculateBattle", RpcTarget.MasterClient);
            yield return new WaitUntil(() => !isTurnActive);

            currTurn++;
            ResetSetting();
        }

        Debug.Log("게임 종료!");
        photonView.RPC("GameOver", RpcTarget.All);
    }

    void ResetSetting()
    {
        photonView.RPC("ResetGameState", RpcTarget.All);
    }

    [PunRPC]
    private void ResetGameState()
    {
        Debug.Log("게임 상태 초기화: 카드, 주사위, 공격 선택 초기화");

        isScoreSelected = false;
        StrategyManager.Instance.isAttackSelected = false;
        StrategyManager.Instance.isDefenceSelected = false;

        CardManager.Instance.ResetCard();
        DiceManager.Instance.ResetDice();

        // 초기화할 때 `null`을 사용하여 완전히 지움
        ExitGames.Client.Photon.Hashtable resetProps = new ExitGames.Client.Photon.Hashtable
        {
            { "Score", null },
            { "isAttackSelected", null },
            { "EnemyScore", null },
            { "EnemyAttack", null }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(resetProps);

        Debug.Log("CustomProperties 초기화 완료!");
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

        Debug.Log($"전투 시작 - 플레이어 점수: {playerScore}, 상대 점수: {enemyScore}");
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

        if (playerAttack && enemyAttack) // 공격 vs 공격
        {
            if (playerScore > enemyScore)
            {
                Debug.Log($"플레이어가 이겼음! 상대방에게 {playerScore * 2} 데미지");
                enemyPlayer.TakeDamage(playerScore * 2);
            }
            else if (enemyScore > playerScore)
            {
                Debug.Log($"상대방이 이겼음! 플레이어에게 {enemyScore * 2} 데미지");
                myPlayer.TakeDamage(enemyScore * 2);
            }
        }
        else if (playerAttack && !enemyAttack) // 공격 vs 수비
        {
            if (playerScore > enemyScore)
            {
                int damage = playerScore - enemyScore;
                Debug.Log($"플레이어 공격 성공! 상대방에게 {damage} 데미지");
                enemyPlayer.TakeDamage(damage);
            }
            else
            {
                Debug.Log($"상대방이 방어 성공! 데미지 없음");
            }
        }
        else if (!playerAttack && enemyAttack) // 수비 vs 공격 (반대 상황)
        {
            if (enemyScore > playerScore)
            {
                int damage = enemyScore - playerScore;
                Debug.Log($"상대방 공격 성공! 플레이어에게 {damage} 데미지");
                myPlayer.TakeDamage(damage);
            }
            else
            {
                Debug.Log($"플레이어가 방어 성공! 데미지 없음");
            }
        }
        else if (!playerAttack && !enemyAttack) // 수비 vs 수비 (쫄보죄)
        {
            Debug.Log($"쫄보죄! 플레이어: {playerScore} 데미지, 상대방: {enemyScore} 데미지");
            myPlayer.TakeDamage(playerScore);
            enemyPlayer.TakeDamage(enemyScore);
        }

        // 체력 동기화
        UIManager.Instance.photonView.RPC("SyncHealth", RpcTarget.All, myActorNumber, enemyActorNumber, myPlayer.playerHealth, enemyPlayer.playerHealth);
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
