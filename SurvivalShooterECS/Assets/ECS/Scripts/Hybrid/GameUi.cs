using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    public Slider HealthSlider;
    public Image DamageImage;
    public Text ScoreText;

    public float FlashSpeed = 5f;
    public Color FlashColor = new Color(1f, 0f, 0f, 0.1f);

    private bool damaged;

    private void Update()
    {
        DamageImage.color = damaged ? FlashColor : Color.Lerp(DamageImage.color, Color.clear, FlashSpeed * Time.deltaTime);

        damaged = false;
    }

    public void OnPlayerTookDamage(float newHealth)
    {
        HealthSlider.value = newHealth;
        damaged = true;
    }

    public void OnEnemyKilled(int score)
    {
        ScoreText.text = $"SCORE: {score}";
    }
}
