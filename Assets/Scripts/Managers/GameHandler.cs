using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }
    [Header("Main components")]
    [SerializeField] private GameObject startUI; // Assign Start Menu UI in Inspector
    [SerializeField] private GameObject gameOverUI; // Assign Game Over UI in Inspector
    [SerializeField] private Transform player; // Assign Player in Inspector
    [SerializeField] private Vector3 startPosition; // Set player's starting position
    [Header("Script Refs")]
    [SerializeField] private AnimalSpawner animalSpawner; // Assign AnimalSpawner script
    [SerializeField] private JumpLandingIndicator landingIndicator; // Assign Jump script
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        ShowStartMenu();
    }

    public void StartGame()
    {
        GameManager.CurrentState = GameManager.GameState.Playing;
        startUI.SetActive(false);
        gameOverUI.SetActive(false);

        // Reset player position and enable movement
        player.position = startPosition;
        player.gameObject.SetActive(true);
        animalSpawner.SpawnInitialAnimals();
    }

    public void GameOver()
    {
        if(GameManager.IsGameOver)
            return;
        player.GetComponent<PlayerController>().GameOverAnimation();
        GameManager.CurrentState = GameManager.GameState.GameOver;
        GameManager.IsPlayerRiding = false;
        //move player to the right + 1.5f translate
        player.position = new Vector3(player.position.x + 1.5f, player.position.y, player.position.z);
        StartCoroutine(ShowGameOverScreen());
        // gameOverUI.SetActive(true);
        //player.gameObject.SetActive(false); // Hide player
    }

    IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(3f);
        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload scene
    }

    private void ShowStartMenu()
    {
        GameManager.CurrentState = GameManager.GameState.Idle;
        startUI.SetActive(true);
        gameOverUI.SetActive(false);
        player.gameObject.SetActive(false); // Hide player until game starts
    }
}