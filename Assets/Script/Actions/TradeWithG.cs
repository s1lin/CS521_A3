using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithG : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;
    
    public TradeWithG() {

        AddPrecondition("InCa", 4);

        AddEffect("InCa", -4);
        AddEffect("InSu", 1);
        AddEffect("Capacity", 3);
    }

    void Start() {
        traderIndex = 6;
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

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

    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InCa"))
                return (int)s.Value == 4;
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

