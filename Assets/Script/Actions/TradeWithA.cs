using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithA : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds

    public Inventory inventory;
    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        //addPrecondition("Capacity", 2);
        addEffect("InTu", 2);
        addEffect("Capacity", -2);
    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        //foreach(KeyValuePair<string, object> s in state) {
        //    if (s.Key.Equals("Capacity"))
        //        return (int)s.Value >= 2;
        //}
        //return false;
        return true;
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
            inventory.GetItemFromTrader(SpiceName.Tu, 2);
        }
        isTrade = true;
        target = GameObject.FindGameObjectWithTag("trader1");
        return true;
    }

    public override bool requiresInRange() {
        return true;// need to be near a trader
    }

    public override void reset() {
        startTime = 0;
        isTrade = false;
    }

   
}

