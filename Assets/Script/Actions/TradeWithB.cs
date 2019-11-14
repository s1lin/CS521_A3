using System.Collections.Generic;
using UnityEngine;

class TradeWithB : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds

    public Inventory inventory;
    void Start() {
        target = 1;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        addPrecondition("InTu", 2);

        addEffect("InTu", -2);
        addEffect("InSa", 1);
        addEffect("Capacity", 1);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InTu"))
                return (int)s.Value >= 2;
        }
        return false;
    }


    public override bool isDone() {
        return isTrade;
    }

    public override bool perform(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        if (inventory.RemoveItem(SpiceName.Tu, 2)) {
            inventory.GetItemFromTrader(SpiceName.Sa, 1);
            isTrade = true;
            return true;
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

