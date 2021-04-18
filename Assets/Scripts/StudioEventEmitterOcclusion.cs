using UnityEngine;
using FMOD;
using FMODUnityResonance;

public class StudioEventEmitterOcclusion : MonoBehaviour
{

    public FMOD.Studio.EventInstance fmodInstance;

    [FMODUnity.EventRef]
    public string fmodEvent;
    public bool occlusionEnabled = true;
    string parameterName = "Occlusion";
    public float occlusionIntensity = 1.0f;
    float occlusion = 0.0f;
    float occlusionUpdate = 0.0f;

    protected void Start()
    {
        fmodInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        fmodInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        if (fmodInstance.isValid())
        {
            fmodInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));

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
