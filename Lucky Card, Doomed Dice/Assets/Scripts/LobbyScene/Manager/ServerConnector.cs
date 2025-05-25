using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ServerConnector : MonoBehaviour
{
    public InputField nicknameInput; // 닉네임 입력 필드
    public Text statusText; // 상태 메시지 표시
    public Launcher launcher; // Photon 서버 연결 담당 (Inspector에서 연결)

    private void Start()
    {
        PlayFabSettings.TitleId = "47F57";  // PlayFab Title ID 설정
    }

    // 서버에 닉네임 등록 및 로그인
    public void ConnectToServer()
    {
        string nickname = nicknameInput.text.Trim();

        if (string.IsNullOrEmpty(nickname))
        {
            statusText.text = "닉네임을 입력하세요!";
            return;
        }

        string uniqueID = SystemInfo.deviceUniqueIdentifier; // 기기 고유 ID 사용 (랜덤 계정 생성)

        var request = new LoginWithCustomIDRequest
        {
            CustomId = uniqueID,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            statusText.text = $"서버 연결 성공!";
            SaveNickname(nickname);
        },
        error =>
        {
            statusText.text = $"서버 연결 실패: {error.GenerateErrorReport()}";
            Debug.LogError($"서버 연결 실패: {error.GenerateErrorReport()}");
        });
    }

    // 서버에 닉네임 저장 후 Photon 연결 실행
    private void SaveNickname(string nickname)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nickname
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            Debug.Log($"닉네임 저장 성공: {result.DisplayName}");

            // PlayFab 로그인 & 닉네임 저장 후 Photon 서버 연결 시작
            SaveNicknameToPhoton(nickname);
            launcher.ConnectToPhoton();
        },
        error =>
        {
            statusText.text = $"닉네임 저장 실패: {error.GenerateErrorReport()}";
            Debug.LogError($"닉네임 저장 실패: {error.GenerateErrorReport()}");
        });
    }

    void SaveNicknameToPhoton(string nickname)
    {
        // Photon에서 사용할 닉네임 설정
        PhotonNetwork.NickName = nickname;

        // Photon CustomProperties에도 닉네임 저장
        Hashtable playerProperties = new Hashtable { { "Nickname", nickname } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        Debug.Log($"Photon 닉네임 설정 완료: {PhotonNetwork.NickName}");
    }
}
