using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using FMODUnityResonance;

public class StudioEventEmitterOcclusion : MonoBehaviour
{

    private EventInstance fmodInstance;

    [EventRef]
    public string fmodEvent;
    public bool occlusionEnabled = true;
    string parameterName = "Occlusion";
    public float occlusionIntensity = 1.0f;
    float occlusion = 0.0f;
    float occlusionUpdate = 0.0f;

    public void LoadBite(int biteNum)
    {
        Debug.Log("Loading Bite");
        fmodEvent = "event:/Bite " + biteNum.ToString();
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        fmodInstance.start();
    }

    public void PlayBite()
    {
        fmodInstance.start();
    }

    public void StopBite()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    // Update is called once per frame
    void Update()
    {
        if (fmodInstance.isValid())
        {
            fmodInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            if (!occlusionEnabled)
            {
                occlusion = 0.0f;
            }

            else if (Time.time >= occlusionUpdate)
            {
                occlusionUpdate = Time.time + FmodResonanceAudio.occlusionDetectionInterval;
                occlusion = occlusionIntensity * FmodResonanceAudio.ComputeOcclusion(transform);
                fmodInstance.setParameterByName(parameterName, occlusion);
            }
        }
    }
}
