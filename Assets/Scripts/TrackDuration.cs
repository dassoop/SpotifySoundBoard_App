using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TrackDuration : MonoBehaviour
{
    private float durationFloat = 1;
    private float progressFloat = 0;

    void Update()
    {
        if (API.instance.trackProgress != null && API.instance.trackDuration != null && API.instance.isConnected == true)
        {
            if(!API.instance.isRequestingTrackInfo && !API.instance.isRequestingPlayerInfo && !API.instance.isRequestingPlaylistInfo)
            {
                if(API.instance.trackDuration != "" && API.instance.trackProgress != "")
                {
                    durationFloat = float.Parse(API.instance.trackDuration, CultureInfo.InvariantCulture);
                    progressFloat = float.Parse(API.instance.trackProgress, CultureInfo.InvariantCulture);

                    gameObject.GetComponent<Slider>().maxValue = durationFloat;
                    gameObject.GetComponent<Slider>().value = progressFloat;
                }
            }
        }
    }
}
