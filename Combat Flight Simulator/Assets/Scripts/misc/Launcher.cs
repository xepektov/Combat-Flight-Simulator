using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
// using UnityEngine.Time;

public class Launcher : MonoBehaviour
{

    public GameObject missilePrefab;
    public GameObject[] trackedObjects;

    public List<GameObject> targetList;
    GameObject CurTarget;
    public Transform launchPos;
    public float launchSpeed;
    public float nextLaunchDelay;
    public bool isLocked;
    public bool isLoaded;
    public int launcher_Capacity;

    float startTime;
    float elapsedTime;
    public float timeInterval;

    float launchStartTime;
    float launchElapsedTime;
    public float launchTimeInterval;

    public float effectiveHeight;
    public float effectiveRange;


    // Start is called before the first frame update
    void Start()
    {
        isLoaded=true;
        isLocked=false;
        startTime = Time.time;
        launchStartTime = Time.time;
        trackedObjects = trackTargets();

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        launchElapsedTime = Time.time - launchStartTime;
        if(elapsedTime > timeInterval){
            // trackedObjects = trackTargets();
            UpdateList();
            // UnityEngine.Debug.Log("update list");
            startTime = Time.time;
        }

        if((CurTarget!=null) && inRange(CurTarget)){
            isLocked = true;
            Vector3 dir = (CurTarget.transform.position - launchPos.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);
            launchPos.rotation = Quaternion.Slerp(launchPos.rotation, rot, Time.deltaTime * 5f); 
        }
        else{
            CurTarget = null;
            isLocked = false;
        }

        if(isLocked && isLoaded && launchElapsedTime > launchTimeInterval){

            GameObject missile_ins = Instantiate(missilePrefab, launchPos.position, launchPos.rotation);
            Missile missile = missile_ins.GetComponent<Missile>();
            missile.Launch(this, CurTarget.GetComponent<Target>());
            launchStartTime = Time.time;
            launcher_Capacity -= 1;
        }

        if(launcher_Capacity == 0){
            isLoaded = false;
        }

        if(!isLoaded && launchElapsedTime > nextLaunchDelay){
            isLoaded = true;
        }



    }

    GameObject[] trackTargets(){
        // bool assigned = false;
        // if(gameObject.CompareTag("enemy")){
        GameObject[] trackedObjects = GameObject.FindGameObjectsWithTag("friendly");
        trackedObjects = trackedObjects.Concat(GameObject.FindGameObjectsWithTag("player")).ToArray();
            // UnityEngine.Debug.Log("done");
            // assigned =true;
        // }
        // else if(gameObject.CompareTag("friendly")){
        //     GameObject[] trackedObjects = GameObject.FindGameObjectsWithTag("enemy");
        //     assigned = true;
        // }
        
        foreach (GameObject trackedObject in trackedObjects){
            Target tgt = trackedObject.GetComponent<Target>();
            tgt.OnDeath += (sender, e) => {
                RemoveTarget(e.dead_guy);
            };
        }

        // UnityEngine.Debug.Log(trackedObjects.Length);
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
        }
        else{
            CurTarget = null;
        }
        // UnityEngine.Debug.Log(targetList.Count);
    }

    bool inRange(GameObject target){
        if((Vector3.Distance(target.transform.position, transform.position) < effectiveRange) && (target.transform.position.z - transform.position.z > effectiveHeight)){
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
