using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithB : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    void Start() {
        target = 1;

        addPrecondition("InTu", 2);

        addEffect("InTu", -2);
        addEffect("InSa", 1);
        addEffect("Capacity", 1);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu"))
                return (int)s.Value >= 2;
        }
        return false;
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
        
        if (inventory.RemoveItemByQuanlity(SpiceName.Tu, 2)) {
            inventory.GetItemFromTrader(SpiceName.Sa, 1);
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

