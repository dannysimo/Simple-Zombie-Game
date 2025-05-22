using UnityEngine;
using UnityEngine.UI;

public class BombTimer : MonoBehaviour
{
    [SerializeField] private Slider timerSlider;
    [SerializeField] private float timerDuration = 5f;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int bombDamage = 10;

    private bool isTimerRunning = false;
    private float currentTime;

    public bool IsBombCollected { get; private set; } = false;
    public bool HasBombBeenThrown { get; private set; } = false;

    private void Start()
    {
        if (timerSlider != null)
        {
            timerSlider.maxValue = timerDuration;
            timerSlider.value = timerDuration;
            timerSlider.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Timer Slider is missing!");
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (timerSlider != null)
            {
                timerSlider.value = currentTime;
            }

            if (currentTime <= 0)
            {
                isTimerRunning = false;
                if (!HasBombBeenThrown)
                {
                    ApplyDamageToPlayer();
                }
                HideSlider();
                IsBombCollected = false;
                Collectible.SetHasBomb(false);
            }
        }
    }

    public void StartBombTimer()
    {
        if (timerSlider != null)
        {
            currentTime = timerDuration;
            timerSlider.gameObject.SetActive(true);
            isTimerRunning = true;
            IsBombCollected = true;
            HasBombBeenThrown = false;

            Debug.Log("Bomb timer started. Bomb is now collected.");
        }
        else
        {
            Debug.LogError("Timer Slider is missing, cannot start bomb timer!");
        }
    }

    public void ResetBombCollected()
    {
        IsBombCollected = false;
        Collectible.SetHasBomb(false);
        Debug.Log("Bomb collection status reset.");
    }

    private void ApplyDamageToPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(bombDamage, Vector3.zero);
            Debug.Log("Player took damage from bomb.");
        }
    }

    public void HideSlider()
    {
        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(false);
            Debug.Log("Slider hidden.");
        }
    }

    public void MarkBombAsThrown()
    {
        HasBombBeenThrown = true;
        Collectible.SetHasBomb(false);
        Debug.Log("Bomb has been thrown.");
    }
}