using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class UserPlaylists_Item : MonoBehaviour
{
    public string playlistName;
    public string playlistURI;
    public string playlistID;
    public string trackURI;
    public string trackName;
    public int trackDuration;
    public Text playlistItemText;
    public MainButton mainButton;
    public GameObject[] mainButtonObjects;

    private void Start()
    {
        //mainButton = FindObjectOfType<MainButton>();

        //mainButtonObjects = GameObject.FindGameObjectsWithTag("TriggerButton");

        //foreach (GameObject mainButtonObject in mainButtonObjects)
        //{
        //    //Debug.Log(mainButtonObject);
        //    mainButton = mainButtonObject.GetComponent<MainButton>();
        //    Debug.Log(mainButton);
        //}
    }

    private void Update()
    {
        //Set Parent To Canvas
        //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        gameObject.transform.SetParent(GameObject.Find("Content_UserPlaylist").transform);
    }

    public void OnPressed()
    {
        RequestFirstTrackOfPlaylist();
    }

    public void SetItemInfo(string name, string uri)
    {
        playlistURI = uri;
        playlistName = name;
        playlistItemText.text = name;
        //Debug.Log("BRUUU " + name);
    }

    public void SendInfoToPlayButton()
    {
        mainButtonObjects = GameObject.FindGameObjectsWithTag("TriggerButton");

        foreach (GameObject mainButtonObject in mainButtonObjects)
        {
            //Debug.Log(mainButtonObject);
            mainButton = mainButtonObject.GetComponent<MainButton>();
            //Debug.Log(mainButton);

            if (mainButton.isActive == true)
            {
                //RequestFirstTrackOfPlaylist();
                mainButton.activePlaylistName = playlistName;
                mainButton.activePlaylistURI = playlistURI;
                mainButton.slider.maxValue = trackDuration;
                mainButton.isActive = false;
            }

            else
            {
                
            }
        }
    }

    public void RequestFirstTrackOfPlaylist()
    {
        if (playlistURI != null)
        {
            playlistID = playlistURI.Replace("spotify:playlist:", "");
        }
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/" + playlistID + "/tracks?limit=1");
        www.SetRequestHeader("Authorization", "Bearer " + API.instance.accessToken);
        StartCoroutine(ResponseCurrentPlaylistInfo(www));
    }

    IEnumerator ResponseCurrentPlaylistInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        Debug.Log("REQUEST PLAYLIST INFO" + www.downloadHandler.text);
        JSONNode playlistItemResponse = JSON.Parse(www.downloadHandler.text);

        foreach (JSONNode item in playlistItemResponse["items"])
        {
            trackURI = item["track"]["uri"];
            Debug.Log("TRACK URI: " + trackURI);
            break;
        }
        RequestTrackInfo();
    }

    public void RequestTrackInfo()
    {
        if (trackURI != null)
        {
            trackURI = trackURI.Replace("spotify:track:", "");
        }

        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/tracks/" + trackURI);
        www.SetRequestHeader("Authorization", "Bearer " + API.instance.accessToken);
        StartCoroutine(ResponseTrackInfo(www));
    }

    IEnumerator ResponseTrackInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        JSONNode trackInfoResponse = JSON.Parse(www.downloadHandler.text);
        trackDuration = trackInfoResponse["duration_ms"];
        SendInfoToPlayButton();
        Debug.Log("TRACK DURATION: " + trackDuration);
    }
}
