using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithH : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds
    
    public TradeWithH() {
        
        addPrecondition("hasOneSa", true);
        addPrecondition("hasOneCi", true);
        addPrecondition("hasOneCl", true);
        addEffect("hasOneSu", true);
    }
    public override bool checkProceduralPrecondition(GameObject agent) {
        throw new NotImplementedException();
    }

    public override bool isDone() {
        return isTrade;
    }

    public override bool perform(GameObject agent) {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > tradeDuration) {
            // finished chopping
            Inventory inventory = agent.GetComponent<Inventory>();
            if (inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1) && inventory.RemoveItem(SpiceName.Cl, 1)) {
                inventory.GetItemFromTrader(SpiceName.Su, 1);
                return true;
            }
            return false;
        }
        return false;
    }

    public override bool requiresInRange() {
        return true;// need to be near a trader
    }

    public override void reset() {
        startTime = 0;
    }
}

