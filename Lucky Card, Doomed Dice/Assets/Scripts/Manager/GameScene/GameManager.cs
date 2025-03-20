using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public GameObject gameStartBtn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) // 방장이 아니라면 게임스타트 버튼 숨김
        {
            gameStartBtn.SetActive(false);
        }   
    }

    public void GameStart()
    {
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            Debug.LogWarning("상대방이 존재하지 않습니다. 게임을 시작할 수 없습니다.");
            return;
        }

        LogManager.Instance.AddRPCLog("게임을 시작합니다.");
        TurnManager.Instance.TurnStart();
        gameStartBtn.SetActive(false);
    }

    public void EndGame()
    {
        LogManager.Instance.AddRPCLog("게임 종료!");

        PlayerManager winner = null;
        int maxHealth = 0;

        foreach (var player in PlayerManager.Players.Values)
        {
            if (player.playerHealth > maxHealth)
            {
                maxHealth = player.playerHealth;
                winner = player;
            }
        }

        int winnerActorNumber = (winner != null) ? winner.photonView.Owner.ActorNumber : -1;
        string winnerNick = (winner != null) ? winner.photonView.Owner.NickName : "Unknown";

        Debug.Log($"승자 확인 - ActorNumber: {winnerActorNumber}, 닉네임: {winnerNick}");

        photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, winnerActorNumber, winnerNick);
    }

    [PunRPC]
    void RPC_EndGame(int winnerActorNumber, string winnerNickName)
    {
        Debug.Log($"[RPC] 게임 종료 수신 - winnerActorNumber: {winnerActorNumber}, 닉네임: {winnerNickName}");

        string message = (winnerActorNumber == -1) ? "무승부!" : $"승자: {winnerNickName}!";
        UIManager.Instance.ShowGameOverScreen(message);
    }

    public void RetryGame()
    {
        UIManager.Instance.CloseGameResultPanle();
    }

    // ✅ 방 나가기 & 로비 이동
    public void ExitRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("방을 나갑니다...");
            PhotonNetwork.LeaveRoom();
        }
    }

    // ✅ 방을 나간 후 로비로 이동
    public override void OnLeftRoom()
    {
        Debug.Log("방을 떠났습니다. 로비로 이동 중...");
        PhotonNetwork.JoinLobby();
    }
}
