using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithH : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    void Start() {

        target = 7;

        addPrecondition("InSa", 1);
        addPrecondition("InCi", 1);
        addPrecondition("InCl", 1);

        addEffect("InSa", -1);
        addEffect("InCi", -1);
        addEffect("InCl", -1);
        addEffect("InSu", 1);
        addEffect("Capacity", 2);

    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

        bool satisfied1 = false;
        bool satisfied2 = false;
        bool satisfied3 = false;

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InSa") && (int)s.Value >= 1) {
                satisfied1 = true;
            }
            if (s.Key.Equals("InCa") && (int)s.Value >= 1) {
                satisfied2 = true;
            }
            if (s.Key.Equals("InCi") && (int)s.Value >= 1) {
                satisfied3 = true;
            }
        }
        return satisfied1 && satisfied2 && satisfied3;
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
            isFinished = true;
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

