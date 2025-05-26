using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;

    [Header("플레이어 / 적 닉네임 및 체력 Text")]
    public RectTransform playerPanel;
    public RectTransform enemyPanel;

    [SerializeField] private Text playerNickname;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyNickname;
    [SerializeField] private Text enemyHealthText;
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Text enemyScoreText;

    [Header("게임종료 UI")]
    public GameObject hidePanel;
    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private Text gameResultText;

    [Header("방 나가기 UI")]
    [SerializeField] private GameObject exitRoomPanel;


    [Header("다이스 / 카드 카운트")]
    [SerializeField] private Text card01DrawCountText;
    [SerializeField] private Text card02DrawCountText;

    [Header("설정 창")]
    public GameObject settingPanel;

    [Header("Obj")]
    [SerializeField] private GameObject objRoot;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(DelayedSetup());

        // drawCount UI 갱신 이벤트 연결
        CardManager.Instance.OnCard01DrawCountChanged += UpdateCardDrawCount;
        CardManager.Instance.OnCard02DrawCountChanged += UpdateCardDrawCount;

        TurnManager.Instance.OnTurnPhaseChanged += HandleTurnPhaseChanged;
    }

    private void HandleTurnPhaseChanged(TurnManager.TurnPhase phase)
    {
        objRoot.SetActive(phase != TurnManager.TurnPhase.None);
    }

    IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.3f); // 0.3초 정도 기다린다

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Nickname", out object nickname))
        {
            playerNickname.text = $"닉네임 : {nickname}";
        }
        else
        {
            playerNickname.text = "닉네임 : ???";
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Health", out object health))
        {
            playerHealthText.text = $"HP : {health}";
        }

        UpdateEnemyInfo();
    }


    // 상대 플레이어가 방에 입장했을 때 호출됨
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새 플레이어의 커스텀 프로퍼티가 설정되도록 잠깐 기다렸다가 업데이트
        StartCoroutine(WaitAndUpdateEnemyInfo(newPlayer));
    }

    private IEnumerator WaitAndUpdateEnemyInfo(Player newPlayer)
    {
        // 조금 기다려서 새 플레이어의 커스텀 프로퍼티가 설정될 시간을 줍니다.
        yield return new WaitForSeconds(0.3f);

        object enemyNick;
        if (newPlayer.CustomProperties.TryGetValue("Nickname", out enemyNick))
        {
            enemyNickname.text = $"닉네임 : {enemyNick}";
        }
        object enemyHealth;
        if (newPlayer.CustomProperties.TryGetValue("Health", out enemyHealth))
        {
            enemyHealthText.text = $"HP : {enemyHealth}";
        }
        else
        {
            Debug.LogWarning("상대 플레이어의 Health 프로퍼티가 설정되지 않았습니다.");
        }
    }

    // 방에 있는 모든 상대방 정보를 초기화하는 메서드 (2인용이므로 최대 1명)
    private void UpdateEnemyInfo()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.IsLocal)
            {
                object enemyNick;
                if (player.CustomProperties.TryGetValue("Nickname", out enemyNick))
                {
                    enemyNickname.text = $"닉네임 : {enemyNick}";
                }
                object enemyHealth;
                if (player.CustomProperties.TryGetValue("Health", out enemyHealth))
                {
                    enemyHealthText.text = $"HP : {enemyHealth}";
                }
            }
        }
    }

    // 상대 플레이어가 방을 떠날 때 호출됨
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 상대방이 나가면 닉네임과 체력 텍스트를 초기화
        enemyNickname.text = "닉네임 : -";
        enemyHealthText.text = "HP : -";
        enemyScoreText.text = "점수 : 0";
    }


    // 커스텀 프로퍼티 변경 시 호출 (상대방 체력이나 점수가 바뀔 때)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Health"))
        {
            int newHealth = (int)changedProps["Health"];

            if (targetPlayer.IsLocal)
            {
                playerHealthText.text = $"HP : {newHealth}";
            }
            else
            {
                enemyHealthText.text = $"HP : {newHealth}";
            }
        }

        if (changedProps.ContainsKey("Score"))
        {
            int newScore = (int)changedProps["Score"];

            if (targetPlayer.IsLocal)
            {
                playerScoreText.text = $"점수 : {newScore}";
            }
            else
            {
                enemyScoreText.text = $"점수 : {newScore}";
            }
        }
    }

    // [PunRPC] // 체력 동기화
    // public void SyncHealth(int myActorNumber, int enemyActorNumber, int newPlayerHealth, int newEnemyHealth)
    // {
    //     Debug.Log("SyncHealth 실행");
    //     int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    //     if (localActorNumber == myActorNumber)
    //     {
    //         playerHealthText.text = $"HP: {newPlayerHealth}";
    //         enemyHealthText.text = $"HP: {newEnemyHealth}";
    //     }
    //     else if (localActorNumber == enemyActorNumber)
    //     {
    //         playerHealthText.text = $"HP: {newEnemyHealth}";
    //         enemyHealthText.text = $"HP: {newPlayerHealth}";
    //     }
    // }

    public void UpdateCardDrawCount(int drawCount, int cardNum)
    {
        if (cardNum == 1)
        {
            card01DrawCountText.text = $"{drawCount} / 3";
        }
        else if (cardNum == 2)
        {
            card02DrawCountText.text = $"{drawCount} / 3";
        }
    }




    public void ShowGameResultPanel(string message) // 게임 종료 스크린 띄우는 로직
    {
        gameResultText.text = message;

        hidePanel.SetActive(true);
        gameResultPanel.SetActive(true);
    }

    public void CloseGameResultPanel() // 게임 결과 창 끄기
    {
        hidePanel.SetActive(false);
        gameResultPanel.SetActive(false);

        TurnManager.Instance.currTurn = 1;

        if(PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.gameStartBtn.SetActive(true);
        }
    }

    public void ShowExitRoomPanel() // 방 나가기 스크린 띄우는 로직
    {
        hidePanel.SetActive(true);
        exitRoomPanel.SetActive(true);
    }

    public void CloseExitRoomPanel() // 방 나가기 창 끄기
    {
        hidePanel.SetActive(false);
        exitRoomPanel.SetActive(false);
    }

    public void ShowSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }



    public void ToggleScoreVisibility(bool show)
    {
        enemyScoreText.gameObject.SetActive(show);
    }
}
