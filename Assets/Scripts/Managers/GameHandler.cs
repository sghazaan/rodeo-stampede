using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    void Start()
    {
        gameOverPanel.SetActive(false);
    }
    void Update()
    {
        if(GameManager.IsGameLost)
        {
            gameOverPanel.SetActive(true);
        }
    }
}