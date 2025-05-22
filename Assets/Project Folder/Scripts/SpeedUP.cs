using UnityEngine;
using CustomZombieNamespace;

public class SpeedUpCollectible : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatFrequency = 1f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float duration = 5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombiePlayer"))
        {
            CustomZombiePlayerControl playerControl = other.GetComponent<CustomZombiePlayerControl>();
            if (playerControl != null)
            {
                playerControl.ApplySpeedBoost(speedMultiplier, duration);
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPosition + new Vector3(0, floatOffset, 0);
    }
}