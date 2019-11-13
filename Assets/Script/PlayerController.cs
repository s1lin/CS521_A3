using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IGoap {

    public Camera cam;

    public NavMeshAgent agent;

    private Inventory inventory;
    private Caravan caravan;
    private Trader traders;
    private GameObject plan;

    private Queue<KeyValuePair<string, object>> globalGoal;

    public PlayerController() {
        globalGoal = new Queue<KeyValuePair<string, object>>();
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaTu", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaSa", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaCa", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaCi", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaCl", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaPe", 2));
        globalGoal.Enqueue(new KeyValuePair<string, object>("CaSu", 2));
    }

    void Start() {
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        caravan = GameObject.FindGameObjectWithTag("Caravan").GetComponent<Caravan>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        plan = GameObject.FindGameObjectWithTag("Plans");

        
    }

    public List<KeyValuePair<string, object>> createGoalState() {
        List<KeyValuePair<string, object>> goal = new List<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("InTu", 2)); //Capacity < 4           
        //goal.Add(new KeyValuePair<string, object>("InSa", 1)); //Capacity < 4          
        //goal.Add(new KeyValuePair<string, object>("InCa", 1)); //Capacity < 4        
        //goal.Add(new KeyValuePair<string, object>("InCi", 1)); //Capacity < 4        
        //goal.Add(new KeyValuePair<string, object>("InCl", 1)); //Capacity < 4        
        //goal.Add(new KeyValuePair<string, object>("InPe", 1)); //Capacity < 4        
        //goal.Add(new KeyValuePair<string, object>("InSu", 1)); //somewhere would get 0;       
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
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public void planFailed(KeyValuePair<string, object> failedGoal) {
        
    }

    public void planFound(KeyValuePair<string, object> goal, Queue<GoapAction> actions) {
        string s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        //print(s);
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
        //GameObject textObject = new GameObject("plan");
        //textObject.transform.SetParent(plan.transform);
        //textObject.AddComponent<Text>().text = s;
        //textObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
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

        }
    }



    public List<KeyValuePair<string, object>> getWorldState() {

        List<KeyValuePair<string, object>> worldData = new List<KeyValuePair<string, object>> {
            new KeyValuePair<string, object>("Capacity", inventory.capacity),
            new KeyValuePair<string, object>("InTu", inventory.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<string, object>("InSa", inventory.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<string, object>("InCa", inventory.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<string, object>("InCi", inventory.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<string, object>("InCl", inventory.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<string, object>("InPe", inventory.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<string, object>("InSu", inventory.GetItemValue(SpiceName.Su)),
            new KeyValuePair<string, object>("CaTu", inventory.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<string, object>("CaSa", inventory.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<string, object>("CaCa", inventory.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<string, object>("CaCi", inventory.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<string, object>("CaCl", inventory.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<string, object>("CaPe", inventory.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<string, object>("CaSu", inventory.GetItemValue(SpiceName.Su))
        };

        return worldData;
    }

    public IEnumerator GoToTrader(int traderIndex) {
        bool updatePath = agent.SetDestination(traders.traderPositions[traderIndex]);
        print(updatePath);
        yield return new WaitForSeconds(2.0f);

    }
}
