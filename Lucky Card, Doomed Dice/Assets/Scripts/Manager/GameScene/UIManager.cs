using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] Text playerNickname;
    [SerializeField] Text playerHealth;

    void Start()
    {
        // ✅ Photon에서 닉네임 가져오기
        if (PhotonNetwork.InRoom)
        {
            object nickname;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Nickname", out nickname))
            {
                playerNickname.text = $"닉네임 : {nickname}";
                Debug.Log($"🎉 게임 씬에서 닉네임 로드: {nickname}");
            }
            else
            {
                Debug.LogError("❌ 닉네임을 가져오지 못함!");
            }
        }
        else
        {
            Debug.LogError("❌ Photon 방에 입장하지 않음!");
        }

        playerHealth.text = $"HP : {GameManager.Instance.playerHealth}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
