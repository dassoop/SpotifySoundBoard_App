using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserPlaylists_Item : MonoBehaviour
{
    public string playlistName;
    public string playlistURI;
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
        gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
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
                mainButton.activePlaylistName = playlistName;
                mainButton.activePlaylistURI = playlistURI;
                mainButton.isActive = false;
            }

            else
            {
                
            }
        }




    }
}
