// bron: https://docs.unity3d.com/6000.2/Documentation/Manual/UIE-get-started-with-runtime-ui.html
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleRuntimeUI : MonoBehaviour
{
    int uiVersion = 0;
    Button _button;
    Label _scoreLabel;
    Label _startLabel;
    Label _finalScoreLabel;
    VisualElement _gameOverVisualElement;
    int _scoreValue = 0;
    [SerializeField] private BirdController birdController;
    [SerializeField] private GameManager gameManager;

    void OnEnable()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(OnUIReload);
        birdController.IncreaseScoreEvent.AddListener(SetScore);
        birdController.CollisionEvent.AddListener(OnGameOver);

        gameManager.GameStartedEvent.AddListener(OnGameStarted);
    }
    
    void OnDestroy()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(OnUIReload);
        birdController.IncreaseScoreEvent.RemoveListener(SetScore);
        birdController.CollisionEvent.RemoveListener(OnGameOver);
        _button.clicked -= OnRestartButtonClicked;
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        // The version number only changes when the UI actually reloads, 
        // so this checks prevents duplicated elements.
        if (uiVersion == version)
            return;

        uiVersion = version;

        _scoreLabel = root.Q<Label>("scoreLabel");
        _startLabel = root.Q<Label>("startLabel");
        _finalScoreLabel = root.Q<Label>("finalScoreLabel");

        _gameOverVisualElement = root.Q<VisualElement>("gameOverVisualElement");
        _button = _gameOverVisualElement.Q<Button>("restartButton");

        _gameOverVisualElement.style.display = DisplayStyle.None;
        _scoreLabel.style.display = DisplayStyle.None;
        _startLabel.style.display = DisplayStyle.Flex;

        _button.clicked += OnRestartButtonClicked;
    }

    private void OnGameOver()
    {
        _scoreLabel.style.display = DisplayStyle.None;
        _finalScoreLabel.text = $"Game over!\nYour final score is: {_scoreValue}";
        _gameOverVisualElement.style.display = DisplayStyle.Flex;
    }

    private void OnRestartButtonClicked()
    {
        gameManager.ResetGame();
    }

    public void OnGameStarted()
    {
        _startLabel.style.display = DisplayStyle.None;
        _scoreLabel.style.display = DisplayStyle.Flex;
    }

    public void SetScore()
    {
        _scoreValue += 1;
        _scoreLabel.text = $"{_scoreValue}";
    }
}
