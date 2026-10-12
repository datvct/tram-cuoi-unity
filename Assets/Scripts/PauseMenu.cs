using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private WaveController waves;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        if (waves == null || pausePanel == null ||
            resumeButton == null || restartButton == null)
        {
            Debug.LogError(
                "PauseMenu: hãy gán đủ các ô trong Inspector."
            );

            enabled = false;
            return;
        }

        pausePanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void Update()
    {
        // Không che bảng kết quả bằng bảng tạm dừng.
        if (!waves.IsStarted || waves.IsFinished)
        {
            pausePanel.SetActive(false);
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.escapeKey.wasPressedThisFrame)
        {
            waves.SetPaused(!waves.IsPaused);
        }

        pausePanel.SetActive(waves.IsPaused);
    }

    private void ResumeGame()
    {
        GameAudio.Instance?.PlayClick();
        waves.SetPaused(false);
        pausePanel.SetActive(false);
    }

    private void RestartGame()
    {
        GameAudio.Instance?.PlayClick();
        GameAudio.Instance?.StopGameplaySounds();

        AudioListener.pause = false;
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }
}