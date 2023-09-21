
using Unity.VisualScripting;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float speed;
    [SerializeField] PlayerController player;
    [SerializeField] public GameObject Target;
    private MainManager mainManager;
    private Vector3 moveVector;
    private Vector3 targetPosition;
    private bool travel = false;
    public bool shootingAtEnemy = false;

    private void Awake()
    {
        mainManager =GameObject.Find("MainManager").GetComponent<MainManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
       

    }

    public void initialize()
    {
        if (player.facingRight)
        {
            moveVector = Vector3.right;
        }
        else
        {
            moveVector = Vector3.left;
        }
        travel = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (travel)
        {
            transform.Translate(moveVector * speed * Time.deltaTime);
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && shootingAtEnemy)
        {
            Debug.Log("HitEnemy");
            collision.GetComponent<EnemyController>().takeAHit();
            mainManager.passTurn();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && !shootingAtEnemy)
        {
                collision.GetComponent<Animator>().SetTrigger("GetHit");
                mainManager.passTurn();
                Destroy(gameObject);

        }else if (collision.gameObject.CompareTag("Wall"))
        {
            mainManager.passTurn();
            Destroy(gameObject);
        }
    }
}
