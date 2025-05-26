using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingPanel;

    private AudioSource _audioSource;
    private Slider _bgmVolumeSlider;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _bgmVolumeSlider = _settingPanel.GetComponentInChildren<Slider>();

        _audioSource.volume = _bgmVolumeSlider.value;

        _bgmVolumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        _audioSource.volume = value;
    }
}
