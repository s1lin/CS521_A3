using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithF : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;


    void Start() {
        target = 5;

        addPrecondition("InSa", 1);
        addPrecondition("InTu", 2);
        addPrecondition("InCi", 1);

        addEffect("InSa", -1);
        addEffect("InTu", -2);
        addEffect("InCi", -1);
        addEffect("InPe", 1);
        addEffect("Capacity", 3);

    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

        bool satisfied1 = false;
        bool satisfied2 = false;
        bool satisfied3 = false;

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu") && (int)s.Value >= 2) {
                satisfied1 = true;
            }
            if (s.Key.Equals("InSa") && (int)s.Value >= 1) {
                satisfied2 = true;
            }
            if (s.Key.Equals("InCi") && (int)s.Value >= 1) {
                satisfied3 = true;
            }
        }
        return satisfied1 && satisfied2 && satisfied3;
    }

    public override bool isDone() {
        return isFinished;
    }

    public override bool IsSucc() {
        return isSucc;
    }

    public override void perform(GameObject agent) {
        StartCoroutine(performAction(agent));
    }

    public IEnumerator performAction(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        bool succ = false;

        if (inventory.RemoveItemByQuanlity(SpiceName.Tu, 2) && inventory.RemoveItemByQuanlity(SpiceName.Sa, 1) && inventory.RemoveItemByQuanlity(SpiceName.Ci, 1)) {
            inventory.GetItemFromTrader(SpiceName.Pe, 1);
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

