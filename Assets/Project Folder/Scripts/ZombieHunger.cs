using UnityEngine;
using UnityEngine.UI;

public class ZombieHungerTimer : MonoBehaviour
{
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private float hungerDuration = 30f;
    [SerializeField] private GameObject deathUI;

    private float currentHungerTime;

    private void Start()
    {
        if (hungerSlider == null)
        {
            Debug.LogError("Hunger Slider is not assigned in the ZombieHungerTimer script!");
            return;
        }

        currentHungerTime = hungerDuration;
        hungerSlider.maxValue = hungerDuration;
        hungerSlider.value = currentHungerTime;

        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentHungerTime > 0)
        {
            currentHungerTime -= Time.deltaTime;
            hungerSlider.value = currentHungerTime;

            if (currentHungerTime <= 0)
            {
                TriggerDeathUI();
            }
        }
    }

    public void ResetHungerTimer()
    {
        currentHungerTime = hungerDuration;
        hungerSlider.value = currentHungerTime;
    }

    private void TriggerDeathUI()
    {
        if (deathUI != null)
        {
            deathUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}