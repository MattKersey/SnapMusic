using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateButton : MonoBehaviour
{
    public GameObject songBitesContainer;
    private BiteController _biteController;

    private void Start()
    {
        _biteController = songBitesContainer.GetComponent<BiteController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _biteController.AllInOrder();
    }
}
