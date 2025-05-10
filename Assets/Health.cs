using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;
    public int maxHealth = 100;
    public Text healthText;
    public int currentHealth;
    public GameObject DeathScreen;

    [Header("Áudio")]
    public AudioClip somDano;

    [Header("Blood")]
    public Image blood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        CameraShake.Instance?.Shake(0.1f, 0.15f);
        if (somDano != null)
            AudioSource.PlayClipAtPoint(somDano, transform.position);

        currentHealth -= damage;
        UpdateUI();

        if (currentHealth <= 0)
        {
            int finalScore = GetComponent<Score>().Currentscore;
            StartCoroutine(DeathAndLoad(finalScore));
        }
    }

    public void SetBloodOpacity(float opacity)
    {
        Color color = blood.color;
        color.a = opacity;
        blood.color = color;
    }
    private IEnumerator DeathAndLoad(int finalScore)
    {
        PlayerPrefs.SetInt("PlayerFinalScore", finalScore);
        PlayerPrefs.Save();

        DeathScreen.SetActive(true);
        Time.timeScale = 0f;


        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        DeathScreen.SetActive(false);
    foreach (var shooter in FindObjectsOfType<MovimentoCC>())
        Debug.Log("Still alive shooter: " + shooter.gameObject.name);
        SceneManager.LoadScene("Leaderboard", LoadSceneMode.Single);
    }

    private void UpdateUI()
    {
        slider.value = currentHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
        float t = 1f - (currentHealth / (float)maxHealth);
        float a = Mathf.Lerp(0f, 0.8f, t);  
        SetBloodOpacity(a);
    }

    public void Heal(int heal)
    {
        currentHealth = currentHealth + heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateUI();
    }
}
