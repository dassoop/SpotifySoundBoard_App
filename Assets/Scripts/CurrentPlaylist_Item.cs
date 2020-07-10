using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlaylist_Item : MonoBehaviour
{
    public string playlistItemName;
    public string playlistItemURI;
    public Text playlistItemText;
    public GameObject[] mainButtonObjects;
    public GameObject panelCurrentPlaylist;

    private void Update()
    {
        //Set Parent To Canvas
        //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);

        gameObject.transform.SetParent(GameObject.Find("CurrentPlaylistContent").transform);
    }

    public void SetItemInfo(string name, string uri)
    {
        playlistItemURI = uri;
        playlistItemName = name;
        playlistItemText.text = name;
    }
}
