using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Shuffle : MonoBehaviour
{
    public API api;
    public GameObject buttonHighlight;

    private void Update()
    {
        if (api.shuffleState == "True")
        {
            buttonHighlight.SetActive(true);
        }
        else
        {
            buttonHighlight.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (api.shuffleState == "True")
        {
            api.ShuffleButtonRequest("false");
        }
        else
        {
            //Debug.Log("HERE");
            api.ShuffleButtonRequest("true");
        }
    }
}
