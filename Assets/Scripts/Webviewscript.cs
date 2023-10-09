using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gpm.WebView;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Webviewscript : MonoBehaviour
{
    public void ShowUrlPopupPositionSize()
    {
        GpmWebViewSafeBrowsing.ShowSafeBrowsing("https://arrowserver.vercel.app/auth/google",
                new GpmWebViewRequest.ConfigurationSafeBrowsing()
                {
                    navigationBarColor = "#000000",
                    navigationTextColor = "#FFFFFF"
                },
                OnCallback);
    }

    private void OnCallback(
    GpmWebViewCallback.CallbackType callbackType,
    string data,
    GpmWebViewError error)
    {
        Debug.Log("OnCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);

                    if (data == "https://arrowserver.vercel.app/auth/protected")
                    {
                        StartCoroutine(FetchJsonData(data));
                    }
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                if (error == null)
                {
                    if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
                    {
                        Debug.Log(string.Format("scheme:{0}", data));
                    }
                }
                else
                {
                    Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                }
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
#if UNITY_ANDROID
        case GpmWebViewCallback.CallbackType.BackButtonClose:
            Debug.Log("BackButtonClose");
            break;
#endif
        }
    }

    IEnumerator FetchJsonData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error while fetching data");
        }
        else
        {
            SigninResponse response = JsonUtility.FromJson<SigninResponse>(request.downloadHandler.text);

            PlayerPrefs.SetString("Token", response.token);

            Debug.Log(response.result);
            PlayerPrefs.SetString("UserData", JsonUtility.ToJson(response.result));

            UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));

            Debug.Log(user.isOnboarded);
            if (user.isOnboarded == true)
            {
                SceneManager.LoadScene("Home");
            }
            else
            {
                SceneManager.LoadScene("[Ob]Start");
            }
        }
    }
}