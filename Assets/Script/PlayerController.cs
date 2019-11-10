using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IGoap {

    public Camera cam;

    public NavMeshAgent agent;

    private Inventory inventory;
    private Caravan caravan;
    private Trader traders;

    public HashSet<KeyValuePair<string, object>> createGoalState() {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>> {
            new KeyValuePair<string, object>("isWin", true)
        };
        return goal;
    }

    public void actionsFinished() {
        throw new System.NotImplementedException();
    }

    public bool moveAgent(GoapAction nextAction) {
        //nextAction.
        //agent.SetDestination(position);
        //nextAction.setInRange(true);
        throw new System.NotImplementedException();
    }

    public void planAborted(GoapAction aborter) {
        throw new System.NotImplementedException();
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal) {
        throw new System.NotImplementedException();
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions) {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //Vector3 position = cam.ScreenToWorldPoint(Input.mousePosition);

            //position.z = -1f;
            //print(position);
            //agent.SetDestination(position);
            StartCoroutine(GoToTrader(0));
            //StartCoroutine();
            //GoToTrader(0);
            Trade(0);

        }
    }

    void Start() {
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public HashSet<KeyValuePair<string, object>> getWorldState() {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasTwoCapacity", inventory.capacity >= 2));
        worldData.Add(new KeyValuePair<string, object>("", caravan.IsWin()));

        return worldData;
    }

    public IEnumerator GoToTrader(int traderIndex) {
        bool updatePath = agent.SetDestination(traders.traderPositions[traderIndex]);
        print(updatePath);
        yield return new WaitForSeconds(2.0f);

    }

    //public void GoToTrader(int traderIndex) {
    //    agent.SetDestination(traders.traderPositions[traderIndex]);
    //}

    public bool Trade(int traderIndex) {
        switch (traderIndex) {
            case 0:            
                inventory.GetItemFromTrader(SpiceName.Tu, 2);
                return true;
            case 1:
                if(inventory.RemoveItem(SpiceName.Tu, 2)) {
                    inventory.GetItemFromTrader(SpiceName.Sa, 1);
                    return true;
                }
                return false;
            case 2:
                if (inventory.RemoveItem(SpiceName.Sa, 2)) {
                    inventory.GetItemFromTrader(SpiceName.Ca, 1);
                    return true;
                }
                return false;
            case 3:
                if (inventory.RemoveItem(SpiceName.Tu, 4)) {
                    inventory.GetItemFromTrader(SpiceName.Ci, 1);
                    return true;
                }
                return false;
            case 4:
                if (inventory.RemoveItem(SpiceName.Ca, 2) && inventory.RemoveItem(SpiceName.Tu, 1)) {
                    inventory.GetItemFromTrader(SpiceName.Cl, 1);
                    return true;
                }
                return false;
            case 5:
                if (inventory.RemoveItem(SpiceName.Tu, 2) && inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1)) {
                    inventory.GetItemFromTrader(SpiceName.Pe, 1);
                    return true;
                }
                return false;
            case 6:
                if (inventory.RemoveItem(SpiceName.Ca, 4)) {
                    inventory.GetItemFromTrader(SpiceName.Su, 1);
                    return true;
                }
                return false;
            case 7:
                if (inventory.RemoveItem(SpiceName.Sa, 1) && inventory.RemoveItem(SpiceName.Ci, 1) && inventory.RemoveItem(SpiceName.Cl, 1)) {
                    inventory.GetItemFromTrader(SpiceName.Su, 1);
                    return true;
                }
                return false;
            default:
                return false;
        }

    }
}
