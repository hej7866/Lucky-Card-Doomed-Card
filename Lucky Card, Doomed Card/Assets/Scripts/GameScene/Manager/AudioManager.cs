using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : SingleTon<AudioManager>
{
    [Header("UI")]
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip _laserSFX;

    void Start()
    {
        _bgmSource.volume = _bgmVolumeSlider.value;
        _bgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);

        _sfxSource.volume = _sfxVolumeSlider.value;
        _sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void SetBgmVolume(float value)
    {
        _bgmSource.volume = value;
    }

    private void SetSfxVolume(float value)
    {
        _sfxSource.volume = value;
    }

    public void PlayLaserSFX()
    {
        _sfxSource.clip = _laserSFX;
        _sfxSource.Play();
    }
}
