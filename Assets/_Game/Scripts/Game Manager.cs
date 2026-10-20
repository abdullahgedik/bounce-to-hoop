using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject levelWonScreen;
    [SerializeField] private GameObject player;

    private bool isGamePaused = false;
    private bool playerIsDead = false;
    private InputAction pauseAction;

    void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("Gameplay/Pause");
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            PauseGame();
        }

        if (playerIsDead)
        {
            GameOver();
        }
    }

    public void PlayerIsDead()
    {
        playerIsDead = true;
    }

    public void PauseGame()
    {
        if (!isGamePaused && !playerIsDead)
        {
            player.GetComponent<BallMovement>().GameIsPaused();
            isGamePaused = !isGamePaused;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else if (isGamePaused && !playerIsDead)
        {
            player.GetComponent<BallMovement>().GameIsPaused();
            isGamePaused = !isGamePaused;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void LevelWon()
    {
        levelWonScreen.SetActive(true);
        player.GetComponent<Health>().enabled = false;
        Destroy(player.GetComponent<Health>());
    }
}
