using UnityEngine;

public class BombThrower : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombSpawnPoint;
    [SerializeField] private float bombThrowForce = 10f;
    [SerializeField] private BombTimer bombTimer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowBomb();
        }
    }

    private void ThrowBomb()
    {
        if (bombTimer == null)
        {
            Debug.LogError("BombTimer reference is missing in BombThrower!");
            return;
        }

        if (bombTimer.IsBombCollected)
        {
            if (bombPrefab != null && bombSpawnPoint != null)
            {
                GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, bombSpawnPoint.rotation);

                Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
                if (bombRb != null)
                {
                    bombRb.AddForce(bombSpawnPoint.forward * bombThrowForce, ForceMode.Impulse);
                }

                bombTimer.MarkBombAsThrown();  
                bombTimer.HideSlider();  

                Debug.Log("Bomb thrown successfully.");
            }
        }
        else
        {
            Debug.Log("No bomb collected to throw!");
        }
    }
}