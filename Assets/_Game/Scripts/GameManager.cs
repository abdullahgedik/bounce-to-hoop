using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static event Action<bool> OnPauseToggle;

    [Header("References")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _levelWonScreen;

    [Header("Debug & Test")]
    [SerializeField] private bool _isTestLevel = false;

    private bool _isGamePaused = false;
    private bool _ballIsDead = false;
    private bool _isLevelWon = false;
    private InputAction _pauseAction;

    void Awake()
    {
        _pauseAction = InputSystem.actions.FindAction("Gameplay/Pause");
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        BallHealth.OnBallDeath += PlayerIsDead;
        Hoop.OnHoopScored += LevelWon;
    }

    private void OnDisable()
    {
        BallHealth.OnBallDeath -= PlayerIsDead;
        Hoop.OnHoopScored -= LevelWon;
    }

    private void Update()
    {
        if (_pauseAction.WasPressedThisFrame())
        {
            PauseGame();
        }
    }

    public void PlayerIsDead()
    {
        _ballIsDead = true;
        GameOver();
    }

    public void PauseGame()
    {
        if (_ballIsDead || _isLevelWon)
            return;

        _isGamePaused = !_isGamePaused;
        Time.timeScale = _isGamePaused ? 0f : 1f;
        _pauseMenu.SetActive(_isGamePaused);
        OnPauseToggle?.Invoke(_isGamePaused);
    }

    public void GameOver()
    {
        _gameOverMenu.SetActive(true);
    }

    public void Retry()
    {
        ReloadCurrentScene();
    }

    public void NextScene()
    {
        if (IsTestLevel())
        {
            ReloadCurrentScene();
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private bool IsTestLevel()
    {
        return _isTestLevel || SceneManager.GetActiveScene().buildIndex < 0;
    }

    private static void ReloadCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();

#if UNITY_EDITOR
        if (scene.buildIndex < 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                scene.path,
                new LoadSceneParameters(LoadSceneMode.Single));
            return;
        }
#endif

        SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LevelWon()
    {
        _isLevelWon = true;
        _levelWonScreen.SetActive(true);
        DOVirtual.DelayedCall(2f, NextScene).SetLink(gameObject);
    }
}
