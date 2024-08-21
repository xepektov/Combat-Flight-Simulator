using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealthInfo {
    public string colliderName;
    public float health;
    public float maxHealth;

}

public class HealthManager : MonoBehaviour
{

    [SerializeField]
    ColliderHealth[] colliderHealths;

    [SerializeField]
    float[] healths;

    [SerializeField]
    HealthInfo[] healthInfos;

    // Start is called before the first frame update
    void Start()
    {
        // colliderHealths = GetComponentsInChildren<ColliderHealth>();
        healths = new float[colliderHealths.Length];

        initializeHealthInfos();
    }

    void initializeHealthInfos(){
        healthInfos = new HealthInfo[colliderHealths.Length];
        for (int i = 0; i < colliderHealths.Length; i++){
            healthInfos[i].colliderName = colliderHealths[i].collider_name;
            healthInfos[i].health = colliderHealths[i].CurrentHealth;
            healthInfos[i].maxHealth = colliderHealths[i].MaxHealth;
        }
    }

    void ApplyColliderDamage(){

    }
}
