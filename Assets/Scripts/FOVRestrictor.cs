using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVRestrictor : MonoBehaviour
{
    public float minMotion = 0.001f;
    public float ratio = 120.0f / 155.0f;
    public float angularCoefficient = 5.0f;
    public float linearCoefficient = 1.0f;
    public float maxFov = 155.0f;
    public float minFov = 80.0f;
    private float ofov;
    private float ifov;
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
        ofov = maxFov;
        ifov = (84.0f / 75.0f) * (ofov - 155.0f) + 120.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 rtouch = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        float angularV = Mathf.Abs(rtouch.x * Time.deltaTime * rotationInfluence);
        float v = characterController.velocity.magnitude;
        float cRate = (angularCoefficient * angularV / 20f) + (linearCoefficient * v / 4f);
        if (cRate < minMotion)
        {
            if (ofov >= maxFov)
                cRate = 0f;
            else if (ifov < 80)
                cRate = -3f * Time.deltaTime;
            else
                cRate = -9f * Time.deltaTime;
        }
        else
        {
            if (ofov > 110)
                cRate = 3f * cRate;
            else if (ofov <= minFov)
                cRate = 0f;
            else
                cRate = 0.5f * cRate;
        }
        ofov -= cRate;
        ifov = (84.0f / 75.0f) * (ofov - 155.0f) + 120.0f;
        Debug.Log("Abcd123456 " + ofov + " " + vignette.VignetteFalloffDegrees);
        // if ofov = 80, ifov = 36
        // if ofov = 155, ifov = 120
        // (120 - 36) / (155 - 80) (ofov - 155) + 120
        // 84 / 
        vignette.VignetteFieldOfView = ofov;
        vignette.VignetteFalloffDegrees = 0.5f * (ofov - ifov);
    }
}


