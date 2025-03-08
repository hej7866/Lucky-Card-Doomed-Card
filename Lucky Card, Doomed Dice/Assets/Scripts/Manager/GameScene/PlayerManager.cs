using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>(); // ID 기반 관리

    public int playerHealth = 1000;

    private void Start()
    {
        // photonView가 초기화된 후 등록 (Start에서 실행)
        if (!Players.ContainsKey(photonView.Owner.ActorNumber))
        {
            Players[photonView.Owner.ActorNumber] = this;
        }

        if (photonView.IsMine) 
        {
            Hashtable props = new Hashtable { { "Health", playerHealth } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public void TakeDamage(int damage)
    {
        // if (!photonView.IsMine) return; // ✅ 내 캐릭터만 체력 감소

        playerHealth -= damage;
        if (playerHealth < 0) playerHealth = 0;

        // // 체력 변경을 Photon에 업데이트
        // Hashtable props = new Hashtable { { "Health", playerHealth } };
        // PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public static PlayerManager GetPlayer(int actorNumber)
    {
        return Players.ContainsKey(actorNumber) ? Players[actorNumber] : null;
    }
}
