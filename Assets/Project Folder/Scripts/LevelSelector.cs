using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void LoadScene1()
    {
        SceneManager.LoadScene("PlayHumans"); 
    }
    public void LoadScene2()
    {
        SceneManager.LoadScene("PlayZombies");
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting"); 
    }
}