using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TradeWithA : GoapAction {

    private bool isSucc = false;

    private bool isFinished = false;

    void Start() {
        target = 0;

        addPrecondition("Capacity", 2);

        addEffect("InTu", 2);
        addEffect("Capacity", -2);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("Capacity"))
                return (int)s.Value >= 2;
        }
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
        inventory.GetItemFromTrader(SpiceName.Tu, 2);
        
        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;
        
        isFinished = true;
        isSucc = true;

    }

    public override bool requiresInRange() {
        return true;
    }

    public override void reset() {
        isFinished = false;
        isSucc = false;
    }
   
}

