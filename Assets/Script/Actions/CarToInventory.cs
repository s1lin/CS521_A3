using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CarToInventory : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
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

        if (inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1) && inventory.RemoveItem(SpiceName.Cl, 1)) {
            inventory.GetItemFromTrader(SpiceName.Su, 1);
            succ = true;
        }

        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;

        isFinished = true;
        isSucc = succ;
    }

    public override bool requiresInRange() {
        return true;
    }

    public override void reset() {
        isFinished = false;
        isSucc = false;
    }
}

