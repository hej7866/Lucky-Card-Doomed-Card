using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _exitPanel;
    public void OnExitPanelBtnClicked()
    {
        _exitPanel.SetActive(true);
    }

    public void OffExitPanelBtnClicked()
    {
        _exitPanel.SetActive(false);
    } 
}
