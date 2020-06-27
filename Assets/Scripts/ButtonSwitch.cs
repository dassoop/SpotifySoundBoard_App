using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    public API api;
    public GameObject buttonGreen;

    public void Update()
    {
        if (api.isTrackPlaying == true)
        {
            buttonGreen.SetActive(true);
        }
        else
        {
            buttonGreen.SetActive(false);
        }
    }

    public void OnPressed()
    {
        if (api.isTrackPlaying == true)
        {
            api.PauseButtonRequest();
        }
        else
        {
            api.PlayButtonRequest();
        }
    }
}
