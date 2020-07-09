using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Repeat_Off : MonoBehaviour
{
    public GameObject buttonGreen;

    void Update()
    {
        if(API.instance.repeatState == "off")
        {
            buttonGreen.gameObject.SetActive(true);
        }
        else
        {
            buttonGreen.gameObject.SetActive(false);
        }
    }
}
