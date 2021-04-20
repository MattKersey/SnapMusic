using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSounds : StudioEventEmitterOcclusion
{
    // Start is called before the first frame update
    new private void Start()
    {
        fmodInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
    }

    public void Play()
    {
        fmodInstance.start();
    }
}
