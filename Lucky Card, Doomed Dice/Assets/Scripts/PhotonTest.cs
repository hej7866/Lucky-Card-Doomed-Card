using Photon.Pun;
using UnityEngine;

public class PhotonTest : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        Debug.Log("Photon 서버 연결 시도 중...");
        PhotonNetwork.ConnectUsingSettings(); // Photon 서버 연결
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Photon 서버 연결 성공!");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"❌ Photon 서버 연결 실패: {cause}");
    }
}
