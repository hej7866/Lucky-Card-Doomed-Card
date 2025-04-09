using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrackCardView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text cardNameText;
    [SerializeField] private Text descriptionText;

    private CrackCard cardData;

    public void Setup(CrackCard card)
    {
        cardData = card;
        icon.sprite = card.icon;
        cardNameText.text = card.cardName;
        descriptionText.text = card.description;
    }
}

