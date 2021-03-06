using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using FMODUnityResonance;

public class StudioEventEmitterOcclusion : MonoBehaviour
{

    public EventInstance fmodInstance;

    [EventRef]
    public string fmodEvent;
    public bool occlusionEnabled = true;
    string parameterName = "Occlusion";
    public float occlusionDepth = 1.0f;
    float occlusion = 0.0f;
    float occlusionUpdate = 0.0f;
    public bool playOnStartup;

    private void Start()
    {
        if (playOnStartup)
        {
            fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
            RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
            fmodInstance.start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Reference: https://alessandrofama.com/tutorials/fmod-unity/occlusion/
        if (fmodInstance.isValid())
        {
            fmodInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            if (!occlusionEnabled)
                occlusion = 0.0f;

            else if (Time.time >= occlusionUpdate)
            {
                occlusionUpdate = Time.time + FmodResonanceAudio.occlusionDetectionRate;
                occlusion = occlusionDepth * FmodResonanceAudio.DynamicOcclusion(transform);
                fmodInstance.setParameterByName(parameterName, occlusion);
            }
        }
    }

    public void StopAudio()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
