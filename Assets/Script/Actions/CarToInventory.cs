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
 
        //addEffect("Capacity", 0);
    }
    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        //foreach (KeyValuePair<string, object> s in state) {
        //    if (s.Key.Contains("Ca"))
        //        if ((int)s.Value > 0)
        //            return false;
        //}

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

