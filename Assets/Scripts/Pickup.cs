
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool picked = false;
    public PlayerController player;
    public AudioSource SFXaudioSource;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        SFXaudioSource = GameObject.Find("SFX Audio Source").GetComponent<AudioSource>();
    }

    // Update is called once per frame


    public virtual void applyPickup() {}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            applyPickup();
        }
    }
}
