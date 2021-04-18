using UnityEngine;
using FMODUnity;

public class AmbienceDoorAudio : StudioEventEmitterOcclusion
{
    // Start is called before the first frame update
    void Start()
    {
        fmodEvent = "event:/Door";
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        fmodInstance.start();
    }
}
