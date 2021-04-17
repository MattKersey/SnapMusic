﻿using UnityEngine;
using FMODUnity;

public class BiteAudio : StudioEventEmitterOcclusion
{
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

    public void SetDirection(int direction)
    {
        fmodInstance.setParameterByName("Direction", direction);
    }
}
