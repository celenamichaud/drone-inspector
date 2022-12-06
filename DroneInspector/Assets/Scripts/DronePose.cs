using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DronePose : MonoBehaviour
{
    public string actionType;
    //public InputActionReference buttonB = null; // button used to delete a pose
    //private void Awake()
    //{ 
    //    buttonB.action.performed += DeletePose;
    //}
    //public void DeletePose(InputAction.CallbackContext context)
    //{
    //    if (!isActiveAndEnabled)
    //    { 
    //        Destroy(gameObject);
    //    }
    //}

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}