using System.Collections;
using UnityEngine;

public class O2 : Pickup
{
    // Start is called before the first frame update

    [SerializeField] AudioClip O2Audio;
      public override void applyPickup()
    {
        player.gameObject.GetComponent<Animator>().SetTrigger("PickUp");
        SFXaudioSource.PlayOneShot(O2Audio);
        StartCoroutine(wait());
       

    }

    public IEnumerator wait()
    {
        
       
        yield return new WaitForSeconds(0.2f);
        
        if(player.O2 < 4)
        {
            player.O2++;
            yield return wait();
        }
        Destroy(gameObject);
       
    }
}
