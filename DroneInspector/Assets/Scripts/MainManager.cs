using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    List<DronePose> poses = new List<DronePose>(); // to maintain a list of poses
    public DronePose posePrefab; // to add new poses
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPose(Vector3 pos, Vector3 fwd, string name, string action = "Photo")
    {
        // todo: modify to take input from mouse click? or some other game object
        // todo: how to integrate default args in c#
        DronePose dp = Instantiate(posePrefab);
        dp.transform.position = pos;
        dp.transform.forward = fwd; // direction
        dp.name = name;
        dp.actionType = action; // default action = photo
        poses.Add(dp);
    }

    public List<DronePose> GetPoses()
    {
        return poses;
    }

    public void ClearPoses()
    {
        foreach (DronePose dp in poses)
        {
            Destroy(dp.gameObject);
        }
        poses.Clear();
    }
}
