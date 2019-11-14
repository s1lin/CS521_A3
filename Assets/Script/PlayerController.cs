using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IGoap {

    public NavMeshAgent agent;

    public GameObject planTextPrefab;
    public GameObject subGoalText;
    public GameObject planPlane;

    private Inventory inventory;
    private Caravan caravan;
    private Trader traders;

    private int index = 0;


    private List<KeyValuePair<string, object>> worldData;

    private List<KeyValuePair<string, object>> subGoal;
    private List<KeyValuePair<string, object>> globalGoal;

    public PlayerController() {
        globalGoal = new List<KeyValuePair<string, object>> {
            //
            new KeyValuePair<string, object>("CaTu", 2),
            new KeyValuePair<string, object>("CaSa", 2),
            new KeyValuePair<string, object>("CaCa", 2),
            new KeyValuePair<string, object>("CaCi", 2),
            new KeyValuePair<string, object>("CaCl", 2),
            new KeyValuePair<string, object>("CaPe", 2)
            //new KeyValuePair<string, object>("CaSu", 2)
        };

        subGoal = new List<KeyValuePair<string, object>> {
            new KeyValuePair<string, object>("InTu", 2), //Capacity < 4           
            new KeyValuePair<string, object>("InSa", 2), //Capacity < 4          
            new KeyValuePair<string, object>("InCa", 1), //Capacity < 4        
            new KeyValuePair<string, object>("InCi", 1), //Capacity < 4        
            new KeyValuePair<string, object>("InCl", 1), //Capacity < 4        
            new KeyValuePair<string, object>("InPe", 1) //Capacity < 4      
            //new KeyValuePair<string, object>("InSu", 1)
        };
    }

    void Start() {
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public KeyValuePair<string, object> GetSubGoals() {
        List<KeyValuePair<string, object>> currentState = GetWorldState();
        print("Here");
        while (index != globalGoal.Count) {
            KeyValuePair<string, object> goal = globalGoal[index];
            KeyValuePair<string, object> state = currentState.Find(e => e.Key.Equals(goal.Key));
            if ((int)state.Value >= ((int)goal.Value)) {
                index++;
            } else {
                KeyValuePair<string, object> sg = subGoal.Find(e => e.Key.Contains(goal.Key.Substring(2)));
                subGoalText.GetComponent<Text>().text = "Get " + sg.Value + " " + sg.Key.Substring(2) + " into Inventory.";
                return sg;
            }
        }

        return new KeyValuePair<string, object>();
    }

    public void ActionFinished(GoapAction action) {
        print("action:" + action.GetType().Name + " is finished.");
    }
    public void GameFinished() {
        print("You WiN!");
    }

    public bool MoveAgent(GoapAction nextAction) {
        Vector3 pos;
        if (nextAction.GetType().Name.Equals("InventoryToCar") || nextAction.GetType().Name.Equals("CarToInventory")) {
            pos = caravan.transform.position;
            agent.SetDestination(pos);
        } else {
            pos = traders.traderPositions[nextAction.target];
            agent.SetDestination(pos);
        }

        if (Vector3.Distance(transform.position, pos) < 3.9f) {
            nextAction.setInRange(true);
            return true;
        }

        return false;
    }

    public void PlanAborted(GoapAction aborter) {
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public void PlanFailed(KeyValuePair<string, object> failedGoal) {

    }

    public void PlanFound(KeyValuePair<string, object> goal, Queue<GoapAction> actions) {

        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
        GameObject[] texts = GameObject.FindGameObjectsWithTag("plan_text");
        for (int i = 0; i < texts.Length; i++) {
            GameObject.Destroy(texts[i]);
        }

        foreach (GoapAction a in actions) {
            GameObject textObject = Instantiate(planTextPrefab, planPlane.transform) as GameObject;
            textObject.GetComponent<Text>().text = GoapAgent.prettyPrint(a);
        }

    }

    public List<KeyValuePair<string, object>> GetWorldState() {

        worldData = new List<KeyValuePair<string, object>> {
            new KeyValuePair<string, object>("Capacity", inventory.capacity),
            new KeyValuePair<string, object>("InTu", inventory.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<string, object>("InSa", inventory.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<string, object>("InCa", inventory.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<string, object>("InCi", inventory.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<string, object>("InCl", inventory.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<string, object>("InPe", inventory.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<string, object>("InSu", inventory.GetItemValue(SpiceName.Su)),
            new KeyValuePair<string, object>("CaTu", caravan.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<string, object>("CaSa", caravan.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<string, object>("CaCa", caravan.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<string, object>("CaCi", caravan.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<string, object>("CaCl", caravan.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<string, object>("CaPe", caravan.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<string, object>("CaSu", caravan.GetItemValue(SpiceName.Su))
        };

        return worldData;
    }

}
