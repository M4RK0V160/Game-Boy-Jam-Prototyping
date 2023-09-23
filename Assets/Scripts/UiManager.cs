
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    private MainManager mainManager;
    public PlayerController playerController;
    [SerializeField] private Sprite[] HealthSprites;
    [SerializeField] private Sprite[] OxigenSprites;
    [SerializeField] private Sprite[] ComponentSprites;
    public GameObject[] componentObjects = new GameObject[5];

    private Image health;
    private Image Oxigen;
    private bool initialized;


    public void Awake()
    {
        mainManager = MainManager.Instance;
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
        if (playerController.HP <= 4)
        {
            if (playerController.HP > 0)
            {
                health.sprite = HealthSprites[playerController.HP];
            }
            else
            {
                playerController.HP = 0;
                health.sprite = HealthSprites[playerController.HP];
            }
            
        }
        else
        {
            playerController.HP = 4;
            health.sprite= HealthSprites[playerController.HP];
        }
        if (playerController.O2 <= 4)
        {
            if (playerController.O2 > 0)
            {
                Oxigen.sprite = OxigenSprites[playerController.O2];
            }
            else
            {
                playerController.O2 = 0;
                Oxigen.sprite = OxigenSprites[playerController.O2]; 
            }
        }
        else
        {
            playerController.HP = 4;
            Oxigen.sprite = OxigenSprites[playerController.O2];
        }


        for (int i = 0; i < 5; i++)
        {
            if (mainManager.pickedComponents[i])
            {
                GameObject.Find("Component" + (i + 1).ToString()).GetComponent<Image>().sprite = ComponentSprites[i];
            }
        }
    }

    public void startGame()
    {
        SceneManager.LoadScene("GameScene");
        
    }
}
