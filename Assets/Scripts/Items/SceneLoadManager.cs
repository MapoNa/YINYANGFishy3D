using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadManager : MonoBehaviour
{
    public void LoadGame()
    {
        // foreach (GameObject rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        // {
        //     if (rootObject.name != "Main Camera") 
        //     {
        //         Destroy(rootObject);
        //     }
        // }

        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
        Time.timeScale = 1f;
    }

    public void LoadScene(string sceneName)
    {
        // Load the specified scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        // Load the Main Menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}