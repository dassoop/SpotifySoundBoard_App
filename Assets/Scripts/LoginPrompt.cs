using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginPrompt : MonoBehaviour
{
    public GameObject textInput;
    public static LoginPrompt instance;

    //private string URL = "https://accounts.spotify.com/authorize?client_id=ef6bfa33a6644af9a8faa584319caeb4&response_type=code&redirect_uri=https%3A%2F%2Fexample.com%2Fcallback&scope=user-read-private%20user-read-email&state=34fFs29kd09";
    //private string URL = "https://accounts.spotify.com/authorize?client_id=ef6bfa33a6644af9a8faa584319caeb4&response_type=code&redirect_uri=https%3A%2F%2Fexample.com%2Fcallback&scope=user-read-private%20user-read-email%20user-modify-playback-state%20user-read-playback-state%20playlist-read-private%20playlist-read-collaborative&state=34fFs29kd09";
    private string URL = "https://accounts.spotify.com/authorize?client_id=ef6bfa33a6644af9a8faa584319caeb4&response_type=code&redirect_uri=https%3A%2F%2Fspotifysoundboard.herokuapp.com%2Fcallback&scope=user-read-private%20user-read-email%20user-modify-playback-state%20user-read-playback-state%20playlist-read-private%20playlist-read-collaborative&state=34fFs29kd09";

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.OpenURL(URL);
    }

    //public void Request()
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(URL);
    //    StartCoroutine(Response(www));
    //}

    //IEnumerator Response(UnityWebRequest www)
    //{
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        // Show results as text
    //        Debug.Log(www.downloadHandler.text);
    //        // Or retrieve results as binary data
    //        byte[] results = www.downloadHandler.data;
    //    }
    //}

    //public void SetCode()
    //{
    //    authCode = textInput.GetComponent<InputField>().text;
    //    //Debug.Log(authCode);
    //    API.instance.Request();
    //}
}
