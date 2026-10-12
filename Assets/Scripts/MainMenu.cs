using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private WaveController waves;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (waves == null ||
            menuPanel == null ||
            startButton == null)
        {
            Debug.LogError(
                "MainMenu: hãy gán đủ các ô trong Inspector."
            );

            enabled = false;
            return;
        }

        menuPanel.SetActive(true);
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        GameAudio.Instance?.PlayClick();
        waves.BeginGame();

        // Chỉ ẩn menu nếu trận đã bắt đầu thành công.
        if (waves.IsStarted)
            menuPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(StartGame);
    }
}