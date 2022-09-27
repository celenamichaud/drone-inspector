using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
public class PoseLoader : MonoBehaviour
{
    List<DronePose> poses = new List<DronePose>();
    public DronePose posePrefab;
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
        poses = new List<DronePose>(GameObject.FindObjectsOfType<DronePose>());
        
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
        DronePose[] poses = GameObject.FindObjectsOfType<DronePose>();
        foreach(DronePose dp in poses)
		{
            Destroy(dp.gameObject);
		}
        this.poses.Clear();
        string json = File.ReadAllText(Application.streamingAssetsPath + "/save.json");
        PathJSON path = JsonConvert.DeserializeObject<PathJSON>(json);
        foreach(PoseJSON pj in path.poses) {
            DronePose dp =  Instantiate(posePrefab);
            dp.transform.position = new Vector3(pj.position[0], pj.position[1], pj.position[2]);
            dp.transform.forward = new Vector3(pj.direction[0], pj.direction[1], pj.direction[2]);
            dp.name = pj.name;
            dp.actionType = pj.actionType;
            this.poses.Add(dp);
        }
    }
}
