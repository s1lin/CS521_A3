using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithA : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public TradeWithA() {        

        AddPrecondition("Capacity", 2);

        AddEffect("InTu", 2);
        AddEffect("Capacity", -2);
    }

    void Start() {
        traderIndex = 0;
    }

    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        inventory.GetItemFromTrader(SpiceName.Tu, 2);

        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;

        isFinished = true;
        isSucc = true;

    }

    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("Capacity"))
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