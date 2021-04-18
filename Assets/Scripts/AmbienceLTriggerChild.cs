using UnityEngine;

public class AmbienceLTriggerChild : MonoBehaviour
{
    AmbienceLTrigger manager;
    string state = "none";

    private void Start()
    {
        manager = GetComponentInParent<AmbienceLTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != "enter")
        {
            manager.ComputeDirection(other.transform.position, transform.position, name, true);
            state = "enter";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (state != "exit")
        {
            manager.ComputeDirection(other.transform.position, transform.position, name, false);
            state = "exit";
        }
    }
}
