using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Repeat_Context : MonoBehaviour
{
    public GameObject buttonGreen;

    void Update()
    {
        if (API.instance.repeatState == "context")
        {
            buttonGreen.gameObject.SetActive(true);
        }
        else
        {
            buttonGreen.gameObject.SetActive(false);
        }
    }
}
