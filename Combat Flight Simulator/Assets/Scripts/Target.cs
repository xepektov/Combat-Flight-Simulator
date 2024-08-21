using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public event EventHandler<EventArgs> OnDeath;
    public class EventArgs : System.EventArgs {
        public GameObject dead_guy;
    }

    public void Die() {
        if (OnDeath != null) {
            OnDeath(this, new EventArgs { dead_guy = this.gameObject });
        }
        dead = true;
    }


    [SerializeField]
    new string name;

    public string Name {
        get {
            return name;
        }
    }

    public Vector3 Position {
        get {
            return rigidbody.position;
        }
    }

    public Vector3 Velocity {
        get {
            return rigidbody.velocity;
        }
    }

    public Plane Plane { get; private set; }
    public Gunner Gunner { get; private set; }
    public Launcher Launcher { get; private set; }

    public bool dead;

    new Rigidbody rigidbody;

    List<Missile> incomingMissiles;
    const float sortInterval = 0.5f;
    float sortTimer;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        dead = false;
        if(TryGetComponent<Plane>(out var plane)) {
            Plane = plane;
        }
        if(TryGetComponent<Gunner>(out var gunner)) {
            Gunner = gunner;
        }
        if(TryGetComponent<Launcher>(out var launcher)) {
            Launcher = launcher;
        }
        // Plane = GetComponent<Plane>();

        incomingMissiles = new List<Missile>();
    }

    void FixedUpdate() {
        sortTimer = Mathf.Max(0, sortTimer - Time.fixedDeltaTime);

        if (sortTimer == 0) {
            SortIncomingMissiles();
            sortTimer = sortInterval;
        }
    }

    void SortIncomingMissiles() {
        var position = Position;

        if (incomingMissiles.Count > 0) {
            incomingMissiles.Sort((Missile a, Missile b) => {
                var distA = Vector3.Distance(a.Rigidbody.position, position);
                var distB = Vector3.Distance(b.Rigidbody.position, position);
                return distA.CompareTo(distB);
            });
        }
    }

    public Missile GetIncomingMissile() {
        if (incomingMissiles.Count > 0) {
            return incomingMissiles[0];
        }

        return null;
    }

    public void NotifyMissileLaunched(Missile missile, bool value) {
        if (value) {
            incomingMissiles.Add(missile);
            SortIncomingMissiles();
        } else {
            incomingMissiles.Remove(missile);
        }
    }
}
