using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackCardUIManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;


    public void ShowCrackDeckUI()
    {
        cardPrefab.SetActive(true);
        UIManager.Instance.hidePanel.SetActive(true);
        ShowCrackDeck(DeckManager.Instance.myDeck);
    }

    public void ShowCrackDeck(List<CrackCard> deck)
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject); // 기존 카드 제거
        }

        foreach (var card in deck)
        {
            GameObject obj = Instantiate(cardPrefab, cardParent);
            obj.GetComponent<CrackCardView>().Setup(card);
            var CrackCardClick = obj.GetComponent<CrackCardClick>();
            CrackCardClick.Initialize(card);
        }
    }
}

