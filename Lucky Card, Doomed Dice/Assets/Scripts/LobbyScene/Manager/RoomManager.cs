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
    
    [Header("비밀번호 입력 UI")]
    [SerializeField] private GameObject passwordInputPanel; // 비밀번호 입력 UI
    [SerializeField] private InputField passwordInputField; // 비밀번호 입력 필드
    [SerializeField] private Button confirmPasswordBtn; // 비밀번호 확인 버튼
    private string selectedRoomName; // 선택한 방 이름 (비밀번호 확인용)
    private string correctRoomPassword; // 선택한 방의 비밀번호 저장

    public InputField roomNameInput;  
    public InputField passwordInput;  
    public Toggle passwordToggle;    
    public Text roomStatusText;     

    private void Start()
    {
        TogglePasswordInput();
        passwordToggle.onValueChanged.AddListener(delegate { TogglePasswordInput(); });

        // 비밀번호 확인 버튼에 함수 연결
        confirmPasswordBtn.onClick.AddListener(AttemptJoinWithPassword);
    }

    public void CreateRoom()
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
        passwordInput.interactable = passwordToggle.isOn; 
        if (!passwordToggle.isOn)
        {
            passwordInput.text = ""; 
        }
    }

    // ✅ 방 생성 또는 참가 (공개방 / 비번방 설정)
    public void CreateOrJoinRoom()
    {
        string enteredRoomName = roomNameInput.text.Trim();
        string enteredPassword = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(enteredRoomName))
        {
            roomStatusText.text = "⚠ 방 이름을 입력하세요!";
            return;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        };

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

        PhotonNetwork.AutomaticallySyncScene = true; 

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🎮 마스터 클라이언트가 씬 이동 실행!");
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            Debug.Log("⏳ 대기 중... 마스터 클라이언트가 씬 이동을 실행해야 함");
        }
    }

    // ❌ 방 참가 실패 시
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomStatusText.text = $"❌ 방 참가 실패: {message}";
        Debug.LogError($"❌ 방 참가 실패: {message}");
    }

    // ✅ 방 리스트에서 비밀번호 체크 후 참가 (방 목록 UI에서 선택 시 호출)
    public void TryJoinRoom(RoomInfo room)
    {
        selectedRoomName = room.Name;

        // 방에 비밀번호가 없으면 즉시 참가
        if (!room.CustomProperties.ContainsKey("pwd") || string.IsNullOrEmpty((string)room.CustomProperties["pwd"]))
        {
            PhotonNetwork.JoinRoom(selectedRoomName);
            roomStatusText.text = $"🚪 공개방 '{selectedRoomName}' 참가 중...";
            return;
        }

        // 비밀번호 방이면 비밀번호 입력 UI 활성화
        correctRoomPassword = (string)room.CustomProperties["pwd"];
        passwordInputPanel.SetActive(true); // 비밀번호 입력 UI 표시
    }

    // ✅ 사용자가 비밀번호 입력 후 확인 버튼을 누르면 실행
    public void AttemptJoinWithPassword()
    {
        string enteredPassword = passwordInputField.text.Trim();

        if (enteredPassword == correctRoomPassword) // 비밀번호가 맞다면 입장
        {
            passwordInputPanel.SetActive(false);
            PhotonNetwork.JoinRoom(selectedRoomName);
            roomStatusText.text = $"🔑 비밀번호 확인 완료! 방 '{selectedRoomName}' 참가 중...";
        }
        else
        {
            roomStatusText.text = $"❌ 비밀번호가 틀렸습니다!";
            Debug.LogError("❌ 비밀번호가 틀렸습니다!");
        }
    }
}
