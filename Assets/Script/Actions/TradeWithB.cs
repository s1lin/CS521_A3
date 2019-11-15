using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithB : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;
 
    public TradeWithB() {

        AddPrecondition("InTu", 2);

        AddEffect("InTu", -2);
        AddEffect("InSa", 1);
        AddEffect("Capacity", 1);
    }
    void Start() {
        traderIndex = 1;
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

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

    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu"))
                return (int)s.Value >= 2;
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
    }
}

