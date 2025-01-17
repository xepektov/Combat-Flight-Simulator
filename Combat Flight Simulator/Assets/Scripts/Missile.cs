﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [SerializeField]
    float lifetime;
    [SerializeField]
    float speed;
    [SerializeField]
    float trackingAngle;
    [SerializeField]
    float damage;
    [SerializeField]
    float damageRadius;
    [SerializeField]
    float turningGForce;
    [SerializeField]
    LayerMask collisionMask;
    [SerializeField]
    new MeshRenderer renderer;
    [SerializeField]
    GameObject explosionGraphic;

    Plane owner;
    Launcher launcher;
    Target target;
    bool exploded;
    Vector3 lastPosition;
    float timer;

    public Rigidbody Rigidbody { get; private set; }

    public void Launch(Plane owner, Target target) {
        this.owner = owner;
        this.target = target;

        Rigidbody = GetComponent<Rigidbody>();

        lastPosition = Rigidbody.position;
        timer = lifetime;

        if (target!= null) target.NotifyMissileLaunched(this, true);
    }

    public void Launch(Launcher owner, Target target) {
        this.launcher = owner;
        this.target = target;

        Rigidbody = GetComponent<Rigidbody>();

        lastPosition = Rigidbody.position;
        timer = lifetime;

        if (target!= null) target.NotifyMissileLaunched(this, true);
    }

    void Explode() {
        if (exploded) return;

        timer = lifetime;
        Rigidbody.isKinematic = true;
        renderer.enabled = false;
        exploded = true;
        explosionGraphic.SetActive(true);

        var hits = Physics.OverlapSphere(Rigidbody.position, damageRadius, collisionMask.value);

        List<float> dist = new List<float>();

        foreach (var hit in hits) {
            Plane other = hit.gameObject.GetComponent<Plane>();
            // Debug.Log(hit.gameObject.name);
            // Debug.Log(Vector3.Distance(transform.position,hit.gameObject.transform.position));
            dist.Add(Vector3.Distance(transform.position,hit.gameObject.transform.position));
            if(other != null && other != owner) {
                other.ApplyDamage(damage);
            }
        }

        float trunc=20f;

        foreach (float d in dist) {
            trunc = Mathf.Min(trunc,Mathf.Floor(d));
        }
        float norm = 0f;
        for(int i=0;i<dist.Count;i++){
            dist[i] = dist[i]-(trunc-0.7f);
            dist[i] = 1/dist[i];
            norm += dist[i]*dist[i];
        }
        norm = Mathf.Sqrt(norm);
        for(int i=0;i<dist.Count;i++){
            dist[i] = dist[i]/norm;
        }

        for(int i=0;i<dist.Count;i++){
            if(hits[i].gameObject.GetComponent<ColliderHealth>()!=null){
                hits[i].gameObject.GetComponent<ColliderHealth>().ApplyDamage(dist[i]*damage);
                // Debug.Log(hits[i].gameObject.GetComponent<ColliderHealth>().collider_name);
                // Debug.Log(dist[i]*damage);
            }
            if(hits[i].gameObject.GetComponent<EnemyHealth>()!=null){
                hits[i].gameObject.GetComponent<EnemyHealth>().ApplyDamage(dist[i]*damage);
                // Debug.Log(hits[i].gameObject.GetComponent<ColliderHealth>().collider_name);
                Debug.Log(dist[i]*damage);
            }
            
        }

        if (target != null) target.NotifyMissileLaunched(this, false);
    }

    void CheckCollision() {
        //missile can travel very fast, collision may not be detected by physics system
        //use raycasts to check for collisions

        var currentPosition = Rigidbody.position;
        var error = currentPosition - lastPosition;
        var ray = new Ray(lastPosition, error.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, error.magnitude, collisionMask.value)) {
            Plane other = hit.collider.gameObject.GetComponent<Plane>();

            if (other == null || other != owner) {
                Rigidbody.position = hit.point;
                Explode();
            }
        }

        lastPosition = currentPosition;
    }

    void TrackTarget(float dt) {
        if (target == null) return;

        var targetPosition = Utilities.FirstOrderIntercept(Rigidbody.position, Vector3.zero, speed, target.Position, target.Velocity);

        var error = targetPosition - Rigidbody.position;
        var targetDir = error.normalized;
        var currentDir = Rigidbody.rotation * Vector3.forward;

        //if angle to target is too large, explode
        if (Vector3.Angle(currentDir, targetDir) > trackingAngle) {
            Explode();
            return;
        }

        //calculate turning rate from G Force and speed
        float maxTurnRate = (turningGForce * 9.81f) / speed;  //radians / s
        var dir = Vector3.RotateTowards(currentDir, targetDir, maxTurnRate * dt, 0);

        Rigidbody.rotation = Quaternion.LookRotation(dir);
    }

    void FixedUpdate() {
        timer = Mathf.Max(0, timer - Time.fixedDeltaTime);

        //explode missile automatically after lifetime ends
        //timer is reused to keep missile graphics alive after explosion
        if (timer == 0) {
            if (exploded) {
                Destroy(gameObject);
            } else {
                Explode();
            }
        }

        if (exploded) return;

        CheckCollision();
        TrackTarget(Time.fixedDeltaTime);

        //set speed to direction of travel
        Rigidbody.velocity = Rigidbody.rotation * new Vector3(0, 0, speed);
    }
}
