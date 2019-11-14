using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CarToInventory : GoapAction {

    private float startTime = 0;
    private bool isFinished = false;

    public float tradeDuration = 0.5f; // seconds

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        return false;
    }

    public override bool isDone() {
        return isFinished;
    }

    public override bool perform(GameObject agent) {
        Caravan caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();

        Inventory inventory = agent.GetComponent<Inventory>();
        if (inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1) && inventory.RemoveItem(SpiceName.Cl, 1)) {
            inventory.GetItemFromTrader(SpiceName.Su, 1);
            isFinished = true;
            return true;
        }
        return false;
    }

    public override bool requiresInRange() {
        return true;
    }

    public override void reset() {
        startTime = 0;
        isFinished = false;
    }
}

