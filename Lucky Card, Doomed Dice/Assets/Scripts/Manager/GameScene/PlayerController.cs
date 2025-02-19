using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Resources 폴더에 있는 플레이어 프리팹의 이름을 지정합니다.
    public string playerPrefab = "PlayerPrefab";
    
    // 옵션: 플레이어가 생성될 위치. (지정하지 않으면 (0,0,0) 위치에 생성됨)
    public Transform spawnPoint;

    private void Start()
    {
        // Photon에 연결되어 있고, 방에 입장한 상태라면
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            PhotonNetwork.Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("플레이어 프리팹을 생성했습니다.");
        }
        else
        {
            Debug.LogWarning("Photon 네트워크에 연결되어 있지 않거나 방에 입장하지 않았습니다.");
        }
    }
}
