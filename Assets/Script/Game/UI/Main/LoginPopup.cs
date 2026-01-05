using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour
{
    public Button Exit_Button;
    public Button Login_Button;

    public TMP_InputField _UserName;
    public TMP_InputField _PassWord;

    public GameObject LoginEror;
    private TextMeshProUGUI LoginErorText;
    private Color LoginErorColor;
    private Coroutine currentCoroutine;

    void Start()
    {   
        Exit_Button.onClick.AddListener(() => {Music.Instance.PlaySE(); Destroy(gameObject);});
        Login_Button.onClick.AddListener(Login);
        LoginEror.SetActive(false);
        LoginErorText = LoginEror.GetComponent<TextMeshProUGUI>();
        LoginErorColor = LoginErorText.color;
    }

    private void Login()
    {   
        Music.Instance.PlaySE();
        String UserName = _UserName.text;
        String PassWord = _PassWord.text;

        if(UserName == "admin" && PassWord == "123456")
        {
            SceneManager.LoadScene("Play");
        }
        else if(UserName == "" && PassWord == "")
        {
            ResetHideCoroutineAndAlpha();
            LoginEror.SetActive(true);
            LoginErorText.text = "Please Input UserName or PassWord";
            currentCoroutine = StartCoroutine(HideEror());
        }
        else
        {   
            ResetHideCoroutineAndAlpha();
            LoginEror.SetActive(true);
            LoginErorText.text = "UserName or PassWord  eror Please Input again";
            currentCoroutine = StartCoroutine(HideEror());
        }
    }

    private void ResetHideCoroutineAndAlpha()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        LoginErorColor.a = 1f;
        LoginErorText.color = LoginErorColor;
    }

    IEnumerator HideEror()
    {  
        yield return new WaitForSeconds(1f);

        float start = 0f;
        float end = 3f;

        while (start < end)
        {
            start += Time.deltaTime;
            LoginErorColor.a = Mathf.Lerp(1,0,start / end);
            LoginErorText.color = LoginErorColor;
            yield return null;
        }

        LoginEror.SetActive(false);
        currentCoroutine = null;
    }
}