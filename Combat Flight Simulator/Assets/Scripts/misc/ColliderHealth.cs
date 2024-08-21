using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderHealth : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;

    public string collider_name;

    public Image damageUI;//need to be set in inspector
    // public Image damageUImain;//need to be set in inspector
    public GameObject explosionPrefab;//need to be set in inspector
    public Material damageMaterial;

    public int CurDamagePhase;

    public Color emissionGreen;
    public Color emissionYellow;
    public Color emissionRed;

    Color emissionColor;

    // Color[] damageColor;

    // HealthManager healthManager;
    public void ApplyDamage(float damage) {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) {
            Debug.Log("Destroyed " + collider_name);
        }

        CurDamagePhase = Mathf.CeilToInt(CurrentHealth / MaxHealth * 3);
        damageUIshow();
    }

    // Start is called before the first frame update
    void Start()
    {
        damageMaterial = damageUI.material;
        // healthManager = GetComponentInParent<HealthManager>();
        CurrentHealth = MaxHealth;
        CurDamagePhase = 3;
        damageUIshow();
    }

    void damageUIshow(){
        if (CurDamagePhase == 3){
            damageMaterial.color = Color.green;
            emissionColor = emissionGreen;
        }
        else if (CurDamagePhase == 2){
            damageMaterial.color = Color.yellow;
            emissionColor = emissionYellow;
        }
        else if (CurDamagePhase == 1){
            damageMaterial.color = Color.red;
            emissionColor = emissionRed;
        }

        damageMaterial.SetColor("_ColorG", damageMaterial.color);
        damageMaterial.SetColor("_ColorR", damageMaterial.color);
        damageMaterial.SetColor("_ColorB", emissionColor);
    }


}
