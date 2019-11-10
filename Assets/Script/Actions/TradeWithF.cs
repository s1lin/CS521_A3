using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithF : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds
    
    public TradeWithF() {
        
        addPrecondition("hasTwoTu", true);
        addPrecondition("hasOneSa", true);
        addPrecondition("hasOneCi", true);
        addEffect("hasOnePe", true);
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
            if (inventory.RemoveItem(SpiceName.Tu, 2) && inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1)) {
                inventory.GetItemFromTrader(SpiceName.Pe, 1);
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

