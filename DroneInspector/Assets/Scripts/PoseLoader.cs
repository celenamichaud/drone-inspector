using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
public class PoseLoader : MonoBehaviour
{
    List<DronePose> poses = new List<DronePose>();
    public DronePose posePrefab;
    public class PoseJSON
	{
        public float[] position;
        public float[] direction;
        public string name;
        public string type;
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
		if (Input.GetKeyDown(KeyCode.Space))
		{
            save();
		}
        if (Input.GetKeyDown(KeyCode.L))
		{
            load();
		}
        
    }

    public void addPose(DronePose t)
	{
        poses.Add(t);
	}

    public void save()
	{

        PathJSON path = new PathJSON();
        path.poses = new List<PoseJSON>();
        foreach(DronePose dp in poses)
		{
            PoseJSON pj = new PoseJSON();
            Vector3 v = dp.transform.position;
            pj.position = new float[3] { v.x, v.y, v.z };
            Vector3 d = dp.transform.forward;
            pj.direction = new float[3] { d.x, d.y, d.z };

            pj.name = dp.name;
            pj.type = "photo";
            path.poses.Add(pj);
        }


        File.WriteAllText(Application.streamingAssetsPath + "/save.json", JsonConvert.SerializeObject(path));
    }
	public void load() {
        
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
            this.poses.Add(dp);
        }
        //instantiate objects
    }
}
