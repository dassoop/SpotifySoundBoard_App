using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateButton : MonoBehaviour
{
    public GameObject button_green;
    public MainButton mainButton;

    private void Update()
    {
        if (mainButton.isActive == true)
        {
            button_green.SetActive(true);
        }
        else
        {
            button_green.SetActive(false);
        }
    }

    public void OnPressedActivate()
    {
        if(mainButton.isActive == false)
        {
            mainButton.isActive = true;
        }
        else
        {
            mainButton.isActive = false;
        }
    }
}
