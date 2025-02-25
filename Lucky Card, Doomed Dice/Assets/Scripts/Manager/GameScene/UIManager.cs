using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerNickname;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyNickname;
    [SerializeField] private Text enemyHealthText;
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Text enemyScoreText;

    private void Start()
    {
        // 내 닉네임 설정 (이미 커스텀 프로퍼티에 저장되었다고 가정)
        if (PhotonNetwork.InRoom)
        {
            object nickname;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Nickname", out nickname))
            {
                playerNickname.text = $"닉네임 : {nickname}";
            }
        }

        // 내 체력 초기화
        object health;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Health", out health))
        {
            playerHealthText.text = $"HP : {health}";
        }

        // 상대방 정보 초기화 (방에 있는 다른 플레이어가 있으면)
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
        enemyScoreText.text = "점수 : -";
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
}
