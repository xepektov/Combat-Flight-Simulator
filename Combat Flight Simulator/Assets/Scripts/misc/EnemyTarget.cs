using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public GameObject[] trackedObjects;

    public List<GameObject> targetList;
    public GameObject CurTarget;

    float startTime;
    float elapsedTime;
    public float timeInterval;

    [SerializeField] GameObject Player_plane;

    // public Camera mainCamera;

    [SerializeField] float viewRange;
    Plane plane;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        // launchStartTime = Time.time;
        trackedObjects = trackTargets();
        plane = GetComponent<Plane>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        // launchElapsedTime = Time.time - launchStartTime;
        if(elapsedTime > timeInterval){
            // trackedObjects = trackTargets();
            UpdateList();
            UnityEngine.Debug.Log("update list");
            startTime = Time.time;
        }
    }

    GameObject[] trackTargets(){
        GameObject[] trackedObjects = GameObject.FindGameObjectsWithTag("friendly");
        trackedObjects = trackedObjects.Concat(GameObject.FindGameObjectsWithTag("player")).ToArray();
        
        // UnityEngine.Debug.Log(trackedObjects.Length);

        foreach (GameObject trackedObject in trackedObjects){
            Target tgt = trackedObject.GetComponent<Target>();
            tgt.OnDeath += (sender, e) => {
                RemoveTarget(e.dead_guy);
            };
        }

        return trackedObjects;
    }

    void UpdateList(){

        foreach(GameObject trackedObject in targetList){
            if(!trackedObjects.Contains(trackedObject)){
                targetList.Remove(trackedObject);
            }
        }

        foreach (GameObject trackedObject in trackedObjects){
            if(inRange(trackedObject)){
                if(!targetList.Contains(trackedObject)){
                    targetList.Add(trackedObject);
                }
            }
            else{
                if(targetList.Contains(trackedObject)){
                    targetList.Remove(trackedObject);
                }
            }
        
        }
        //get random gamobject from targetList
        if(targetList.Count > 0){
            CurTarget = targetList[Random.Range(0,targetList.Count)];
            GetComponent<Plane>().target = CurTarget.GetComponent<Target>();
        }
        else{
            CurTarget = Player_plane;
            GetComponent<Plane>().target = Player_plane.GetComponent<Target>();
        }
        UnityEngine.Debug.Log(targetList.Count);
    }

    bool inRange(GameObject target){
        if(Vector3.Distance(target.transform.position, transform.position) < viewRange){
            return true;
        }
        else{
            return false;
        }
    }

    void RemoveTarget(GameObject target){
        List<GameObject> newTrackedObjects = new List<GameObject>();
        for(int i = 0; i < trackedObjects.Length; i++){
            newTrackedObjects.Add(trackedObjects[i]);
        }
        newTrackedObjects.Remove(target);
        trackedObjects = newTrackedObjects.ToArray();
    }
}
