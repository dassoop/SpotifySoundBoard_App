using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class MainButton : MonoBehaviour
{
	public API api;
	public GameObject buttonGreen;
    public GameObject buttonFadeInRed;
	public Slider slider;
    public Text activePlaylistText;
    public string activePlaylistName;
    public string activePlaylistURI;
	public int activePlaylistFirstTrackDuration;
    public int volume;
    public bool isActive;
    public bool fadeInButtonIsActive = false;
    public bool isFadingIn = false;

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
        if(fadeInButtonIsActive)
        {
            StartCoroutine(VolumeFadeIn());
        }
		API.instance.StartPlaylistRequest(activePlaylistURI, (int)slider.value);
    }

    public void FadeInButtonActivate()
    {
        if(!fadeInButtonIsActive)
        {
            fadeInButtonIsActive = true;
            buttonFadeInRed.SetActive(true);
        }
        else
        {
            fadeInButtonIsActive = false;
            buttonFadeInRed.SetActive(false);
        }
    }

    IEnumerator VolumeFadeIn()
    {
        API.instance.isFadingIn = true;
        volume = 0;
        while(volume < 101)
        {
            ChangeVolumeRequest();
            yield return new WaitForSeconds(.03f);
            volume++;
            Debug.Log(volume);
        }
        volume = 100;
        API.instance.isFadingIn = false;
        Debug.Log("FADE DONE");
    }

    public void ChangeVolumeRequest()
    {
        string bodyJsonString = "Body: Info";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        UnityWebRequest www = UnityWebRequest.Put("https://api.spotify.com/v1/me/player/volume?volume_percent=" + volume, bodyRaw);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + API.instance.accessToken);

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
