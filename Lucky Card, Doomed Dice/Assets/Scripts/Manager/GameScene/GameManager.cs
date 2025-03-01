using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameStartBtn;

    public void GameStart()
    {
        // 🔹 상대방이 없으면 게임 시작 불가능
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            Debug.LogWarning("상대방이 존재하지 않습니다. 게임을 시작할 수 없습니다.");
            return;
        }

        gameStartBtn.SetActive(false);
        TurnManager.Instance.TurnStart();
    }
}
