
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    private MainManager mainManager;
    public PlayerController playerController;
    [SerializeField] private Sprite[] HealthSprites;
    [SerializeField] private Sprite[] OxigenSprites;

    private Image health;
    private Image Oxigen;
    private bool initialized;


    public void Awake()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>().Instance;
        initialized = false;
        
    }

    public void initialize()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        health = GameObject.Find("Health").GetComponent<Image>();
        Oxigen = GameObject.Find("Oxigen").GetComponent<Image>();
        initialized = true;
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && initialized)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        health.sprite = HealthSprites[playerController.HP];
        Oxigen.sprite = OxigenSprites[playerController.O2];
    }

    public void startGame()
    {
        SceneManager.LoadScene("GameScene");
        
    }
}
