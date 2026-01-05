using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public AudioSource seAudioSource;
    public AudioClip mainBgm;
    public AudioClip battleBgm;
    public AudioClip ButtonClickSE;
    public static Music Instance;
    public bool isSE = true;

    void Awake()
    {
        if(Instance == null) Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    //BGM
    public void playMainBGM()
    {
        if(bgmAudioSource.clip != mainBgm)
        {
            bgmAudioSource.clip = mainBgm;
            bgmAudioSource.Play();
        }
    }
    public void playBattleBGM()
    {
        if(bgmAudioSource.clip != battleBgm)
        {
            bgmAudioSource.clip = battleBgm;
            bgmAudioSource.Play();
        }
    }
    public void OpenBGM()
    {   if(!bgmAudioSource.isPlaying)
            bgmAudioSource.Play();
    }
    public void CloseBGM()
    {
        bgmAudioSource.Stop();
    }
    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        bgmAudioSource.volume = volume;
    }



    //SE
    public void PlaySE()
    {   
        if(isSE == true)
            seAudioSource.PlayOneShot(ButtonClickSE);
    }
    public void OpenSE()
    {
        isSE = true;
    }
    public void CloseSE()
    {
        isSE = false;
    }
    public void SetSEVolume(float volume)
    {
        seAudioSource.volume = Mathf.Clamp01(volume);
    }

}
