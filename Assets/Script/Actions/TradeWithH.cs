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

    public Inventory inventory;
    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        addPrecondition("InSa", 1);
        addPrecondition("InCi", 1);
        addPrecondition("InCl", 1);

        addEffect("InSa", -1);
        addEffect("InCi", -1);
        addEffect("InCl", -1);
        addEffect("InSu", 1);
        addEffect("Capacity", 2);

    }

    public override bool checkProceduralPrecondition(HashSet<KeyValuePair<string, object>> state) {

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

