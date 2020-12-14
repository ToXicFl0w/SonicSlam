using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void playLevel1()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void playLevel2()
    {
        SceneManager.LoadScene("Level_2");
    }
    public void playLevel3()
    {
        SceneManager.LoadScene("MP_Level_1");
    }
    public void playLevel4()
    {
        SceneManager.LoadScene("Level_3");
    }

    public void GoToSettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
    }

    public void GoToMainMenu()
    {
        print("Go to mainmenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
