using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameStartBtn;

    void Start()
    {
        if(!PhotonNetwork.IsMasterClient) // 방장이 아니라면 게임스타트 버튼을 숨긴다.
        {
            gameStartBtn.SetActive(false);
        }   
    }

    public void GameStart()
    {
        // 🔹 상대방이 없으면 게임 시작 불가능
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            Debug.LogWarning("상대방이 존재하지 않습니다. 게임을 시작할 수 없습니다.");
            return;
        }

        LogManager.Instance.AddLog("게임을 시작합니다.");
        TurnManager.Instance.TurnStart();
        gameStartBtn.SetActive(false);
    }
}
