using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddPose : MonoBehaviour
{
    public InputActionReference buttonA = null;

    private void Awake()
    {
        buttonA.action.performed += AddPosePressed;
    }

    private void AddPosePressed(InputAction.CallbackContext context)
    {
        // add the drone pose
    }
}
