using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject mutedIcon; 

    private bool isPaused = false;
    private bool isMuted = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;

        
        AudioListener.volume = isMuted ? 0f : 1f;

        
        if (mutedIcon != null)
        {
            mutedIcon.SetActive(isMuted);
        }
    }
}
