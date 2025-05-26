using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CrackCardClick : MonoBehaviour
{
    private Button crackCardPrefab;

    // 이 버튼이 대표하는 카드
    public CrackCard crackCardData;

    private void Awake()
    {
        crackCardPrefab = GetComponent<Button>();
        crackCardPrefab.onClick.AddListener(OnButtonClicked);
    }


    public void Initialize(CrackCard card)
    {
        crackCardData = card;
        // UI 텍스트/아이콘 설정도 여기서 하면 됨
    }

    private void OnButtonClicked()
    {
        if (TurnManager.Instance.CurrentPhase == TurnManager.TurnPhase.Battle)
        {
            LogManager.Instance.AddLog("전투 페이즈에는 덱을 펼칠 수 없습니다!");
            return;
        }

        if (TurnManager.Instance.isScoreSelected)
        {
            LogManager.Instance.AddLog("이미 스코어를 결정하셨습니다.");
            return;
        }

        // 여기에 크랙카드 핸들러에 전달해주는 코드
            var user = PlayerManager.Players[PhotonNetwork.LocalPlayer.ActorNumber];
        PlayerManager opponent = null;

        // 플레이어 찾기 로직 (예: Dictionary에서 내 ActorNumber가 아니면 opponent로 설정)
        // 예: 1:1 게임이라면
        foreach (var kvp in PlayerManager.Players)
        {
            if (kvp.Key != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                opponent = kvp.Value;
                break;
            }
        }

        CrackCardHandler.Instance.UseCrackCard(crackCardData, user, opponent);
        DeckManager.Instance.UseCard(crackCardData);
        if (!DeckManager.Instance.CanUseCard()) return;
        Destroy(this.gameObject);
        LogManager.Instance.AddLog($"{crackCardData.cardName} 카드를 사용했습니다!");
        DeckManager.Instance.usedCount++;
    }
}

