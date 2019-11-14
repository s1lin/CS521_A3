using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithD : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    void Start() {
        target = 3;

        addPrecondition("InTu", 4);

        addEffect("InTu", -4);
        addEffect("InCi", 1);
        addEffect("Capacity", 3);
    }


    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu"))
                return (int)s.Value == 4;
        }
        return false;
    }

    public override bool isDone() {
        return isFinished;
    }

    public override bool perform(GameObject agent) {
        StartCoroutine(performAction(agent));
        return isSucc;
    }

    public IEnumerator performAction(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        bool succ = false;

        if (inventory.RemoveItem(SpiceName.Tu, 4)) {
            inventory.GetItemFromTrader(SpiceName.Ci, 1);
            succ = true;
        }
        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;

        isFinished = true;
        isSucc = succ;
    }

    public override bool requiresInRange() {
        return true;// need to be near a trader
    }

    public override void reset() {
        isFinished = false;
        isSucc = false;
    }
}

