using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject PauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        //Set the game to pause when the scene starts
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        //Set the game to pause
        PauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        //Set the game to resume
        PauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}