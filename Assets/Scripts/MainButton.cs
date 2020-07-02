using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : MonoBehaviour
{
	public API api;
	public GameObject buttonGreen;
    public Text activePlaylistText;
    public string activePlaylistName;
    public string activePlaylistURI;
    public bool isActive;

    public UserPlaylists_Item userPlaylistsItem;

	public void Update()
	{
		if (api.isTrackPlaying == true)
		{
			//buttonGreen.SetActive(true);
		}
		else
		{
			//buttonGreen.SetActive(false);
		}
        activePlaylistText.text = activePlaylistName;
	}

	public void OnPressed()
	{
		//Debug.Log("Start Playlist: " + activePlaylistName);
		//Debug.Log("uri: " + activePlaylistURI);
		API.instance.StartPlaylistRequest(activePlaylistURI);
        //api.RequestCurrentPlaylistInfo();
    }
}
