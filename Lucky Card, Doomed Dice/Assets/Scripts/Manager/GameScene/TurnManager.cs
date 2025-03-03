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

    public int maxTurns = 15;
    private int currTurn = 1;
    public float thinkingTime = 30f;
    public float battleTime = 15f;
    private bool isTurnActive = false;
    private float turnTimer;

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

            // 턴을 모든 플레이어에게 동기화
            photonView.RPC("SyncTurn", RpcTarget.All, currTurn);

            // **① 준비 단계 (30초)**
            turnTimer = thinkingTime; 
            photonView.RPC("UpdatePhase", RpcTarget.All, "전략 페이즈", turnTimer);
            yield return StartCoroutine(ThinkingPhase());

            // **② 전투 단계 (15초)**
            turnTimer = battleTime;
            photonView.RPC("UpdatePhase", RpcTarget.All, "전투 페이즈", turnTimer);
            yield return StartCoroutine(BattlePhase());

            // **③ 턴 종료 & 다음 턴 시작**
            currTurn++;
        }

        Debug.Log("게임 종료!");
        photonView.RPC("GameOver", RpcTarget.All);
    }

    [PunRPC]
    private void SyncTurn(int turn)
    {
        currTurn = turn;
        Debug.Log($"턴 {currTurn} 동기화 완료!");
    }

    [PunRPC]
    private void GameOver()
    {
        Debug.Log("게임이 끝났습니다! 승패를 결정하세요.");
    }

    private IEnumerator ThinkingPhase()
    {
        Debug.Log("카드 선택 & 주사위 굴리기 단계");

        while (turnTimer > 0)
        {
            turnTimer -= Time.deltaTime;
            photonView.RPC("UpdateTurnTimer", RpcTarget.All, turnTimer);
            yield return null;
        }

        Debug.Log("생각하는 시간이 끝났습니다!");
    }

    private IEnumerator BattlePhase()
    {
        Debug.Log("전투 단계");

        while (turnTimer > 0)
        {
            turnTimer -= Time.deltaTime;
            photonView.RPC("UpdateTurnTimer", RpcTarget.All, turnTimer);
            yield return null;
        }

        Debug.Log("전투가 끝났습니다!");
    }

    [PunRPC]
    private void UpdatePhase(string phase, float time)
    {
        Phase.text = phase;
        turnTimer = time;
        turnTime.text = Mathf.FloorToInt(turnTimer).ToString();
    }

    [PunRPC]
    private void UpdateTurnTimer(float time)
    {
        turnTimer = time;
        turnTime.text = Mathf.FloorToInt(turnTimer).ToString();
    }
}
