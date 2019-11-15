using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithG : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;


    void Start() {
        target = 6;

        addPrecondition("InCa", 4);

        addEffect("InCa", -4);
        addEffect("InSu", 1);
        addEffect("Capacity", 3);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InCa"))
                return (int)s.Value == 4;
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

        if (inventory.RemoveItemByQuanlity(SpiceName.Ca, 4)) {
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
        return true;// need to be near a trader
    }

    public override void reset() {
        isFinished = false;
        isSucc = false;
    }
}

