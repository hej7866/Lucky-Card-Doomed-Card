using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public Text statusText; // UI에 연결 상태 표시
    public string gameVersion = "1.0"; // 게임 버전 관리

    public GameObject loginPanel;
    public GameObject roomList;

    // Photon 서버 연결 시도
    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Photon 서버 연결 중...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings(); // 서버 연결 시작
        }
    }

    // Photon 서버 연결 성공 시 호출
    public override void OnConnectedToMaster()
    {
        statusText.text = "Photon 서버 연결 성공! 로비 접속 중...";
        PhotonNetwork.JoinLobby(); // 자동으로 로비에 접속
    }

    // 로비 접속 완료
    public override void OnJoinedLobby()
    {
        statusText.text = "로비 참가 완료! 방을 만들거나 참가하세요.";
        Debug.Log("로비 참가 완료!");

        loginPanel.SetActive(false);
        roomList.SetActive(true);
    }

    // 서버 연결 실패 시 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"서버 연결 실패: {cause}";
        Debug.LogError($"Photon 서버 연결 실패: {cause}");
    }


    public void OnExitBtnClicked()
    {
        Application.Quit();
    }
}
