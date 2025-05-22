using CustomZombieNamespace;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;  // Prefabul de explozie
    [SerializeField] private float explosionDuration = 2f; // Durata exploziei
    [SerializeField] private float stunRadius = 5f; // Raza în care zombie-urile vor fi afectate de stun
    [SerializeField] private float stunDuration = 3f; // Durata stunsului

    private void OnCollisionEnter(Collision collision)
    {
        // Verificăm dacă bomba a intrat în coliziune și apoi activăm explozia
        TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            // Instanțiem explozia la poziția bombei
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Creăm un collider trigger pentru zona de stun
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, stunRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Zombie"))
                {
                    // Aplicăm stun-ul asupra zombie-urilor din raza exploziei
                    hitCollider.GetComponent<CustomZombieCharacterControl>()?.ApplyStun(stunDuration);
                }
            }

            // Distrugem bomba după ce explozia a fost instanțiată
            Destroy(gameObject);
        }
    }
}