using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithE : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public TradeWithE() {

        AddPrecondition("InCa", 1);
        AddPrecondition("InTu", 1);

        AddEffect("InCa", -1);
        AddEffect("InTu", -1);
        AddEffect("InCl", +1);
        AddEffect("Capacity", 1);
    }

    void Start() {
        traderIndex = 4;
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        bool succ = false;

        if (inventory.RemoveItemByQuanlity(SpiceName.Ca, 2) && inventory.RemoveItemByQuanlity(SpiceName.Tu, 1)) {
            inventory.GetItemFromTrader(SpiceName.Cl, 1);
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

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu") && (int)s.Value >= 1) {
                satisfied1 = true;
            }
            if (s.Key.Equals("InCa") && (int)s.Value >= 2) {
                satisfied2 = true;
            }
        }
        return satisfied1 && satisfied2;
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
