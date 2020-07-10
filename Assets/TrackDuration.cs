using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TrackDuration : MonoBehaviour
{
    private float durationFloat;
    private float progressFloat;

    void Update()
    {
        durationFloat = float.Parse(API.instance.trackDuration, CultureInfo.InvariantCulture);
        progressFloat = float.Parse(API.instance.trackProgress, CultureInfo.InvariantCulture);
        Debug.Log(durationFloat);

        gameObject.GetComponent<Slider>().maxValue = durationFloat;
        gameObject.GetComponent<Slider>().value = progressFloat;
    }
}
