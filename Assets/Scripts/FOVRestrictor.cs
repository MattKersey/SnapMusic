using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVRestrictor : MonoBehaviour
{
    public float minMotion = 0.001f;
    public float ratio = 120.0f / 155.0f;
    private float ofov = 155.0f;
    private float ifov = 120.0f;
    private float simulationRate = 60f;
    private float rotationScaleMultiplier;
    private float rotationInfluence;
    private OVRVignette vignette;
    private OVRPlayerController playerController;
    private CharacterController characterController;


    void Start()
    {
        playerController = GetComponent<OVRPlayerController>();
        characterController = GetComponent<CharacterController>();
        playerController.GetRotationScaleMultiplier(ref rotationScaleMultiplier);
        rotationInfluence = simulationRate * playerController.RotationAmount * rotationScaleMultiplier;
        vignette = GetComponentInChildren<OVRVignette>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 rtouch = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        float angularV = Mathf.Abs(rtouch.x * Time.deltaTime * rotationInfluence);
        float v = characterController.velocity.magnitude;
        float cRate = (angularV / 20f) + (v / 4f);
        if (cRate < minMotion)
        {
            if (ifov >= 120)
                cRate = 0f;
            else if (ifov < 80)
                cRate = -3f * Time.deltaTime;
            else
                cRate = -9f * Time.deltaTime;
        }
        else
        {
            if (ofov > 130)
                cRate = 3f * cRate;
            else if (ofov <= 80)
                cRate = 0f;
            else
                cRate = 0.5f * cRate;
        }
        ofov -= cRate;
        ifov = ofov * ratio;
        vignette.VignetteFieldOfView = ofov;
        vignette.VignetteFalloffDegrees = 0.5f * (ofov - ifov);
    }
}


