using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;

    public string collider_name;

    // public Image damageUI;//need to be set in inspector
    // public Image damageUImain;//need to be set in inspector
    public GameObject explosionPrefab;//need to be set in inspector
    public void ApplyDamage(float damage) {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) {
            Debug.Log("Destroyed " + collider_name);
            Die();
        }
        
        // CurDamagePhase = Mathf.CeilToInt(CurrentHealth / MaxHealth * 3);
        // damageUIshow();
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;   
    }

    void Die(){
        GetComponent<Target>().Die();
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

}
