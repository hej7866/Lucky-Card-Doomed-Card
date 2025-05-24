using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CalculateBattle()
    {
        // 내 정보 가져오기
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object playerScoreObj) ||
            !PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isAttackSelected", out object playerAttackObj))
        {
            Debug.LogError("로컬 플레이어의 Score 혹은 isAttackSelected를 찾을 수 없습니다! 전투 계산 중단");
            return;
        }

        int playerScore = (int)playerScoreObj;
        bool playerAttack = (bool)playerAttackObj;

        // 상대방 정보 가져오기
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("EnemyScore", out object enemyScoreObj) ||
            !PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("EnemyAttack", out object enemyAttackObj))
        {
            Debug.LogError("EnemyScore 또는 EnemyAttack 값을 찾을 수 없습니다! 전투 계산 중단");
            return;
        }

        int enemyScore = (int)enemyScoreObj;
        bool enemyAttack = (bool)enemyAttackObj;

        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int enemyActorNumber = PhotonNetwork.PlayerListOthers[0].ActorNumber;

        PlayerManager myPlayer = PlayerManager.GetPlayer(myActorNumber);
        PlayerManager enemyPlayer = PlayerManager.GetPlayer(enemyActorNumber);

        if (myPlayer == null || enemyPlayer == null)
        {
            Debug.LogError("PlayerManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        // 연출 유틸
        var cannon = FindObjectOfType<BulletEffect>();

        // ===== 전투 판정 =====

        if (playerAttack && enemyAttack)
        {
            if (playerScore > enemyScore)
            {
                int dmg = playerScore * 2;
                enemyPlayer.TakeDamage(dmg);
                UIManager.Instance.enemyPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.playerPanel.GetComponent<RectTransform>(), UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), 3);
            }
            else if (enemyScore > playerScore)
            {
                int dmg = enemyScore * 2;
                myPlayer.TakeDamage(dmg);
                UIManager.Instance.playerPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), UIManager.Instance.playerPanel.GetComponent<RectTransform>(), 3);
            }
        }
        else if (playerAttack && !enemyAttack)
        {
            if (playerScore > enemyScore)
            {
                int dmg = playerScore - enemyScore;
                enemyPlayer.TakeDamage(dmg);
                UIManager.Instance.enemyPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.playerPanel.GetComponent<RectTransform>(), UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), 3);
            }
        }
        else if (!playerAttack && enemyAttack)
        {
            if (enemyScore > playerScore)
            {
                int dmg = enemyScore - playerScore;
                myPlayer.TakeDamage(dmg);
                UIManager.Instance.playerPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), UIManager.Instance.playerPanel.GetComponent<RectTransform>(), 3);
            }
        }
        else if (!playerAttack && !enemyAttack)
        {
            if (playerScore > enemyScore)
            {
                int dmg = playerScore;
                myPlayer.TakeDamage(dmg);
                UIManager.Instance.playerPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), UIManager.Instance.playerPanel.GetComponent<RectTransform>(), 3);
            }
            else if (enemyScore > playerScore)
            {
                int dmg = enemyScore;
                enemyPlayer.TakeDamage(dmg);
                UIManager.Instance.enemyPanel.GetComponent<Effect>()?.PlayHitEffect(dmg);
                cannon?.FireBullet(UIManager.Instance.playerPanel.GetComponent<RectTransform>(), UIManager.Instance.enemyPanel.GetComponent<RectTransform>(), 3);
            }
        }

        // 체력 동기화
        UIManager.Instance.photonView.RPC("SyncHealth", RpcTarget.All,
            myActorNumber, enemyActorNumber, myPlayer.playerHealth, enemyPlayer.playerHealth);
    }

}
