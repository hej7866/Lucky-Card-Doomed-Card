using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public Transform roomListContainer; // 방 목록 UI 부모 오브젝트
    public GameObject roomListItemPrefab; // 방 목록 UI 프리팹

    private Dictionary<string, GameObject> roomItems = new Dictionary<string, GameObject>();

    // ✅ 로비에 들어오면 자동으로 방 목록 갱신 시작
    public override void OnJoinedLobby()
    {
        Debug.Log("✅ 로비 참가 완료! 방 목록을 가져옵니다.");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"📢 방 목록 업데이트: {roomList.Count}개의 방 발견됨.");

        // ✅ 기존 목록 삭제 (삭제된 방 반영)
        foreach (var item in roomItems.Values)
        {
            Destroy(item);
        }
        roomItems.Clear();

        // ✅ 새로운 방 목록 추가
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue; // 삭제된 방 건너뛰기

            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContainer);
            roomItem.GetComponentInChildren<Text>().text = $"방: {room.Name} (인원 {room.PlayerCount}/2)";
            roomItem.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room));

            roomItems.Add(room.Name, roomItem);
        }
    }


    // ✅ 방 선택 후 참가
    public void JoinRoom(RoomInfo room)
    {
        string roomPassword = (string)room.CustomProperties["pwd"];
        Debug.Log($"🛠 방 '{room.Name}' 참가 시도, 비밀번호: {roomPassword}");
        
        FindObjectOfType<RoomManager>().TryJoinRoom(room);
    }
}
