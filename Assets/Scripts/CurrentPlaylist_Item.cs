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

    private void Start()
    {

    }

    private void Update()
    {
        //Set Parent To Canvas
        gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void SetItemInfo(string name, string uri)
    {
        playlistItemURI = uri;
        playlistItemName = name;
        playlistItemText.text = name;
    }

    //public void SendInfoToPlayButton()
    //{

    //    mainButtonObjects = GameObject.FindGameObjectsWithTag("TriggerButton");

    //    foreach (GameObject mainButtonObject in mainButtonObjects)
    //    {
    //        //Debug.Log(mainButtonObject);
    //        mainButton = mainButtonObject.GetComponent<MainButton>();
    //        Debug.Log(mainButton);

    //        if (mainButton.isActive == true)
    //        {
    //            mainButton.activePlaylistName = playlistItemName;
    //            mainButton.activePlaylistURI = playlistItemURI;
    //            mainButton.isActive = false;
    //        }

    //        else
    //        {

    //        }
    //    }




    //}

}
