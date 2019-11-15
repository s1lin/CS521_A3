using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithF : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public TradeWithF() {
        traderIndex = 5;

        AddPrecondition("InSa", 1);
        AddPrecondition("InTu", 2);
        AddPrecondition("InCi", 1);

        AddEffect("InSa", -1);
        AddEffect("InTu", -2);
        AddEffect("InCi", -1);
        AddEffect("InPe", 1);
        AddEffect("Capacity", 3);
    }
    void Start() {
        traderIndex = 5;
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

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

    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {

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

    public override bool IsDone() {
        return isFinished;
    }

    public override bool IsSucc() {
        return isSucc;
    }

    public override void Reset() {
        isFinished = false;
        isSucc = false;
    }
}