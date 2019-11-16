using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class InventoryToCar : GoapAction {

    private bool isSucc = false;
    private bool isFinished = false;

    public InventoryToCar() {
        AddEffect("Capacity", 4);
    }
    public override void DoAction(GameObject agent) {
        StartCoroutine(Action(agent));
    }

    public IEnumerator Action(GameObject agent) {

        Inventory inventory = agent.GetComponent<Inventory>();
        Caravan caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();

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

        inWait = true;
        yield return new WaitForSecondsRealtime(actionDuration);
        inWait = false;

        isFinished = true;
        isSucc = true;
    }

    public override bool IsActionUsable(List<KeyValuePair<string, object>> state) {
        return !state.Find(e => (int)e.Value > 0 && e.Key != ("Capacity")).Equals(new KeyValuePair<string, object>());
    }

    public override bool IsDone() {
        return isFinished;
    }
    public override bool IsSucc() {
        return isSucc;
    }

    public override void Reset() {
        isFinished = false;
        isSucc = false;
    }
}