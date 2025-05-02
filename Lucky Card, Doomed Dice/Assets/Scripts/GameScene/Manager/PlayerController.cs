using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public string playerPrefab = "PlayerPrefab";
    public Transform spawnPoint;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"방 참가 확인: {PhotonNetwork.CurrentRoom.Name}");
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        // 혹시라도 이전에 남아있는 Spawned 상태 제거
        PhotonNetwork.LocalPlayer.CustomProperties.Remove("Spawned");

        Debug.Log("방 참가 완료, 플레이어 생성 시도!");
        SpawnPlayer();
    }

    public override void OnLeftRoom()
    {
        // 방 나갈 때 상태 정리
        PhotonNetwork.LocalPlayer.CustomProperties.Remove("Spawned");
    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Spawned") &&
            (bool)PhotonNetwork.LocalPlayer.CustomProperties["Spawned"])
        {
            Debug.Log("이미 플레이어가 생성됨, 중복 생성 방지");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        PhotonNetwork.Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        Hashtable props = new Hashtable { { "Spawned", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log("플레이어 프리팹을 생성했습니다!");
    }
}
