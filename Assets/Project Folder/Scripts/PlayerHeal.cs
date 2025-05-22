using CustomZombieNamespace;
using UnityEngine;

public class PlayerHealing : MonoBehaviour
{
    [SerializeField] private float healingRange = 5f;
    [SerializeField] private KeyCode healKey = KeyCode.F;

    private void Update()
    {
        if (Input.GetKeyDown(healKey) && Collectible.HasVaccine)
        {
            if (HealNearbyZombies())
            {
                Collectible.SetHasVaccine(false);
            }
            else
            {
                Debug.Log("No stunned zombies nearby to heal.");
            }
        }
    }

    private bool HealNearbyZombies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, healingRange);
        bool healedAnyZombie = false;

        foreach (var hit in hits)
        {
            CustomZombieCharacterControl zombie = hit.GetComponent<CustomZombieCharacterControl>();
            if (zombie != null && zombie.IsStunned)
            {
                Debug.Log("Healing a stunned zombie!");
                zombie.Disappear();  
                ScoreManager.AddPoints(500);  
                healedAnyZombie = true;
            }
        }

        return healedAnyZombie;
    }
}