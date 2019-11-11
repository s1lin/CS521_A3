using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CarToInventory : GoapAction {

    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds

    private Caravan caravan;
    
    void Start() {
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
        addPrecondition("caHasOneTu", caravan.GetItemValue(SpiceName.Tu) >= 1);
        addPrecondition("caHasOneSa", caravan.GetItemValue(SpiceName.Sa) >= 1);
        addPrecondition("caHasOneCa", caravan.GetItemValue(SpiceName.Ca) >= 1);
        addPrecondition("caHasOneCi", caravan.GetItemValue(SpiceName.Ci) >= 1);
        addPrecondition("caHasOneCl", caravan.GetItemValue(SpiceName.Cl) >= 1);
        addPrecondition("caHasOnePe", caravan.GetItemValue(SpiceName.Pe) >= 1);
        addPrecondition("caHasOneSu", caravan.GetItemValue(SpiceName.Su) >= 1);
        //addEffect("hasOneSu", true);
    }
    public override bool checkProceduralPrecondition(HashSet<KeyValuePair<string, object>> state) {
        return false;
    }

    public override bool isDone() {
        return true;
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
        return true;// need to be near a 
    }

    public override void reset() {
        startTime = 0;
    }
}

