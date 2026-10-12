using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private EnemyChase enemyPrefab;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private int[] enemiesPerWave = { 5, 8, 12 };
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float breakBetweenWaves = 3f;
    [SerializeField] private float minimumPlayerDistance = 2f;

    private int aliveEnemies;
    private bool isReady;

    public bool IsStarted { get; private set; }
    public int Score { get; private set; }
    public int CurrentWave { get; private set; }
    public int TotalWaves => enemiesPerWave.Length;
    public bool IsFinished { get; private set; }
    public bool IsPaused { get; private set; }
    public bool HasWon { get; private set; }

    private void Start()
    {
        if (enemyPrefab == null ||
            player == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0 ||
            enemiesPerWave == null ||
            enemiesPerWave.Length == 0)
        {
            Debug.LogError("WaveController: hãy gán đủ Prefab, Player và điểm sinh.");
            enabled = false;
            return;
        }

        if (enemyPrefab.GetComponent<Health>() == null)
        {
            Debug.LogError("Enemy Prefab đang thiếu component Health.");
            enabled = false;
            return;
        }

        Time.timeScale = 1f;
        isReady = true;
        SetPlayerControls(false);
    }

    private void Update()
    {
        if (IsStarted && !IsFinished &&
        player != null && player.IsDead)
        {
            Finish(false);
        }
    }

    private IEnumerator RunWaves()
    {
        for (int waveIndex = 0;
             waveIndex < enemiesPerWave.Length;
             waveIndex++)
        {
            CurrentWave = waveIndex + 1;
            int spawnedEnemies = 0;

            Debug.Log($"Bắt đầu đợt {CurrentWave}.");

            while (spawnedEnemies < enemiesPerWave[waveIndex])
            {
                Transform spawnPoint = FindSpawnPoint();

                if (spawnPoint == null)
                {
                    // Chờ rồi thử lại nếu chưa có điểm phù hợp.
                    yield return new WaitForSeconds(0.25f);
                    continue;
                }

                EnemyChase enemy = Instantiate(
                    enemyPrefab,
                    spawnPoint.position,
                    Quaternion.identity
                );

                enemy.SetTarget(player.transform);

                Health enemyHealth = enemy.GetComponent<Health>();
                enemyHealth.Died += OnEnemyDied;

                aliveEnemies++;
                spawnedEnemies++;

                yield return new WaitForSeconds(spawnInterval);
            }

            // Đã sinh đủ: chờ người chơi tiêu diệt hết quái.
            while (aliveEnemies > 0)
            {
                yield return null;
            }

            // Cho các xử lý khác trong khung hình có cơ hội hoàn tất.
            yield return null;

            if (player.IsDead)
            {
                Finish(false);
                yield break;
            }

            if (waveIndex < enemiesPerWave.Length - 1)
            {
                Debug.Log("Hết đợt. Nghỉ 3 giây.");
                yield return new WaitForSeconds(breakBetweenWaves);
            }
        }

        Finish(!player.IsDead);
    }

    private Transform FindSpawnPoint()
    {
        int startIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = (startIndex + i) % spawnPoints.Length;
            Transform point = spawnPoints[index];

            if (point == null)
            {
                continue;
            }

            float distance = Vector2.Distance(
                point.position,
                player.transform.position
            );

            if (distance >= minimumPlayerDistance)
            {
                return point;
            }
        }

        return null;
    }

    private void OnEnemyDied(Health enemyHealth)
    {
        enemyHealth.Died -= OnEnemyDied;
        aliveEnemies--;

        if (!IsFinished && !player.IsDead)
        {
            Score += 10;
            Debug.Log($"Điểm: {Score}. Quái còn sống: {aliveEnemies}.");
        }
    }

    private void Finish(bool won)
    {
        if (IsFinished)
        {
            return;
        }
        IsPaused = false;
        Time.timeScale = 1f;
        IsFinished = true;
        HasWon = won;
        StopAllCoroutines();

        player.GetComponent<PlayerMovement>().enabled = false;

        foreach (PlayerWeapon weapon in player.GetComponentsInChildren<PlayerWeapon>())
        {
            weapon.enabled = false;
        }

        foreach (PlayerAim aim in player.GetComponentsInChildren<PlayerAim>())
        {
            aim.enabled = false;
        }

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        body.linearVelocity = Vector2.zero;
        body.constraints = RigidbodyConstraints2D.FreezeAll;

        Debug.Log(won
            ? $"Bạn thắng! Tổng điểm: {Score}."
            : $"Kết thúc lượt chơi. Tổng điểm: {Score}.");
    }

    public void SetPaused(bool paused)
    {
        if (!IsStarted || IsFinished ||
            player == null || player.IsDead)
        {
            return;
        }

        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        SetPlayerControls(!paused);
    }
    
    private void SetPlayerControls(bool active)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (movement != null)
            movement.enabled = active;

        foreach (PlayerWeapon weapon in
                player.GetComponentsInChildren<PlayerWeapon>())
        {
            weapon.enabled = active;
        }

        foreach (PlayerAim aim in
                player.GetComponentsInChildren<PlayerAim>())
        {
            aim.enabled = active;
        }

        if (!active)
        {
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();

            if (body != null)
                body.linearVelocity = Vector2.zero;
        }
    }

    public void BeginGame()
    {
        if (!isReady || IsStarted || IsFinished)
            return;

        IsStarted = true;
        IsPaused = false;
        Time.timeScale = 1f;

        SetPlayerControls(true);
        StartCoroutine(RunWaves());
    }

    private void OnDestroy()
    {
        // Tránh để scene tiếp theo vẫn bị đứng vì timeScale = 0.
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}