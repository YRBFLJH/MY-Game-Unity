using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    public Button Normal_Button;
    public Button Music_Button;
    public Button Screen_Button;
    public Button Exit_Button;

    public GameObject _Normal;


    public GameObject _Music;
    public Toggle _BGMSwitch;
    public Toggle _SESwitch;
    public Slider _BGMVolume;
    public Slider _SEVolume;


    public GameObject _Screen;
    public TMP_Dropdown resolutionChanged;

    private static int CurrentOptionIndex = 0;


    void Start()
    {
        Exit_Button.onClick.AddListener(() => {Music.Instance.PlaySE(); Destroy(gameObject);});
        Normal_Button.onClick.AddListener(ChangedNormal);
        Music_Button.onClick.AddListener(ChangedMusic);
        Screen_Button.onClick.AddListener(ChangedScreen);

        _Normal.SetActive(true);
        _Music.SetActive(false);
        _Screen.SetActive(false);

        RecordBGMToggleState();
        _BGMSwitch.onValueChanged.AddListener(BGMSwitch);
        RecordBGMVolumeState();
        _BGMVolume.onValueChanged.AddListener(BGMVolumeChanged);

        RecordSEToggleState();
        _SESwitch.onValueChanged.AddListener(SESwitch);
        RecordSEVolumeState();
        _SEVolume.onValueChanged.AddListener(SEVolumeChanged);

        resolutionChanged.onValueChanged.AddListener(OnResolutionSelect);
        resolutionChanged.value = CurrentOptionIndex;
    }

    private void ChangedNormal()
    {   
        Music.Instance.PlaySE();
        _Normal.SetActive(true);
        _Music.SetActive(false);
        _Screen.SetActive(false);
    }
    private void ChangedMusic()
    {   
        Music.Instance.PlaySE();
        _Normal.SetActive(false);
        _Music.SetActive(true);
        _Screen.SetActive(false);
    }
    private void ChangedScreen()
    {   
        Music.Instance.PlaySE();
        _Normal.SetActive(false);
        _Music.SetActive(false);
        _Screen.SetActive(true);
    }    




    private void BGMSwitch(bool isOn)
    {   
        Music.Instance.PlaySE();
        if(isOn) Music.Instance.OpenBGM();
        else Music.Instance.CloseBGM();
    }
    private void RecordBGMToggleState()
    {   
        bool isBGM = Music.Instance.bgmAudioSource.isPlaying;
        _BGMSwitch.isOn = isBGM;
    }

    private void BGMVolumeChanged(float volume)
    {   
        Music.Instance.SetBGMVolume(volume);
    }
    private void RecordBGMVolumeState()
    {   
        _BGMVolume.value = Music.Instance.bgmAudioSource.volume;
    }




    private void SESwitch(bool isOn)
    {   
        if(isOn) {Music.Instance.OpenSE();Music.Instance.PlaySE();}
        else Music.Instance.CloseSE();
    }
    private void RecordSEToggleState()
    {   
        bool isSE = Music.Instance.seAudioSource.isPlaying;
        _SESwitch.isOn = isSE;
    }

    private void SEVolumeChanged(float volume)
    {   
        Music.Instance.SetSEVolume(volume);
    }
    private void RecordSEVolumeState()
    {   
        _SEVolume.value = Music.Instance.seAudioSource.volume;
    }


    private void OnResolutionSelect(int optionIndex)
    {   
        Music.Instance.PlaySE();
        switch (optionIndex)
        {
            case 0:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                CurrentOptionIndex = optionIndex;
                break;
            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                CurrentOptionIndex = optionIndex;
                break;
            case 2:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                CurrentOptionIndex = optionIndex;
                break;
        }
    }
}
