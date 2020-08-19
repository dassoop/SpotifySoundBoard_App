using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject tutorial;
    public GameObject readMe;

    public void OpenMenu()
    {
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void OpenTutorial()
    {
        tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorial.SetActive(false);
    }

    public void OpenReadMe()
    {
        readMe.SetActive(true);
    }

    public void CloseReadMe()
    {
        readMe.SetActive(false);
    }
}
