using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CarToInventory : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public CarToInventory() {
        AddEffect("Capacity", 0);
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        bool succ = false;

        foreach (KeyValuePair<SpiceName, int> value in takeout) {
            inventory.GetItemFromCaravan(value.Key, value.Value);
            succ = true;
        }

        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;

        isFinished = true;
        isSucc = succ;
    }

    //Will not be used unless there are an infinite loop
    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {
        if ((int)state.Find(e => e.Key.Equals("Capacity")).Value != 4)
            return false;
        foreach (KeyValuePair<string, object>s in state) {
            if (s.Key.Contains("Ca")) {
                if ((int)s.Value > 8)
                    return true;
            }
        }
        return false;
    }

    public override bool IsDone() {
        return isFinished;
    }

    public override bool IsSucc() {
        return isSucc;
    }

    public override void Reset() {
        isFinished = false;
        isSucc = false;
        takeout = new List<KeyValuePair<SpiceName, int>>();
    }
}

