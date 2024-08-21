using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetBoxAll : MonoBehaviour
{
    public PlayerTarget playerTarget;
    public GameObject targetBox;
    public GameObject curTarget;
    public List<GameObject> targetList;
    public Camera mainCamera;

    public Dictionary<GameObject, GameObject> targetBoxDict; //enemy, targetBox

    // Start is called before the first frame update
    void Start()
    {
        targetBoxDict = new Dictionary<GameObject, GameObject>();
        // targetBoxDict.Add(curTarget, Instantiate(targetBox, transform));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool canSeeTarget = false;

        targetList = playerTarget.targetList;
        curTarget = playerTarget.CurTarget;

        foreach (GameObject target in targetList){
            Ray ray = new Ray(mainCamera.transform.position, target.transform.position - mainCamera.transform.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                if(hit.collider.gameObject.tag == "enemy"){
                    canSeeTarget= true;
                }
                else{
                    canSeeTarget= false;
                }
            }
            else{
                canSeeTarget= false;
            }

            if (canSeeTarget && target != curTarget && !target.GetComponent<Target>().dead){
                Vector3 targetScreenPos = TransformToHUDSpace(target.transform.position);
                if(targetScreenPos.z>0){

                    GameObject targetBoxInstance;
                    if(targetBoxDict.Count==0 || !targetBoxDict.ContainsKey(target)){
                        targetBoxInstance = Instantiate(targetBox, transform);
                        targetBoxDict.Add(target, targetBoxInstance);
                        targetBoxInstance.transform.GetChild(0).gameObject.GetComponent<Text>().text = target.GetComponent<Target>().name;
                    }
                    else{
                        targetBoxInstance = targetBoxDict[target];
                    }

                    float targetDistance = Vector3.Distance(target.transform.position, transform.position);
                    Vector3 boxPos = new Vector3(targetScreenPos.x, targetScreenPos.y, 0);
                    targetBoxInstance.GetComponent<RectTransform>().localPosition = boxPos;
                    
                    
                    targetBoxInstance.transform.GetChild(1).gameObject.GetComponent<Text>().text = string.Format("{0:0 m}", targetDistance);
                    
                }
                else{
                    if(targetBoxDict.ContainsKey(target)){
                        Destroy(targetBoxDict[target]);
                        targetBoxDict.Remove(target);
                    }
                }
            }
            else{
                if(targetBoxDict.ContainsKey(target)){
                    Destroy(targetBoxDict[target]);
                    targetBoxDict.Remove(target);
                }
            }
        }

        foreach (GameObject target in targetBoxDict.Keys){
            if(!targetList.Contains(target)){
                Destroy(targetBoxDict[target]);
                targetBoxDict.Remove(target);
            }
        }
    }

    Vector3 TransformToHUDSpace(Vector3 worldSpace) {
        var screenSpace = mainCamera.WorldToScreenPoint(worldSpace);
        return screenSpace - new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2);
    }
}
