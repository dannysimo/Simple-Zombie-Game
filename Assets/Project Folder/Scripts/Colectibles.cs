using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatFrequency = 1f;
    [SerializeField] private GameObject effectPrefab;

    public static bool HasVaccine { get; private set; } = false;
    public static bool HasBomb { get; private set; } = false;

    private Vector3 startPosition;
    private BombTimer bombTimer;

    private void Start()
    {
        startPosition = transform.position;
        bombTimer = FindObjectOfType<BombTimer>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPosition + new Vector3(0, floatOffset, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (HasVaccine && HasBomb) return;
            if (HasVaccine && CompareTag("Vaccine")) return;
            if (HasBomb && CompareTag("Bomb")) return;

            if (CompareTag("Vaccine"))
            {
                HasVaccine = true;
                Debug.Log("Vaccine collected! You can heal stunned zombies.");
                ScoreManager.AddPoints(100);
            }
            else if (CompareTag("Bomb"))
            {
                HasBomb = true;
                bombTimer?.StartBombTimer();
                ScoreManager.AddPoints(100); 
            }

            if (effectPrefab != null)
            {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    public static void SetHasVaccine(bool value)
    {
        HasVaccine = value;
    }

    public static void SetHasBomb(bool value)
    {
        HasBomb = value;
    }
}