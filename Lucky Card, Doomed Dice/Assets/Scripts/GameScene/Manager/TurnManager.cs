using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Instance { get; private set; }
    public enum TurnPhase { None, Strategy, Battle }

    [SerializeField] private Text Phase;
    [SerializeField] private Text turnTime;
    [SerializeField] private Text turnText;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyHealthText;

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.None;


    [SerializeField] private int maxTurns = 10;
    public int currTurn = 1;
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

    private void Start()
    {
        turnText.text = "1 / 10";   
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
            LogManager.Instance.AddRPCLog($"턴 {currTurn} 시작!");

            photonView.RPC("SyncTurn", RpcTarget.All, currTurn);

            // **전략 페이즈 (30초) - 점수 숨기기**
            StartTurnTimer(thinkingTime, TurnPhase.Strategy, $"전략 {currTurn}페이즈", false);
            yield return new WaitUntil(() => !isTurnActive);

            // **전투 페이즈 (15초) - 점수 공개 & 데미지 계산**
            StartTurnTimer(battleTime, TurnPhase.Battle, $"전투 {currTurn}페이즈", true);

            yield return new WaitForSeconds(2f);
            BattleManager.Instance.CalculateBattle();

            yield return new WaitUntil(() => !isTurnActive);

            currTurn++;
            photonView.RPC("UpdateTurnText", RpcTarget.All, currTurn);
            ResetSetting();

            // ✅ 게임 종료 조건 확인 후 종료
            if (CheckGameOverCondition())
            {
                GameManager.Instance.EndGame();
                yield break;  // 루프 탈출
            }
        }

        GameManager.Instance.EndGame();
        photonView.RPC("GameOver", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateTurnText(int currTurn)
    {
        turnText.text = $"{currTurn} / 10";
    }


    private bool CheckGameOverCondition() // 게임 종료조건을 만족했는지 체크하는 로직
    {
        int alivePlayers = 0;

        foreach (var player in PlayerManager.Players.Values)
        {
            if (player.playerHealth > 0)
            {
                alivePlayers++;
            }
        }

        // 살아있는 플레이어가 1명 이하이면 게임 종료
        return alivePlayers <= 1;
    }

    void ResetSetting()
    {
        photonView.RPC("ResetTurnState", RpcTarget.All);
    }

    [PunRPC]
    private void ResetTurnState()
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



    private void StartTurnTimer(float duration, TurnPhase turnPhase, string phase, bool showScore)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turnEndTime = PhotonNetwork.Time + duration;
            photonView.RPC("SyncTurnTimer", RpcTarget.All, turnPhase, turnEndTime, phase, showScore);
        }
    }

    [PunRPC]
    private void SyncTurn(int turn)
    {
        currTurn = turn;
        Debug.Log($"턴 {currTurn} 동기화 완료!");
    }

    [PunRPC]
    private void SyncTurnTimer(TurnPhase turnPhase, double endTime, string phase, bool showScore)
    {
        turnEndTime = endTime;
        isTurnActive = true;
        Phase.text = phase;
        UIManager.Instance.ToggleScoreVisibility(showScore);

        // 현재 페이즈 설정
        CurrentPhase = turnPhase;
        Debug.Log($"[TurnManager] CurrentPhase = {CurrentPhase}");
    }

    public void ResetGameState()
    {
        StopAllCoroutines();               // 코루틴 중지
        CurrentPhase = TurnPhase.None;
        currTurn = 1;
        isTurnActive = false;
        isScoreSelected = false;

        turnTime.text = "";
        Phase.text = "";
        turnText.text = "1 / 10";          // 첫 턴 기준

        Debug.Log("TurnManager 초기화 완료");
    }



    [PunRPC]
    private void GameOver()
    {
        LogManager.Instance.AddRPCLog("게임이 끝났습니다! 승패를 결정하세요.");
    }

}
