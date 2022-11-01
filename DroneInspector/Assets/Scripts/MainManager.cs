using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public GameObject subject; // the object of interest to the drone
    List<DronePose> poses = new List<DronePose>(); // to maintain a list of poses
    public DronePose posePrefab; // to add new poses
    // public DronePose hiddenPosePrefab; // second rendering of drone
    public Toggle addToggle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(addToggle.isOn) {
            // ray casting to add poses in selected location
            if(Input.GetMouseButtonDown(0))
            {
                // note: here is where code would be triggered to launch some small GUI to input new info
                // for this drone placement
                if(Input.mousePosition.y / Screen.height > 0.85f)
                {
                    return; // mouse click was to turn off add toggle, do not add drone
                }
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 2.0f; // some distance away from the camera
                // note: could this be a useful way of placing drones in first person view?
                Vector3 dronePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3 droneToSubject = dronePosition - subject.transform.position; // todo: check this
                string name = "DronePose " + poses.Count; // divide count by 2 if rendering 2 drones for xray visual
                AddPose(dronePosition, droneToSubject, name);
            }
        }
    }

    public void AddPose(Vector3 pos, Vector3 fwd, string name, string action = "Photo")
    {
        DronePose dp = Instantiate(posePrefab);
        dp.transform.position = pos;
        dp.transform.forward = fwd; // direction
        dp.transform.parent = subject.transform; // child of subject
        dp.name = name;
        dp.actionType = action; // default action = photo
        poses.Add(dp);

        //DronePose hiddenDP = Instantiate(hiddenPosePrefab);
        //hiddenDP.transform.position = pos;
        //hiddenDP.transform.forward = fwd;
        //hiddenDP.transform.parent = dp.transform;
        //hiddenDP.name = "Hidden" + name;
        //hiddenDP.actionType = action;
        //poses.Add(hiddenDP);
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
