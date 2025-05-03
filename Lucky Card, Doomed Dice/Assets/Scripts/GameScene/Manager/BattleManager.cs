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
        object playerScoreObj, playerAttackObj;
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out playerScoreObj) ||
            !PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isAttackSelected", out playerAttackObj)
        )
        {
            Debug.LogError("로컬 플레이어의 Score 혹은 isAttackSelected를 찾을 수 없습니다! 전투 계산 중단");
            return;
        }

        int playerScore = (int)playerScoreObj;
        bool playerAttack = (bool)playerAttackObj;

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
            if (playerScore > enemyScore)
            {
                int damage = playerScore;
                Debug.Log($"쫄보죄! 플레이어에게 {damage} 데미지");
                myPlayer.TakeDamage(damage);
            }
            else if(playerScore < enemyScore)
            {
                int damage = enemyScore;
                Debug.Log($"쫄보죄! 상대방에게 {damage} 데미지");
                enemyPlayer.TakeDamage(damage);
            }
        }

        // 체력 동기화
        UIManager.Instance.photonView.RPC("SyncHealth", RpcTarget.All, myActorNumber, enemyActorNumber, myPlayer.playerHealth, enemyPlayer.playerHealth);
    }

}
