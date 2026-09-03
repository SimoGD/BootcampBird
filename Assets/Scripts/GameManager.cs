using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private BirdController birdController;
    public UnityEvent GameStartedEvent;
    private bool _gameStarted = false;
    void OnEnable()
    {
        birdController.CollisionEvent.AddListener(StopGame);

        Time.timeScale = 0f;
    }
    private void OnDisable()
    {
        birdController.CollisionEvent.RemoveListener(StopGame);
    }

    private void Update()
    {
        if (!_gameStarted && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0.0f;
    }


    private void StartGame()
    {
        _gameStarted = true;
        Time.timeScale = 1.0f;

        GameStartedEvent.Invoke();
    }

    public void ResetGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Game");
    }
}
