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
    public GameObject panelCurrentPlaylist;
    public Slider spotifyVolumeSlider;
    public JSONNode playlistItemResponse;
    public int playlistItemCount = 0;

    public CurrentPlaylist_Item currentPlaylistItemPrefab;

    [HideInInspector]
    public int trackStartPosition;
    [HideInInspector]
    public float itemMovementAmount = -35;
    [HideInInspector]
    public float playlistItemMovementAmount = -35;

    [HideInInspector]
    public bool isTrackPlaying;
    [HideInInspector]
    public bool isTrackChanging;
    [HideInInspector]
    public bool isFadingIn = false;

    private string authCode;
    public string accessToken;

    //String from Spotify JSON
    [HideInInspector]
    public string trackInfo;
    [HideInInspector]
    public string previousTrackInfo;
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
    [HideInInspector]
    public string currentPlaylistName;
    [HideInInspector]
    public string shuffleState;
    [HideInInspector]
    public string repeatState;
    [HideInInspector]
    public string trackProgress;
    [HideInInspector]
    public string trackDuration;
    [HideInInspector]
    public string availableMarkets;
    [HideInInspector]
    public string userRegion;

    private bool isRequestingPlaylistInfo = false;
    private bool isRequestingTrackInfo = false;
    private bool isRequestingPlayerInfo = false;

    private string URL = "https://accounts.spotify.com/api/token";

    private string clientID = "ef6bfa33a6644af9a8faa584319caeb4";
    private string clientSecret = "94ec513e49cf472892006983735503fc";

    private string deviceID;

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

    public void Start()
    {
        StartCoroutine(LoopingUpdate());
    }

    public void Update()
    {
        //RequestPlayerInfo();
        //if (isRequestingTrackInfo == false)
        //{
        //    RequestTrackInfo();
        //}

        //if (isRequestingPlayerInfo == false)
        //{
        //    RequestPlayerInfo();
        //}

        if (trackInfo != previousTrackInfo)
        {
            //Debug.Log("SONG CHANGED");
            isTrackChanging = true;
            RequestPlaylistInfo();
            RequestCurrentPlaylistInfo();
            previousTrackInfo = trackInfo;
        }
        previousTrackInfo = trackInfo;


        if (isPlayingString == "True")
        {
            isTrackPlaying = true;
        }
        else
        {
            isTrackPlaying = false;
        }
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

        //byte[] bytesToEncode = Encoding.UTF8.GetBytes("ef6bfa33a6644af9a8faa584319caeb4:94ec513e49cf472892006983735503fc");
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(clientID + ":" + clientSecret);
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
            //Debug.Log(www.responseCode);

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
        RequestUserInfo();
        //RequestTrackInfo();
        //RequestCurrentPlaylistInfo();
    }

    //Set Access Code equal to the code pasted into the text field
    public void SetCode()
    {
        //authCode = textInput.GetComponent<InputField>().text;
        authCode = GUIUtility.systemCopyBuffer;
        Debug.Log(authCode);
        Request();
    }

    //**REQUEST USER INFO (LIKE LOCATION AND PERMISSIONS)
    public void RequestUserInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/me");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(ResponseUserInfo(www));
    }

    IEnumerator ResponseUserInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        JSONNode userResponseInfo  = JSON.Parse(www.downloadHandler.text);
        userRegion = userResponseInfo["country"];
        Debug.Log("REGION: " + userRegion);
    }

    //**GET USER PLAYLISTS**
    public void RequestUserPlaylists()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/me/playlists?limit=50");
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
            //Debug.Log(www.responseCode);
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
    //REQUEST SINGLE PLAYLIST INFORMATION LIKE NAME
    public void RequestPlaylistInfo()
    {
        //isRequestingPlaylistInfo = true;
        if (currentPlaylist != null)
        {
            currentPlaylist = currentPlaylist.Replace("spotify:playlist:", "");
        }
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/" + currentPlaylist + "/");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        StartCoroutine(ResponsePlaylistInfo(www));
    }

    IEnumerator ResponsePlaylistInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        JSONNode playlistInfoItemResponse = JSON.Parse(www.downloadHandler.text);
        currentPlaylistName = playlistInfoItemResponse["name"];
    }

    //REQUEST CURRENT PLAYLIST TRACKS AND CREATE THEM IN THE LIST (NO LONGER FIRED OFF THE MAIN BUTTONS)
    public void RequestCurrentPlaylistInfo()
    {
        isRequestingPlaylistInfo = true;

        if (currentPlaylist != null)
        {
            currentPlaylist = currentPlaylist.Replace("spotify:playlist:", "");
        }
        //UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/{playlist_id}/tracks?limit=6");
        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/playlists/" + currentPlaylist + "/tracks?limit=30");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        StartCoroutine(ResponseCurrentPlaylistInfo(www));
    }

    IEnumerator ResponseCurrentPlaylistInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("REQUEST PLAYLIST INFO" + www.downloadHandler.text);
        RefreshCurrentPlaylistInfo();
        JSONNode playlistItemResponse = JSON.Parse(www.downloadHandler.text);
        //RefreshCurrentPlaylistInfo();
        ParsePlaylistItemResponse(playlistItemResponse);
    }

    public void ParsePlaylistItemResponse(JSONNode playlistItemResponse)
    {
        playlistItemCount = 0;

        foreach (JSONNode item in playlistItemResponse["items"])
        {
            bool isAvailableInRegion = false;

            if (playlistItemCount < 30)
            {
                foreach (JSONNode markets in item["track"]["album"]["available_markets"])
                {
                    availableMarkets = markets;

                    if (availableMarkets == userRegion)
                    {
                        isAvailableInRegion = true;
                    }
                }

                if (!isAvailableInRegion)
                {
                    continue;
                }

                string playlistItemName = item["track"]["name"];
                string playlistItemURI = item["track"]["uri"];

                Vector3 newPosition = new Vector3(currentPlaylistLabel.position.x, currentPlaylistLabel.position.y + playlistItemMovementAmount, currentPlaylistLabel.position.z);
                
                currentPlaylistItem.GetComponent<CurrentPlaylist_Item>().SetItemInfo(playlistItemName, playlistItemURI);
                Instantiate(currentPlaylistItem, newPosition, currentPlaylistLabel.rotation);

                playlistItemMovementAmount -= 35;
                playlistItemCount++;

                //Debug.Log("ITEM MOVEMENT AMOUNT: " + playlistItemMovementAmount);   
                //Debug.Log("PARSING " + playlistItemCount);
            }
        }
        isRequestingPlaylistInfo = false;
    }

    //**DELETE CURRENT PLAYLIST LIST AND RESET THE SPAWN POINT**
    public void RefreshCurrentPlaylistInfo()
    {
        GameObject[] gameObjects;

        gameObjects = GameObject.FindGameObjectsWithTag("CurrentPlaylist_Item");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
        //RequestCurrentPlaylistInfo();
        playlistItemMovementAmount = -35;
        playlistItemCount = 0;
    }


    //**REQUEST TRACK INFO** (ACTUALLY PLAYER INFORMATION)
    public void RequestPlayerInfo()
    {
        isRequestingTrackInfo = true;
        //string bodyJsonString = "Body: Info";
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);

        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/me/player");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(ResponsePlayerInfo(www));
    }

    IEnumerator ResponsePlayerInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            //Debug.Log(www.error);
        }

        else
        {
            //Debug.Log(www.downloadHandler.text);
        }

        JSONNode playerInfoResponse = JSON.Parse(www.downloadHandler.text);
        trackInfo = playerInfoResponse["item"]["name"];
        trackURI = playerInfoResponse["item"]["uri"];
        isPlayingString = playerInfoResponse["is_playing"];
        currentPlaylist = playerInfoResponse["context"]["uri"];

        deviceID = playerInfoResponse["device"]["id"];
        shuffleState = playerInfoResponse["shuffle_state"];
        repeatState = playerInfoResponse["repeat_state"];
        trackProgress = playerInfoResponse["progress_ms"];

        foreach (JSONNode item in playerInfoResponse["item"]["album"]["artists"])
        {
            trackArtist = item["name"];
        }

        isRequestingTrackInfo = false;
    }

    public void RequestTrackInfo()
    {
        if(trackURI != null)
        {
            trackURI = trackURI.Replace("spotify:track:", "");
        }

        UnityWebRequest www = UnityWebRequest.Get("https://api.spotify.com/v1/tracks/" + trackURI);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(ResponseTrackInfo(www));
    }

    IEnumerator ResponseTrackInfo(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        JSONNode trackInfoResponse = JSON.Parse(www.downloadHandler.text);
        trackDuration = trackInfoResponse["duration_ms"];

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            //Debug.Log(www.downloadHandler.text);
        }
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

        //RequestTrackInfo();
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

        //RequestTrackInfo();
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
            //Debug.Log(www.downloadHandler.text);
        }

        //RequestTrackInfo();
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

        //RequestTrackInfo();
    }

    //**START PLAYLIST REQUEST**
    public void StartPlaylistRequest(string playlistURI, int trackStartPosition)
    {
        PlaylistForm playlistForm = new PlaylistForm();
        playlistForm.context_uri = playlistURI;
        playlistForm.position_ms = trackStartPosition;
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
        //RequestTrackInfo();
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

    IEnumerator LoopingUpdate()
    {
        while (true)
        {
            RequestPlayerInfo();
            RequestTrackInfo();
            //RequestPlayerInfo();
            yield return new WaitForSeconds(.2f);
        }
    }

    public void RepeatButtonRequest(String state)
    {
        //UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/repeat?state=context&device_id=0d1841b0976bae2a3a310dd74c0f3df354899bc8", "Accept: application/json");
        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/repeat?state=" + state + "&device_id=" + deviceID, "Accept: application/json");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(RepeatButtonResponse(www));
    }
    IEnumerator RepeatButtonResponse(UnityWebRequest www)
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

    public void ShuffleButtonRequest(String state)
    {
        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/shuffle?state=" + state + "&device_id=" + deviceID, "Accept: application/json");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        StartCoroutine(ShuffleButtonResponse(www));
    }
    IEnumerator ShuffleButtonResponse(UnityWebRequest www)
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