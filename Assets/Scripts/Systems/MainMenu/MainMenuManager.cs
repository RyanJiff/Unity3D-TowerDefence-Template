using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    /*
     * Manages menus in the main screen
     */

    void Update()
    {
        
    }

    /// <summary>
    /// Loads scene 1 in the inspector
    /// </summary>
    public void LoadScene1()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit(0);
    }
}
