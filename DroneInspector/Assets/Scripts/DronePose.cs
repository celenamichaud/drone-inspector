using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DronePose : MonoBehaviour
{
    // consider creating functions here to return the data to the serializer

    public DronePose()
    {
        
    }

    public DronePose(float[] pos, float[] dir, string name, string type)
    {
        DronePose dp = new();
        dp.transform.localPosition = new Vector3(pos[0], pos[1], pos[2]);
        // how to handle direction
        dp.name = name;
        // type will be saved into private variable
    }
}