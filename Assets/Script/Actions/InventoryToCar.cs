using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InventoryToCar : GoapAction {

    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds

    private Caravan caravan;
    public Inventory inventory;

    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();

        addEffect("Capacity", 4);
    }
    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {

        //foreach (KeyValuePair<string, object> s in state) {
        //    if (s.Key.Equals("Capacity"))
        //        if ((int)s.Value < 4)
        //            return true;

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
            inventory.items.TryGetValue(SpiceName.Tu, out int value);
            inventory.items.TryGetValue(SpiceName.Sa, out int value2);
            inventory.items.TryGetValue(SpiceName.Ca, out int value3);
            inventory.items.TryGetValue(SpiceName.Ci, out int value4);
            inventory.items.TryGetValue(SpiceName.Cl, out int value5);
            inventory.items.TryGetValue(SpiceName.Pe, out int value6);
            inventory.items.TryGetValue(SpiceName.Su, out int value7);

            caravan.GetItemFromInventory(SpiceName.Tu, value);
            caravan.GetItemFromInventory(SpiceName.Sa, value2);
            caravan.GetItemFromInventory(SpiceName.Ca, value3);
            caravan.GetItemFromInventory(SpiceName.Ci, value4);
            caravan.GetItemFromInventory(SpiceName.Cl, value5);
            caravan.GetItemFromInventory(SpiceName.Pe, value6);
            caravan.GetItemFromInventory(SpiceName.Su, value7);

            //addEffect("hasTwoCapacity", true);
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

