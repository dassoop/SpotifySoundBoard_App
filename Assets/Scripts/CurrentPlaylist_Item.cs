using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlaylist_Item : MonoBehaviour
{
    public string playlistItemName;
    public string playlistItemURI;
    public int playlistItemCount;
    public Text playlistItemText;
    public Canvas canvas;
    public GameObject[] mainButtonObjects;
    public GameObject panelCurrentPlaylist;
    public GameObject buttonGreen;

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        transform.localScale = new Vector3(transform.localScale.x * canvas.transform.localScale.x * .9f, transform.localScale.y * canvas.transform.localScale.y * .9f, transform.localScale.z * canvas.transform.localScale.z * .9f);
    }

    private void Update()
    {
        //Set Parent To Canvas
        //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        gameObject.transform.SetParent(GameObject.Find("Content_CurrentPlaylist").transform);

        if (playlistItemName == API.instance.trackInfo)
        {
            //Debug.Log(API.instance.trackURI);
            //Debug.Log(playlistItemURI);
            buttonGreen.SetActive(true);
        }
        else if (API.instance.isTrackChanging == true)
        {
            //Debug.Log(API.instance.trackURI);
            //Debug.Log(playlistItemURI);
            buttonGreen.SetActive(false);
            API.instance.isTrackChanging = false;
        }
    }

    public void SetItemInfo(string name, string uri, int count)
    {
        playlistItemURI = uri;
        playlistItemName = name;
        playlistItemCount = count;
        playlistItemText.text = name;
    }
}
