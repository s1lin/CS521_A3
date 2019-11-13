﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TradeWithC : GoapAction {

    private bool isTrade = false;
    private float startTime = 0;
    public float tradeDuration = 0.5f; // seconds


    public Inventory inventory;
    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        addPrecondition("InSa", 2);

        addEffect("InSa", -2);
        addEffect("InCa", 1);        
        addEffect("Capacity", 1);

    }

    public override bool checkProceduralPrecondition(List<KeyValuePair<string, object>> state) {
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals("InSa"))
                return (int)s.Value >= 2;
        }
        return false;
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
            if (inventory.RemoveItem(SpiceName.Sa, 2)) {
                inventory.GetItemFromTrader(SpiceName.Ca, 1);
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

