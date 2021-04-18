using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSound : StudioEventEmitterOcclusion
{
    // Start is called before the first frame update
    new void Start()
    {
        fmodInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
    }

    public void Play()
    {
        fmodInstance.start();
    }
}
