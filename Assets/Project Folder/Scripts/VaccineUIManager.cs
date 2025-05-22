using UnityEngine;

public class VaccineUIManager : MonoBehaviour
{
    [SerializeField] private GameObject vaccineUI; 

    private void Start()
    {
        if (vaccineUI != null)
        {
            vaccineUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Vaccine UI is not assigned in the inspector!");
        }
    }

    private void Update()
    {
        if (vaccineUI != null)
        {
            vaccineUI.SetActive(Collectible.HasVaccine);
        }
    }
}
