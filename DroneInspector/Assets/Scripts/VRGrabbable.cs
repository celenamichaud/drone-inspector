using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrabbable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity; // grabbable object needs unlimited velocity

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
