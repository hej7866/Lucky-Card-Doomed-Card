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
    private double turnEndTime;

    public bool selectScore = false;

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

            selectScore = false;

            photonView.RPC("SyncTurn", RpcTarget.All, currTurn);

            // **① 전략 페이즈 (30초) - 점수 숨기기**
            StartTurnTimer(thinkingTime, "전략 페이즈", false);
            yield return new WaitUntil(() => !isTurnActive);

            // **② 전투 페이즈 (15초) - 점수 공개**
            StartTurnTimer(battleTime, "전투 페이즈", true);
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
