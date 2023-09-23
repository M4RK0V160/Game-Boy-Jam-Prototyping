using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentController : Pickup
{

    public int ID;
    private MainManager mainManager;
    public AudioClip componentAudio;
    public override void applyPickup()
    {
        SFXaudioSource.PlayOneShot(componentAudio);
        mainManager = MainManager.Instance;
        mainManager.pickedComponents[ID-1] = true;
        Destroy(gameObject);
    }
}
