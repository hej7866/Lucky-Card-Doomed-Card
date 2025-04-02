using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeckManager : SingleTon<DeckManager>
{

    [Header("모든 가능 카드 목록")]
    public List<CrackCard> allCardPool;

    [Header("현재 내 덱")]
    public List<CrackCard> myDeck = new List<CrackCard>();

    [Header("최대 사용 가능 횟수")]
    public int maxUses = 3;
    private int usedCount = 0;



    public void InitializeDeck()
    {
        myDeck.Clear();
        List<CrackCard> shuffled = new List<CrackCard>(allCardPool);
        Shuffle(shuffled);

        for (int i = 0; i < 8 && i < shuffled.Count; i++)
        {
            myDeck.Add(shuffled[i]);
        }

        Debug.Log("덱 초기화 완료!");
    }

    public bool CanUseCard()
    {
        return usedCount < maxUses;
    }

    public void UseCard(CrackCard card)
    {
        if (!CanUseCard())
        {
            Debug.LogWarning("카드 사용 횟수 초과!");
            return;
        }

        if (!myDeck.Contains(card))
        {
            Debug.LogWarning("덱에 없는 카드입니다!");
            return;
        }

        usedCount++;
        myDeck.Remove(card);

        PlayerManager user = null;
        PlayerManager opponent = null;

        foreach (var kvp in PlayerManager.Players)
        {
            if (kvp.Key == PhotonNetwork.LocalPlayer.ActorNumber)
                user = kvp.Value;
            else
                opponent = kvp.Value;
        }

        if (user != null && opponent != null)
        {
            CrackCardHandler.Instance.UseCrackCard(card, user, opponent);
            Debug.Log($"카드 사용: {card.cardName} ({usedCount}/{maxUses})");
        }
        else
        {
            Debug.LogWarning("플레이어 정보 부족 (user or opponent null)");
        }
    }

    private void Shuffle<T>(List<T> list) // 튜플 스왑 알고리즘
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
