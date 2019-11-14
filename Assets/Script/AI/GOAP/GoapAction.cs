
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class GoapAction : MonoBehaviour {
    
    private List<KeyValuePair<string, object>> preconditions;
    private List<KeyValuePair<string, object>> effects;

    private bool inRange = false;

    public float actionDuration = 20f;//about 1s
    public float cost = 1f;
    public int target = -1;
    public bool inWait = false;

    public GoapAction() {
        preconditions = new List<KeyValuePair<string, object>>();
        effects = new List<KeyValuePair<string, object>>();
    }

    public void doReset() {
        inRange = false;
        inWait = false;
        reset();
    }

    public bool isInRange() {
        return inRange;
    }

    public void setInRange(bool inRange) {
        this.inRange = inRange;
    }

    public void addPrecondition(string key, object value) {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void removePrecondition(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in preconditions) {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            preconditions.Remove(remove);
    }

    public void addEffect(string key, int value) {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public void removeEffect(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in effects) {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }

    public List<KeyValuePair<string, object>> Preconditions {
        get {
            return preconditions;
        }
    }

    public List<KeyValuePair<string, object>> Effects {
        get {
            return effects;
        }
    }

    public abstract void reset();

    public abstract bool isDone();

    public abstract bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state);

    public abstract bool perform(GameObject agent);

    public abstract bool requiresInRange();




}