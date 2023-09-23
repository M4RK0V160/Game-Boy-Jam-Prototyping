using System.Collections;
using UnityEngine;

public class Health : Pickup
{
    // Start is called before the first frame update

    [SerializeField] AudioClip HPAudio;
    public override void applyPickup()
    {
        player.gameObject.GetComponent<Animator>().SetTrigger("PickUp");
        SFXaudioSource.PlayOneShot(HPAudio);
        StartCoroutine(wait());


    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(0.2f);

        if (player.HP < 4)
        {
            player.HP++;
            yield return wait();
        }
        Destroy(gameObject);

    }
}
