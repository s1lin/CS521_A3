using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IGoap {

    public NavMeshAgent agent;
    public NavMeshSurface surface;

    public GameObject planTextPrefab;
    public GameObject subGoalText;
    public GameObject globalGoalText;
    public GameObject actionText;
    public GameObject winText;
    public GameObject planPlane;

    private Inventory inventory;
    private Caravan caravan;
    private Trader traders;

    private int index = 0;
    private int planIndex = 1;

    private List<KeyValuePair<string, object>> worldData;
    private List<KeyValuePair<string, object>> globalGoal;

    public PlayerController() {
        globalGoal = new List<KeyValuePair<string, object>> {
            new KeyValuePair<string, object>("CaCi", 2),            
            new KeyValuePair<string, object>("CaPe", 2),
            new KeyValuePair<string, object>("CaSu", 2),
            new KeyValuePair<string, object>("CaCl", 2),
            new KeyValuePair<string, object>("CaTu", 2),
            new KeyValuePair<string, object>("CaSa", 2),
            new KeyValuePair<string, object>("CaCa", 2)
        };
    }

    void Start() {
        surface.BuildNavMesh();
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public KeyValuePair<string, object> GetSubGoals() {
        List<KeyValuePair<string, object>> currentState = GetWorldState();
        index = 0;
        while (index != globalGoal.Count) {
            KeyValuePair<string, object> goal = globalGoal[index];
            KeyValuePair<string, object> state = currentState.Find(e => e.Key.Equals(goal.Key));
            globalGoalText.GetComponent<Text>().text = "Get " + ((int)goal.Value - (int)state.Value) + " " + goal.Key.Substring(2) + " into Caravan.";
            if ((int)state.Value >= ((int)goal.Value)) {
                index++;
            } else {//Happen only if the stuff is stolen or initialize the next goal
                KeyValuePair<string, object> sg = new KeyValuePair<string, object>(goal.Key, ((int)goal.Value) - (int)state.Value);
                subGoalText.GetComponent<Text>().text = "Get " + sg.Value + " " + sg.Key.Substring(2) + " into Caravan.";
                return sg;
            }
        }

        return new KeyValuePair<string, object>();
    }

    public void ActionFinished(GoapAction action) {
        GameObject[] texts = GameObject.FindGameObjectsWithTag("plan_text");
        Destroy(texts[1]);
    }
    public void GameFinished() {
        winText.GetComponent<Text>().text = "true";
    }

    public bool MoveAgent(GoapAction nextAction) {

        Vector3 pos;
        if (nextAction.GetType().Name.Equals("InventoryToCar") || nextAction.GetType().Name.Equals("CarToInventory")) {
            pos = caravan.transform.position;
            agent.SetDestination(pos);
        } else {
            pos = traders.traderPositions[nextAction.traderIndex];
            agent.SetDestination(pos);
        }

        if (Vector3.Distance(transform.position, pos) < 5f) {
            nextAction.inRange = true;
            return true;
        }

        return false;
    }

    public void PlanAborted(GoapAction aborter) {
        actionText.GetComponent<Text>().text = "Plan " + planIndex + " action " + GoapAgent.Display(aborter) + " Aborted.";
    }

    public void PlanFailed(KeyValuePair<string, object> failedGoal) {

    }

    public void PlanFound(KeyValuePair<string, object> goal, Queue<GoapAction> actions) {

        GameObject[] texts = GameObject.FindGameObjectsWithTag("plan_text");
        for (int i = 0; i < texts.Length; i++) {
            GameObject.Destroy(texts[i]);
        }

        GameObject textObject = Instantiate(planTextPrefab, planPlane.transform) as GameObject;
        textObject.GetComponent<Text>().text = "The Number " + planIndex.ToString() + " Plan.";
        foreach (GoapAction a in actions) {
            textObject = Instantiate(planTextPrefab, planPlane.transform) as GameObject;
            textObject.GetComponent<Text>().text = GoapAgent.Display(a);
        }
        planIndex++;
        Debug.Log(GoapAgent.Display(actions.ToArray()));
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
