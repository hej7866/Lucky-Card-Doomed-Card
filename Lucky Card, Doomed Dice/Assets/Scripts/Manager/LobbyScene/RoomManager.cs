using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("방생성 관련 UI")]
    [SerializeField] private Button createRoomBtn;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject hideEffectPanel;
    public InputField roomNameInput;  // 방 이름 입력 필드
    public InputField passwordInput;  // 비밀번호 입력 필드
    public Toggle passwordToggle;     // 비밀번호 사용 여부 체크박스
    public Text roomStatusText;       // 방 상태 표시 UI
    private string enteredRoomName;
    private string enteredPassword;

    private void Start()
    {
        TogglePasswordInput(); // 초기 체크박스 상태 반영
        passwordToggle.onValueChanged.AddListener(delegate { TogglePasswordInput(); }); // 체크박스 변경 감지
    }

    public void CreateRoom() // 방 생성 창 띄우기
    {
        hideEffectPanel.SetActive(true);
        createRoomPanel.SetActive(true);
    }

    public void CloseCreateRoomPanel()
    {
        hideEffectPanel.SetActive(false);
        createRoomPanel.SetActive(false);
    }

    // ✅ 체크박스 상태에 따라 비밀번호 입력 활성화 / 비활성화
    public void TogglePasswordInput()
    {
        passwordInput.interactable = passwordToggle.isOn; // 체크되면 입력 가능, 아니면 비활성화
        if (!passwordToggle.isOn)
        {
            passwordInput.text = ""; // 체크 해제하면 비밀번호 초기화
        }
    }

    // ✅ 방 생성 또는 참가 (공개방 / 비번방 설정)
    public void CreateOrJoinRoom()
    {
        enteredRoomName = roomNameInput.text.Trim();
        enteredPassword = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(enteredRoomName))
        {
            roomStatusText.text = "⚠ 방 이름을 입력하세요!";
            return;
        }

        // 방 옵션 설정
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        };

        // 비밀번호 방이면 Custom Properties에 비밀번호 추가
        if (passwordToggle.isOn && !string.IsNullOrEmpty(enteredPassword))
        {
            roomOptions.CustomRoomProperties["pwd"] = enteredPassword;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "pwd" };
        }

        PhotonNetwork.JoinOrCreateRoom(enteredRoomName, roomOptions, TypedLobby.Default);
        roomStatusText.text = $"🛠 방 '{enteredRoomName}' 생성 또는 참가 시도...";
    }

    // ✅ 방 참가 성공 시 게임 씬 이동
    public override void OnJoinedRoom()
    {
        roomStatusText.text = $"✅ 방 '{PhotonNetwork.CurrentRoom.Name}' 참가 성공!";
        Debug.Log($"✅ 방 참가 완료! 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // ❌ 방 참가 실패 시
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomStatusText.text = $"❌ 방 참가 실패: {message}";
        Debug.LogError($"❌ 방 참가 실패: {message}");
    }


    // ✅ 방 리스트에서 비밀번호 체크 후 참가 (방 목록 UI에서 선택 시 호출)
    public void JoinRoomWithPassword(RoomInfo room)
    {
        // 방에 비밀번호가 설정되지 않았다면 그냥 입장
        if (!room.CustomProperties.ContainsKey("pwd") || string.IsNullOrEmpty((string)room.CustomProperties["pwd"]))
        {
            PhotonNetwork.JoinRoom(room.Name);
            roomStatusText.text = $"🚪 공개방 '{room.Name}' 참가 중...";

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
            return;
        }

        // 비밀번호가 있는 방이라면 입력된 비밀번호와 비교
        string roomPassword = (string)room.CustomProperties["pwd"];

        if (roomPassword == enteredPassword) // 비밀번호 일치 여부 확인
        {
            PhotonNetwork.JoinRoom(room.Name);
            roomStatusText.text = $"🔑 비밀번호 확인 완료! 방 '{room.Name}' 참가 중...";

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
        }
        else
        {
            roomStatusText.text = $"❌ 비밀번호가 틀렸습니다!";
            Debug.LogError("❌ 비밀번호가 틀렸습니다!");
        }
    }

}
