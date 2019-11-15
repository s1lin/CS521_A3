using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour {    

    public List<KeyValuePair<string, object>> preconditions;
    public List<KeyValuePair<string, object>> effects;

    public List<KeyValuePair<SpiceName, int>> takeout;

    public float actionDuration = 15f;
    public float cost = 1f;
    public int traderIndex = -1;

    public bool inRange = false;
    public bool inWait = false;
    public bool init = true;

    public GoapAction() {
        preconditions = new List<KeyValuePair<string, object>>();
        effects = new List<KeyValuePair<string, object>>();
        takeout = new List<KeyValuePair<SpiceName, int>>();
    }

    public void DoReset() {
        inRange = false;
        inWait = false;
        init = true;
        Reset();
    }

    public bool IsInRange() {
        return inRange;
    }

    public void AddPrecondition(string key, object value) {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void AddEffect(string key, int value) {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public abstract void Reset();

    public abstract bool IsDone();

    public abstract bool IsSucc();

    public abstract bool IsActionUsable(List<KeyValuePair<string, object>> state);

    public abstract void DoAction(GameObject agent);
}