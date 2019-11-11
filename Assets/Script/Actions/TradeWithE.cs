using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithE : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds
    public Inventory inventory;
    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        addPrecondition("InCa", 1);
        addPrecondition("InTu", 1);

        addEffect("InCa", -1);
        addEffect("InTu", -1);
        addEffect("InCl", +1);
        addEffect("Capacity", 1);

    }

    public override bool checkProceduralPrecondition(HashSet<KeyValuePair<string, object>> state) {

        bool satisfied1 = false;
        bool satisfied2 = false;

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu") && (int)s.Value >= 1) {
                satisfied1 = true;
            }
            if (s.Key.Equals("InCa") && (int)s.Value >= 2) {
                satisfied2 = true;
            }
        }
        return satisfied1 && satisfied2;
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
            if (inventory.RemoveItem(SpiceName.Ca, 2) && inventory.RemoveItem(SpiceName.Tu, 1)) {
                inventory.GetItemFromTrader(SpiceName.Cl, 1);
                isTrade = true;
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
        isTrade = false;
    }
}

