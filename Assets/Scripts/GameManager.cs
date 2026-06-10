using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;              // TextMeshPro — required in Unity 2022+ and Unity 6
// If you do NOT have TextMeshPro installed, replace TMPro.TextMeshProUGUI
// with UnityEngine.UI.Text  and change "using TMPro" to "using UnityEngine.UI"

public class GameManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────
    // Any script in the project can call GameManager.instance.AddScore()
    // without needing a reference, as long as ONE GameManager is in the scene
    public static GameManager instance;

    // ── Game state ───────────────────────────────────────────────
    [Header("Game State")]
    public int score       = 0;
    public int lives       = 3;
    public int maxTargets  = 4;  // Maximum targets allowed on screen at once

    // ── UI References — drag TextMeshPro Text objects here ───────
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject       gameOverPanel;  // Optional: a UI panel to show on death

    // ── Spawning ─────────────────────────────────────────────────
    [Header("Spawning")]
    public GameObject  targetPrefab;   // Drag the Target prefab here
    public Transform[] spawnPoints;    // Drag empty GameObjects as spawn locations

    private int activeTargets = 0;      // Track how many are alive

    // ─────────────────────────────────────────────────────────────

    void Awake()
    {
        // ✅ FIX: Reset timeScale here in case we came from a Game Over state
        Time.timeScale = 1f;

        // Singleton setup
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Warn if prefab is missing — prevents silent crash on spawn
        if (targetPrefab == null)
        {
            Debug.LogError("[GameManager] targetPrefab is not assigned! Drag the Target prefab in.");
            return;
        }

        // Hide game over panel if assigned
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Spawn the initial set of targets
        SpawnInitialTargets();
        UpdateUI();
    }

    // Called from Bullet.cs when a target is hit ─────────────────
    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    // Called when the player takes damage ────────────────────────
    public void LoseLife()
    {
        lives--;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayPlayerHit();

        UpdateUI();

        if (lives <= 0)
            GameOver();
    }

    // Called from Bullet.cs after a target is destroyed ──────────
    // delay: seconds to wait before spawning the replacement
    public void SpawnTarget(float delay = 0f)
    {
        activeTargets = Mathf.Max(0, activeTargets - 1);
        if (activeTargets < maxTargets)
            StartCoroutine(SpawnAfterDelay(delay));
    }

    // Called when a target collides with the player.
    // Handles life loss, target cleanup, and replacement spawn in one place.
    public void HandleTargetPlayerCollision(GameObject target, float respawnDelay = 1f)
    {
        LoseLife();

        if (target != null)
            Destroy(target);

        SpawnTarget(respawnDelay);
    }

    public void RestartGame()
    {
        // Reset timeScale before reloading — otherwise the game stays paused
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Private helpers ──────────────────────────────────────────

    void SpawnInitialTargets()
    {
        int count = (spawnPoints != null && spawnPoints.Length > 0)
            ? Mathf.Min(maxTargets, spawnPoints.Length)
            : maxTargets;

        for (int i = 0; i < count; i++)
            SpawnTargetAt(i);
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (targetPrefab == null)
        {
            Debug.LogError("[GameManager] targetPrefab is missing or destroyed. Cannot spawn target.");
            yield break;
        }

        // Pick a random spawn point; fall back to world origin if none set
        Vector3 pos;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            pos = spawnPoints[idx].position;
        }
        else
        {
            // ✅ FIX: Fallback spawn so game still works with no spawn points set
            float rx = Random.Range(-8f, 8f);
            float rz = Random.Range(-8f, 8f);
            pos = new Vector3(rx, 0.5f, rz);
        }

        Instantiate(targetPrefab, pos, Quaternion.identity);
        activeTargets++;
    }

    void SpawnTargetAt(int index)
    {
        if (targetPrefab == null)
        {
            Debug.LogError("[GameManager] targetPrefab is missing or destroyed. Cannot spawn target.");
            return;
        }

        Vector3 pos;
        if (spawnPoints != null && index < spawnPoints.Length)
            pos = spawnPoints[index].position;
        else
            pos = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));

        Instantiate(targetPrefab, pos, Quaternion.identity);
        activeTargets++;
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (livesText != null) livesText.text = "Lives: " + lives;
    }

    void GameOver()
    {
        Debug.Log("[GameManager] Game Over — Final score: " + score);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopBallMove();
            AudioManager.instance.PlayGameOver();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}

// ── GAMEMANAGER SETUP CHECKLIST ──────────────────────────────
// 1. GameObject → Create Empty → name it "GameManager"
// 2. Attach GameManager.cs to it
// 3. In Inspector, drag in:
//    • targetPrefab  → your Target prefab from Assets/Prefabs
//    • spawnPoints   → drag 4–6 empty GameObjects placed around the arena
//    • scoreText     → a TextMeshProUGUI object in your Canvas
//    • livesText     → a TextMeshProUGUI object in your Canvas
// 4. If TextMeshPro is not installed:
//    Window → Package Manager → search "TextMeshPro" → Install
//    Then import TMP Essentials when prompted
// ─────────────────────────────────────────────────────────────