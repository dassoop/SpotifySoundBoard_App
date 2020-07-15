﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayController : MonoBehaviour
{
    public Text trackNameDisplay;
    public Text trackArtistDisplay;
    public Text playlistURIDisplay;
    public Text userPlaylists;
    public Text currentPlaylistDisplay;
    public Text userIDDisplay;
    public Text userRegionDisplay;
    public Text connectedDevice;


    public API api;

    private string trackName;

    private void Update()
    {
        trackNameDisplay.text = api.trackInfo;
        trackArtistDisplay.text = api.trackArtist;
        userPlaylists.text = api.userPlaylistsString;
        currentPlaylistDisplay.text = api.currentPlaylistName;
        userIDDisplay.text = api.userID;
        userRegionDisplay.text = api.userRegion;
        connectedDevice.text = api.connectedDeviceName;
    }
}
