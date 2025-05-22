using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    [Header("Invincibility Frames")]
    [SerializeField] private float invincibilityDuration = 2f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("Damage Visual Effects")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material damageMaterial;
    private Material[] originalMaterials;
    private bool isFlashing = false;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 5f;
    private Rigidbody rb;

    [Header("DeathUI")]
    [SerializeField] private TextMeshProUGUI deathScoreText; // Referință directă pentru scorul din DeathUI

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (skinnedMeshRenderer != null)
        {
            originalMaterials = skinnedMeshRenderer.materials;
        }

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                StopAllCoroutines();
                RestoreOriginalMaterials();
            }
        }
    }

    public void TakeDamage(int damageAmount, Vector3 hitDirection)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        UpdateHealthText();

        StartInvincibility();

        StartCoroutine(DamageVisualEffect());

        ApplyKnockback(hitDirection);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    private IEnumerator DamageVisualEffect()
    {
        isFlashing = true;

        float flashDuration = invincibilityDuration;
        float timeElapsed = 0f;

        while (timeElapsed < flashDuration)
        {
            if (skinnedMeshRenderer != null)
            {
                Material[] materials = skinnedMeshRenderer.materials;
                materials[0] = damageMaterial;
                materials[1] = damageMaterial;
                skinnedMeshRenderer.materials = materials;
            }

            yield return new WaitForSeconds(0.1f);

            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.materials = originalMaterials;
            }

            yield return new WaitForSeconds(0.1f);

            timeElapsed += 0.2f;
        }

        RestoreOriginalMaterials();

        isFlashing = false;
    }

    private void RestoreOriginalMaterials()
    {
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.materials = originalMaterials;
        }
    }

    private void ApplyKnockback(Vector3 hitDirection)
    {
        if (rb != null)
        {
            rb.AddForce(hitDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    private void Die()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (deathScoreText != null)
        {
            deathScoreText.text = $"{ScoreManager.Instance.GetCurrentScore()}";
        }

        Time.timeScale = 0;
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}