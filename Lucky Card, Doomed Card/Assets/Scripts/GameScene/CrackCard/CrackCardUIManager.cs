using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrackCardUIManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardParentObj;
    [SerializeField] private Transform cardParent;

    [SerializeField] private GameObject offCrackDeckBtn;


    public void ShowCrackDeckUI()
    {
        cardParentObj.SetActive(true);
        offCrackDeckBtn.SetActive(true);
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

    public void OffCrackDeck()
    {
        cardParentObj.SetActive(false);
        offCrackDeckBtn.SetActive(false);
        UIManager.Instance.hidePanel.SetActive(false);
    }
}

