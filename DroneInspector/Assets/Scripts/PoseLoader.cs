using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
public class PoseLoader : MonoBehaviour
{
    public MainManager mainManager;
    public GameObject subject; // the object of interest to the drone

    public class PoseJSON
	{
        public float[] position;
        public float[] direction;
        public string name;
        public string actionType;
        // create variables to hold drone camera position, and photo subject
	}

    public class PathJSON
	{
        public List<PoseJSON> poses;
	}

    
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.S))
		{
            save();
		}
        if (Input.GetKeyDown(KeyCode.L))
		{
            load();
		}
        
    }

    public void save()
	{
        PathJSON path = new PathJSON();
        path.poses = new List<PoseJSON>();
        List<DronePose> poses = mainManager.GetPoses();
        
        foreach (DronePose dp in poses)
		{
            PoseJSON pj = new PoseJSON();

            Vector3 v = dp.transform.position;
            pj.position = new float[3] { v.x, v.y, v.z };
            
            Vector3 d = dp.transform.forward;
            pj.direction = new float[3] { d.x, d.y, d.z };

            pj.name = dp.name;
            
            pj.actionType = "photo";
            
            path.poses.Add(pj);
        }

        File.WriteAllText(Application.streamingAssetsPath + "/save.json", JsonConvert.SerializeObject(path));
    }
	public void load() 
    {
        mainManager.ClearPoses();
        string json = File.ReadAllText(Application.streamingAssetsPath + "/save.json");
        PathJSON path = JsonConvert.DeserializeObject<PathJSON>(json);
        foreach(PoseJSON pj in path.poses) {
            Vector3 pos = new(pj.position[0], pj.position[1], pj.position[2]);
            Vector3 fwd = new(pj.direction[0], pj.direction[1], pj.direction[2]);
            string name = pj.name;
            string action = pj.actionType;
            mainManager.AddPose(pos, fwd, name, action);
        }
    }
}
