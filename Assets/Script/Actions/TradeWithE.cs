using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithE : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    void Start() {
        target = 4;

        addPrecondition("InCa", 1);
        addPrecondition("InTu", 1);

        addEffect("InCa", -1);
        addEffect("InTu", -1);
        addEffect("InCl", +1);
        addEffect("Capacity", 1);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

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

    public override bool requiresInRange() {
        return true;// need to be near a trader
    }

    public override void reset() {
        isFinished = false;
        isSucc = false;
    }
}
