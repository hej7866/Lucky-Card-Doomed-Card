using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;


public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartBattleWhenReady()
    {
        Debug.Log("[BattleManager] StartBattleWhenReady 호출됨");
        StartCoroutine(WaitAndStartBattle());
    }

    IEnumerator WaitAndStartBattle()
    {
        Debug.Log("[BattleManager] WaitAndStartBattle 시작");

        // 오류 디버그용 .. 
        if (PlayerManager.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber) == null)
        {
            Debug.LogError("1번문제: 내 PlayerManager 없음");
        }
        else if (PhotonNetwork.PlayerListOthers.Length <= 0)
        {
            Debug.LogError("2번문제: 상대 없음");
        }
        else if (PlayerManager.GetPlayer(PhotonNetwork.PlayerListOthers[0].ActorNumber) == null)
        {
            Debug.LogError("3번문제: 상대 PlayerManager 없음");
        }

        yield return new WaitUntil(() =>
            PlayerManager.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber) != null &&
            PhotonNetwork.PlayerListOthers.Length > 0 &&
            PlayerManager.GetPlayer(PhotonNetwork.PlayerListOthers[0].ActorNumber) != null
        );

        Debug.Log("[BattleManager] 조건 만족 → CalculateBattle 호출");
        CalculateBattle();
    }

    public void CalculateBattle()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 방장만 전투 계산

        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object playerScoreObj) ||
            !PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isAttackSelected", out object playerAttackObj))
        {
            Debug.LogError("내 Score 또는 isAttackSelected 없음");
            return;
        }

        Player enemy = PhotonNetwork.PlayerListOthers[0];
        if (!enemy.CustomProperties.TryGetValue("Score", out object enemyScoreObj) ||
            !enemy.CustomProperties.TryGetValue("isAttackSelected", out object enemyAttackObj))
        {
            Debug.LogError("상대 Score 또는 isAttackSelected 없음");
            return;
        }

        int playerScore = (int)playerScoreObj;
        bool playerAttack = (bool)playerAttackObj;
        int enemyScore = (int)enemyScoreObj;
        bool enemyAttack = (bool)enemyAttackObj;

        int winnerActor = -1;
        int loserActor = -1;
        int damage = 0;

        // ===== 전투 판정 =====
        if (playerAttack && enemyAttack) // 공격 vs 공격
        {
            if (playerScore > enemyScore)
            {
                winnerActor = PhotonNetwork.LocalPlayer.ActorNumber;
                loserActor = enemy.ActorNumber;
                damage = playerScore * 2;
            }
            else if (enemyScore > playerScore)
            {
                winnerActor = enemy.ActorNumber;
                loserActor = PhotonNetwork.LocalPlayer.ActorNumber;
                damage = enemyScore * 2;
            }
        }
        else if (playerAttack && !enemyAttack && playerScore > enemyScore) // 공격 vs 수비
        {
            winnerActor = PhotonNetwork.LocalPlayer.ActorNumber;
            loserActor = enemy.ActorNumber;
            damage = playerScore - enemyScore;
        }
        else if (!playerAttack && enemyAttack && enemyScore > playerScore) // 수비 vs 공격
        {
            winnerActor = enemy.ActorNumber;
            loserActor = PhotonNetwork.LocalPlayer.ActorNumber;
            damage = enemyScore - playerScore;
        }
        else if (!playerAttack && !enemyAttack) // 수비 vs 수비
        {
            if (playerScore > enemyScore)
            {
                winnerActor = enemy.ActorNumber;
                loserActor = PhotonNetwork.LocalPlayer.ActorNumber;
                damage = playerScore;
            }
            else if (enemyScore > playerScore)
            {
                winnerActor = PhotonNetwork.LocalPlayer.ActorNumber;
                loserActor = enemy.ActorNumber;
                damage = enemyScore;
            }
        }

        if (winnerActor == -1 || loserActor == -1)
        {
            Debug.Log("무승부 또는 전투 없음");
            return;
        }

        // 체력감소
        PlayerManager loserPM = PlayerManager.GetPlayer(loserActor);
        if (loserPM != null)
        {
            loserPM.photonView.RPC("RpcTakeDamage", loserPM.photonView.Owner, damage);
        }
        else
        {
            Debug.Log("패자 없음.");
        }

        // 체력 동기화
        // UIManager.Instance.photonView.RPC
        // (
        //     "SyncHealth",
        //     RpcTarget.All,
        //     PhotonNetwork.LocalPlayer.ActorNumber,
        //     enemy.ActorNumber,
        //     PlayerManager.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber).playerHealth,
        //     PlayerManager.GetPlayer(enemy.ActorNumber).playerHealth
        // );

        // 연출 정보 전송
        photonView.RPC("PlayBulletAnimation", RpcTarget.All, winnerActor, damage, 3);
    }

    [PunRPC]
    private void PlayBulletAnimation(int winnerActor, int damage, int bulletCount)
    {
        RectTransform myPanel = UIManager.Instance.playerPanel;
        RectTransform enemyPanel = UIManager.Instance.enemyPanel;

        bool isWinner = PhotonNetwork.LocalPlayer.ActorNumber == winnerActor; // 위너 액터가 나인가?
        RectTransform from = isWinner ? myPanel : enemyPanel; // 트루면 myPanel이 쏨
        RectTransform to = isWinner ? enemyPanel : myPanel; // 폴스면 enemyPanel이 쏨

        BulletEffect cannon = FindObjectOfType<BulletEffect>();
        AudioManager.Instance.PlayLaserSFX();
        cannon?.FireBullet(from, to, bulletCount);
        to.GetComponent<Effect>()?.PlayHitEffect(damage);
    }
}
