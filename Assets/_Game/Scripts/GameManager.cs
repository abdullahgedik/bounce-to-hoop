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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
