using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float countdownTime = 30f;
    private bool gameActive = true;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameStatusText;

    public PlayerMovement playerMovement;
    public float fallTimeLimit = 4f;
    private float fallTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameActive) return;

        countdownTime -= Time.deltaTime;
        UpdateTimerUI();

        if (countdownTime <= 0f)
        {
            GameOver();
        }

        CheckIfPlayerFalling();
    }

    public void GameWon()
    {
        ShowGameStatus("Win!");
    }

    public void GameOver()
    {
        ShowGameStatus("Game Over");
    }

    private void ShowGameStatus(string message)
    {
        gameActive = false;
        Time.timeScale = 0f;
        gameStatusText.text = message;
        StartCoroutine(RestartAfterDelay(2f));
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.Ceil(countdownTime) + "s";
        }
    }

    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        RestartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CheckIfPlayerFalling()
    {
        if (playerMovement != null && !playerMovement.isGrounded)
        {
            fallTimer += Time.deltaTime;

            if (fallTimer >= fallTimeLimit)
            {
                GameOver();
            }
        }
        else
        {
            fallTimer = 0f;
        }
    }
}
