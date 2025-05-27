using System.Collections.Generic;
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>(); // ID 기반 관리

    public int playerHealth = 500;

    private IEnumerator Start()
    {
        yield return null;

        int actorNumber = photonView.OwnerActorNr;

        // 무조건 덮어쓰기 (예전 것 제거)
        Players[actorNumber] = this;
        Debug.Log($"[PlayerManager] 등록 완료 : {actorNumber}");

        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Health", playerHealth } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            Debug.Log($"[PlayerManager] 내 초기 체력 등록 완료: {playerHealth}");
        }
    }



    [PunRPC]
    public void RpcTakeDamage(int damage)
    {
        Debug.Log($"[RpcTakeDamage] {PhotonNetwork.NickName} 체력 감소: {damage}");

        playerHealth -= damage;
        if (playerHealth < 0) playerHealth = 0;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Health", playerHealth } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }


    public void SetHealth(int hp)
    {
        playerHealth = hp;
    }

    public static PlayerManager GetPlayer(int actorNumber)
    {
        return Players.ContainsKey(actorNumber) ? Players[actorNumber] : null;
    }
}
