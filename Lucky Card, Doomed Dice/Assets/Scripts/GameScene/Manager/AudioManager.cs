using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : SingleTon<AudioManager>
{
    [SerializeField] private GameObject settingPanel;

    private AudioSource audioSource;
    private Slider bgmVolumeSlider;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bgmVolumeSlider = settingPanel.GetComponentInChildren<Slider>();

        audioSource.volume = bgmVolumeSlider.value;

        bgmVolumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
