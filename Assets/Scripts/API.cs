using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class API : MonoBehaviour
{
    public static API instance;
    public Text trackInfoDisplay;
    public Transform userPlayListLabel;
    public Transform currentPlaylistLabel;
    public GameObject textInput;
    public GameObject authorizeButton;
    public GameObject textConnected;
    public GameObject userPlaylistItem;
    public GameObject currentPlaylistItem;
    public Slider spotifyVolumeSlider;
    public JSONNode playlistItemResponse;
    public int playlistItemCount = 0;

    [HideInInspector]
    public float itemMovementAmount = -35;
    [HideInInspector]
    public float playlistItemMovementAmount = -35;

    [HideInInspector]
    public bool isTrackPlaying;

    private string authCode;
    private string accessToken;

    //String from Spotify JSON
    [HideInInspector]
    public string trackInfo;
    [HideInInspector]
    public string trackURI;
    [HideInInspector]
    public string trackArtist;
    [HideInInspector]
    public string isPlayingString;
    [HideInInspector]
    public string userPlaylistsString;
    [HideInInspector]
    public string currentPlaylist;


    private bool isRequestingPlaylistInfo = false;

    private string URL = "https://accounts.spotify.com/api/token";

    private string clientID = "ef6bfa33a6644af9a8faa584319caeb4";
    private string clientSecret = "94ec513e49cf472892006983735503fc";

    private void Awake()
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

    public void Update()
    {
       if (isPlayingString == "True")
        {
            isTrackPlaying = true;
        }
       else
        {
            isTrackPlaying = false;
        }

        if (isRequestingPlaylistInfo == false)
        {
            //RequestCurrentPlaylistInfo();
        }
        else
        {

        }

        //RequestCurrentPlaylistInfo();
    }

    //**Request Access Token with Code**
    public void Request()
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", authCode);
        //form.AddField("redirect_uri", "https://example.com/callback");
        form.AddField("redirect_uri", "https://spotifysoundboard.herokuapp.com/callback");

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        byte[] bytesToEncode = Encoding.UTF8.GetBytes("ef6bfa33a6644af9a8faa584319caeb4:94ec513e49cf472892006983735503fc");
        string encodedText = Convert.ToBase64String(bytesToEncode);

        www.SetRequestHeader("Authorization", "Basic " + encodedText);

        StartCoroutine(Response(www));
    }

    IEnumerator Response(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);
            Debug.Log(www.responseCode);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }

        JSONNode tokenResponseInfo = JSON.Parse(www.downloadHandler.text);
        accessToken = tokenResponseInfo["access_token"];
        //Debug.Log("Access token is " + accessToken);

        authorizeButton.SetActive(false);
        textInput.SetActive(false);
        textConnected.SetActive(true);
        RequestUserPlaylists();
        RequestTrackInfo();
        //RequestCurrentPlaylistInfo();
    }

    //Set Access Code equal to the code pasted into the text field
    public void SetCode()
    {
        authCode = textInput.GetComponent<InputField>().text;
        Request();
    }




    //**GET USER PLAYLISTS**
    public void RequestUserPlaylists()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/me/playlists?limit=8");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        StartCoroutine(ResponseUserPlaylists(www));
    }

    IEnumerator ResponseUserPlaylists(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.responseCode);
        }

        JSONNode playlistResponseInfo = JSON.Parse(www.downloadHandler.text);
        ParseJSONArray(playlistResponseInfo);
        //userPlaylistsString = playlistResponseInfo["items"]["name"];
        //Debug.Log(userPlaylistsString);
    }

    private void ParseJSONArray(JSONNode playlistResponseInfo)
    {
        foreach (JSONNode item in playlistResponseInfo["items"])
        {
            string playlistName = item["name"];
            string playlistURI = item["uri"];

            userPlaylistsString = playlistName;

            Vector3 newPosition = new Vector3(userPlayListLabel.position.x, userPlayListLabel.position.y + itemMovementAmount, userPlayListLabel.position.z);
            Instantiate(userPlaylistItem, newPosition, userPlayListLabel.rotation);

            userPlaylistItem.GetComponent<UserPlaylists_Item>().SetItemInfo(playlistName, playlistURI);

            //Debug.Log(itemMovementAmount);
            itemMovementAmount -= 35;
        }
    }


    //REQUEST CURRENT PLAYLISTINFO FIRED OFF THE MAIN BUTTONS
    public void RequestCurrentPlaylistInfo()
    {
        isRequestingPlaylistInfo = true;

        currentPlaylist = currentPlaylist.Replace("spotify:playlist:", "");
        //UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/{playlist_id}/tracks?limit=6");
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/" + currentPlaylist + "/tracks?limit=6");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        StartCoroutine(ResponseCurrentPlaylistInfo(www));
    }

    IEnumerator ResponseCurrentPlaylistInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        isRequestingPlaylistInfo = false;

        //Debug.Log("REQUEST PLAYLIST INFO" + www.downloadHandler.text);
        JSONNode playlistItemResponse = JSON.Parse(www.downloadHandler.text);
        RefreshCurrentPlaylistInfo();
        ParsePlaylistItemResponse(playlistItemResponse);

    }

    public void ParsePlaylistItemResponse(JSONNode playlistItemResponse)
    {
        foreach (JSONNode item in playlistItemResponse["items"])
        {
            if(playlistItemCount < 6)
            {
                string playlistItemName = item["track"]["name"];
                string playlistItemURI = item["track"]["uri"];

                Vector3 newPosition = new Vector3(currentPlaylistLabel.position.x, currentPlaylistLabel.position.y + playlistItemMovementAmount, currentPlaylistLabel.position.z);
                Instantiate(currentPlaylistItem, newPosition, currentPlaylistLabel.rotation);

                currentPlaylistItem.GetComponent<CurrentPlaylist_Item>().SetItemInfo(playlistItemName, playlistItemURI);

                //Debug.Log("ITEM MOVEMENT AMOUNT: " + playlistItemMovementAmount);
                playlistItemMovementAmount -= 35;
                playlistItemCount++;
                Debug.Log("PARSING " + playlistItemCount);
            }
        }
    }

    public void RefreshCurrentPlaylistInfo()
    {
        GameObject[] gameObjects;

        gameObjects = GameObject.FindGameObjectsWithTag("CurrentPlaylist_Item");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
        RequestCurrentPlaylistInfo();
        playlistItemMovementAmount = -35;
        playlistItemCount = 0;
    }


    //**REQUEST TRACK INFO**
    public void RequestTrackInfo()
    {
        //string bodyJsonString = "Body: Info";
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);

        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/me/player");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(ResponseTrackInfo(www));
    }

    IEnumerator ResponseTrackInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        JSONNode trackInfoResponse = JSON.Parse(www.downloadHandler.text);
        trackInfo = trackInfoResponse["item"]["name"];
        trackURI = trackInfoResponse["item"]["uri"];
        trackArtist = trackInfoResponse["artists"]["name"];
        isPlayingString = trackInfoResponse["is_playing"];
        currentPlaylist = trackInfoResponse["context"]["uri"];
    }







    // **Playback Button Requests**

    public void PlayButtonRequest()
    {
        TrackForm[] trackForm = new TrackForm[1];
        trackForm[0] = new TrackForm();
        trackForm[0].uris = trackURI;
        //trackForm.uris = trackURI;
        string bodyRaw = JsonUtility.ToJson(trackForm);
        //Debug.Log(bodyRaw);

        //string bodyJsonString = "context_uri : " + trackURI;
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);

        //JSONObject json = new JSONObject();
        //json.Add("uri_context", trackURI);

        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/play", bodyRaw);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");
        //www.SetRequestHeader("context_uri", "spotify:album:1Je1IMUlBXcx1Fz0WE7oPT");

        StartCoroutine(PlayButtonResponse(www));
    }

    IEnumerator PlayButtonResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        RequestTrackInfo();
    }

    public void PauseButtonRequest()
    {
        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/pause", "Accept: application/json");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(PauseButtonResponse(www));
    }

    IEnumerator PauseButtonResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        RequestTrackInfo();
    }

    public void SkipButtonRequest()
    {
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post("https://api.spotify.com/v1/me/player/next", form);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //www.SetRequestHeader("Accept", "application/json");
        //www.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(SkipButtonResponse(www));
    }

    IEnumerator SkipButtonResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        RequestTrackInfo();
    }

    public void PreviousButtonRequest()
    {
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post("https://api.spotify.com/v1/me/player/previous", form);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //www.SetRequestHeader("Accept", "application/json");
        //www.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(PreviousButtonResponse(www));
    }

    IEnumerator PreviousButtonResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        RequestTrackInfo();
    }

    //**START PLAYLIST REQUEST**
    public void StartPlaylistRequest(string playlistURI)
    {
        PlaylistForm playlistForm = new PlaylistForm();
        playlistForm.context_uri = playlistURI;
        string bodyRaw = JsonUtility.ToJson(playlistForm);
        //Debug.Log(bodyRaw);

        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/play", bodyRaw);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(StartPlaylistResponse(www));
    }

    IEnumerator StartPlaylistResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }

        //RefreshCurrentPlaylistInfo();
        RequestTrackInfo();
    }



    //**VOLUME CONTROLS**
    public void ChangeVolumeRequest()
    {
        int volume = (int)spotifyVolumeSlider.GetComponent<Slider>().value;
        //Debug.Log(volume);
        string bodyJsonString = "Body: Info";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/volume?volume_percent=" + volume, bodyRaw);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        StartCoroutine(ChangeVolumeResponse(www));
    }

    IEnumerator ChangeVolumeResponse(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}

//-H "Accept: application/json"
//-H "Content-Type: application/json"

//"access_token":"BQB20Zuj1wXJvU9gHtF93gzXlpRwYuoO0h-RAkkftAwujNUllFzr_5DA4ArfA2euEOsOskU04SbhU1IvkBiDiqrJPSMpi1qaLMhjXsxEA6ZWdNbKWRaCwqyFNG6YjYp-MXum5jrv1ABYSA21pLyQueji-Uf_zg"
//"token_type":"Bearer"
//"expires_in":3600
//"refresh_token":"AQD_QuH_8G7cxjuH9H07noVo8K8BnXvmVNsgjkJyXYaUt779Ua9cvW9tQ63Xieu64HU9B2Zc0YTRIVr7WCWnC3ougCn9kgICDW7eJAU1XcGCd9N_omDZOGTZyBvRizyrCjQ"
//"scope":"user-read-email user-read-private"