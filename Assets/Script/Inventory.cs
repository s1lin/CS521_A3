using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public int capacity = 4;
    public Dictionary<SpiceName, int> items;
    private Caravan caravan;
    private Text inventoryText;

    public Inventory() {
        items = new Dictionary<SpiceName, int> {
            { SpiceName.Tu, 0 },
            { SpiceName.Sa, 0 },
            { SpiceName.Ca, 0 },
            { SpiceName.Ci, 0 },
            { SpiceName.Cl, 0 },
            { SpiceName.Pe, 0 },
            { SpiceName.Su, 0 }
        };
    }
    void Start() {
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
    }

    public void GetItemFromCaravan(SpiceName name, int value) {
        int curValue = caravan.GetItemValue(name);
        if (curValue >= value && capacity >= value) {
            for (int i = 0; i < value; i++)
                PutItem(name);
            for (int i = 0; i < value; i++)
                caravan.RemoveItem(name);
        }
    }

    public void GetItemFromTrader(SpiceName name, int value) {
        if (capacity >= value) {
            for (int i = 0; i < value; i++)
                PutItem(name);
        }
    }

    public void PutItem(SpiceName name) {
        int value = 0;
        if (capacity >= 0) {

            if (items.TryGetValue(name, out value)) {
                items.Remove(name);
                items.Add(name, value + 1);
            } else
                items.Add(name, 1);

            value++;
            inventoryText = GameObject.FindGameObjectWithTag(SpiceNames.ToString(name)).GetComponent<Text>();
            inventoryText.text = "" + value;
            capacity--;
        }
    }

    public bool RemoveItem(SpiceName name, int quanlity) {
        int value = 0;
        if (capacity >= 0) {
            if (items.TryGetValue(name, out value)) {
                if (value != 0 && value >= quanlity) {
                    value -= quanlity;
                    items.Remove(name);
                    items.Add(name, value);

                    capacity += quanlity;
                    inventoryText = GameObject.FindGameObjectWithTag(SpiceNames.ToString(name)).GetComponent<Text>();
                    inventoryText.text = string.Format("{0}", value);
                    return true;
                }
            }
        }
        return false;
    }

    public int GetItemValue(SpiceName name) {
        items.TryGetValue(name, out int value);
        return value;
    }

}
