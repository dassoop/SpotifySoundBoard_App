using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderButton : MonoBehaviour
{
    public GameObject slider;
    public bool isActive = false;

    public void Start()
    {
        slider.gameObject.SetActive(false);
    }

    public void Update()
    {
        
    }

    public void OnPressed()
    {
        if(isActive == false)
        {
            slider.gameObject.SetActive(true);
            isActive = true;
        }
        else
        {
            slider.gameObject.SetActive(false);
            isActive = false;
        }
        
    }        
}
