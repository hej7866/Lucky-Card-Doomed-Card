using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public GameObject gameStartBtn;
    public GameObject exitRoomBtn;


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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"방장이 변경되었습니다! 새로운 방장: {newMasterClient.NickName}");

        // 자신이 새로운 방장이면 게임 시작 버튼 보이기
        if (PhotonNetwork.IsMasterClient)
        {
            gameStartBtn.SetActive(true);
        }
        else
        {
            gameStartBtn.SetActive(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 상대방이 방을 떠났을 때
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            LogManager.Instance.AddRPCLog("상대방이 나갔습니다. 게임이 종료됩니다.");
            EndGame();

            //photonView.RPC("GameOver", RpcTarget.All);
        }
    }

    [PunRPC]
    void ResetGameStateRPC() // 게임상태를 초기화하는 로직 (게임끝났을때 해줘야함)
    {
        TurnManager.Instance.ResetGameState();
        StrategyManager.Instance.ResetStrategyState();
        CardManager.Instance.ResetCard();
        DeckManager.Instance.InitializeDeck(); // 크랙카드도 초기화
    }

    public void EndGame()
    {
        LogManager.Instance.AddRPCLog("게임 종료!");

        // 상태 초기화 전파
        photonView.RPC("ResetGameStateRPC", RpcTarget.All);

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

        photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, winnerActorNumber, winnerNick);
    }

    [PunRPC]
    void RPC_EndGame(int winnerActorNumber, string winnerNickName)
    {
        string message = (winnerActorNumber == -1) ? "무승부!" : $"승자: {winnerNickName}!";
        UIManager.Instance.ShowGameResultPanel(message);
    }

    public void RetryGame()
    {
        UIManager.Instance.CloseGameResultPanel();

        foreach (var player in Photon.Pun.PhotonNetwork.PlayerList) // 체력 초기화
        {
            if (player.IsLocal)
            {
                PlayerManager.GetPlayer(player.ActorNumber).SetHealth(500); // 내부 상태
            }

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash["Health"] = 500;
            player.SetCustomProperties(hash); // 네트워크에 체력 동기화
        }
    }

    // 방 나가기 & 로비 이동
    public void ExitRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // 방을 나간 후 연결 상태 확인하고 로비로 이동
    public override void OnLeftRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // 다시 서버에 연결
            return;
        }
    }

    // Photon이 재연결되면 로비로 이동
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.LoadLevel("LobbyScene"); // 로비 씬으로 이동
    }
}
