using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Caravan : MonoBehaviour {

    private Dictionary<SpiceName, int> items;
    private Inventory inventory;

    private Text caravanText;
    private int tu, sa, ca, ci, cl, pe, su;
 
    void Start() {
        items = new Dictionary<SpiceName, int>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            GetItemFromInventory(SpiceName.Ca, 2);
            GetItemFromInventory(SpiceName.Tu, 2);
        }

        if (IsWin()) {
            print("Player Win");
        }
    }

    public bool IsWin() {
        if (items.Keys.Count == 8) {
            foreach (KeyValuePair<SpiceName, int> entry in items) {
                if (entry.Value < 2)
                    return false;
            }
            return true;
        }
        return false;
    }

    public void GetItemFromInventory(SpiceName name, int value) {
        int curValue = inventory.GetItemValue(name);
        if (curValue >= value) {
            for (int i = 0; i < value; i++)
                PutItem(name);
            inventory.RemoveItem(name, value);
        }
    }

    public void PutItem(SpiceName name) {
        if (items.TryGetValue(name, out int value)) {
            items.Remove(name);
            items.Add(name, value + 1);
        } else
            items.Add(name, 1);

        value++;
        caravanText = GameObject.FindGameObjectWithTag(SpiceNames.ToString(name) + "_c").GetComponent<Text>();
        caravanText.text = "" + value;
    }
    public void RemoveItem(SpiceName name) {
        int value = 0;

        if (items.TryGetValue(name, out value)) {
            if (value != 0) {
                value--;
                items.Remove(name);
                items.Add(name, value);
                
                caravanText = GameObject.FindGameObjectWithTag(SpiceNames.ToString(name) + "_c").GetComponent<Text>();
                caravanText.text = string.Format("{0}", value);
            }
        }

    }

    public int GetItemValue(SpiceName name) {
        items.TryGetValue(name, out int value);
        return value;
    }
}
