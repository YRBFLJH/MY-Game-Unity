using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button Start_Button;
    public Button Setting_Button;

    public GameObject SettingPrefab;
    private GameObject _SettingInstance;
    public GameObject LoginPrefab;
    private GameObject _loginInstance;

    void Start()
    {
        Start_Button.onClick.AddListener(StartGame);
        Setting_Button.onClick.AddListener(Setting);

        Music.Instance.playMainBGM();
    }

    private void StartGame()
    {   
        Music.Instance.PlaySE();
        if (_loginInstance == null)
            _loginInstance = Instantiate(LoginPrefab,transform);
    }

    private void Setting()
    {   
        Music.Instance.PlaySE();
        if (_SettingInstance == null)
            _SettingInstance = Instantiate(SettingPrefab,transform);
    }

}
