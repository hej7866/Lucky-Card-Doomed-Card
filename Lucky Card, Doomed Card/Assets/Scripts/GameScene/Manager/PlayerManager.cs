using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>(); // ID 기반 관리

    public int playerHealth = 500;

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

    [PunRPC]
    public void RpcTakeDamage(int damage)
    {
        Debug.Log($"[RpcTakeDamage] {PhotonNetwork.NickName} 체력 감소: {damage}");

        playerHealth -= damage;
        if (playerHealth < 0) playerHealth = 0;

        Hashtable props = new Hashtable { { "Health", playerHealth } };
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
