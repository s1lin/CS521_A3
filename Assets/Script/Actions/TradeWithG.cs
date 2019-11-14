using System.Collections.Generic;
using UnityEngine;

class TradeWithG : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds

    public Inventory inventory;

    void Start() {
        target = 6;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        addPrecondition("InCa", 4);

        addEffect("InCa", -4);
        addEffect("InSu", 1);
        addEffect("Capacity", 3);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InCa"))
                return (int)s.Value == 4;
        }
        return false;
    }

    public override bool isDone() {
        return isTrade;
    }

    public override bool perform(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        if (inventory.RemoveItem(SpiceName.Ca, 4)) {
            inventory.GetItemFromTrader(SpiceName.Su, 1);
            isTrade = true;
            return true;
        }
        return false;
    }

    public override bool requiresInRange() {
        return true;// need to be near a trader
    }

    public override void reset() {
        isTrade = false;
        startTime = 0;
    }
}

