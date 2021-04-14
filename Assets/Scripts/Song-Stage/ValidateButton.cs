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
        Debug.Log(_biteController);
    }

    // TODO: Have a trigger command that would
    // enable `_biteController.AllInOrder`
    private void OnTriggerEnter(Collider other)
    {

        _biteController.AllInOrder();
    }
}
