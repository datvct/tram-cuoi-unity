using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private WaveController waves;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button restartButton;

    private bool resultShown;

    private void Start()
    {
        if (player == null || waves == null ||
            healthText == null || scoreText == null ||
            waveText == null || endPanel == null ||
            resultText == null || finalScoreText == null ||
            restartButton == null)
        {
            Debug.LogError("GameHUD: hãy gán đủ các ô trong Inspector.");
            enabled = false;
            return;
        }

        endPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void Update()
    {
        healthText.text = $"Máu: {player.CurrentHealth}/{player.MaxHealth}";
        scoreText.text = $"Điểm: {waves.Score}";
        waveText.text = waves.IsStarted
            ? $"Đợt: {waves.CurrentWave}/{waves.TotalWaves}"
            : "Đợt: —";

        if (waves.IsFinished && !resultShown)
        {
            resultShown = true;
            GameAudio.Instance?.PlayResult(waves.HasWon);
            resultText.text = waves.HasWon
                ? "Bạn thắng!"
                : "Bạn đã thua!";

            finalScoreText.text = $"Tổng điểm: {waves.Score}";
            endPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
}