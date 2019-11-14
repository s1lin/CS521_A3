using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner {

    private List<GoapAction> actionList;
    private HashSet<GoapAction> usedActionList;

    public GoapPlanner() {
        this.actionList = new List<GoapAction>();
        this.usedActionList = new HashSet<GoapAction>();
    }

    public Queue<GoapAction> plan(List<GoapAction> actions, List<KeyValuePair<string, object>> worldState,
                                  KeyValuePair<string, object> goal) {

        List<Node> leaves = new List<Node>();
        leaves = plan(leaves, actions, worldState, goal);

        if (leaves.Count == 0) {
            // oh no, we didn't get a plan
            Debug.Log("NO PLAN");
            return null;
        }

        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (Node a in leaves) {
            queue.Enqueue(a.action);
        }

        // hooray we have a plan!
        return queue;
    }
    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */
    private List<Node> plan(List<Node> leaves, List<GoapAction> actions, List<KeyValuePair<string, object>> worldState,
                           KeyValuePair<string, object> goal) {


        // reset the actions so we can start fresh with them
        foreach (GoapAction a in actions) {
            actionList.Add(a);
            a.doReset();
        }

        Debug.Log("Start World:" + GoapAgent.prettyPrint(worldState));

        List<GoapAction> usableActions = findActions(worldState, new List<GoapAction>());

        // build up the tree and record the leaf nodes that provide a solution to the goal.


        // build graph
        Node start = new Node(null, 0, worldState, null);
        bool success = false;
        KeyValuePair<string, object> subGoal;

        if (goal.Key.Contains("Ci")) {
            subGoal = new KeyValuePair<string, object>("InTu", 2);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            subGoal = new KeyValuePair<string, object>("InTu", 4);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            GoapAction actionD = actionList.Find(e => e.GetType().Name.Equals("TradeWithD"));
            usableActions = new List<GoapAction> { actionD };
        }

        if (goal.Key.Contains("Pe")) {
            subGoal = new KeyValuePair<string, object>("InCi", 1);
            leaves = plan(leaves, actions, worldState, subGoal);
            if (leaves.Count > 0) {
                start = leaves[leaves.Count - 2];
                leaves.RemoveAt(leaves.Count - 1);//Remove the caravan action
            } else
                return new List<Node>();
        }

        if (goal.Key.Contains("Su")) {

            for (int i = 0; i < 2; i++) {
                subGoal = new KeyValuePair<string, object>("InCa", 2);
                buildGraph(start, leaves, usableActions, subGoal);
                start = leaves[leaves.Count - 1];

                //Store to Caravan
                subGoal = new KeyValuePair<string, object>("Ca" + subGoal.Key.Substring(2), (int)subGoal.Value * (i + 1));
                GoapAction store = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
                usableActions = new List<GoapAction> { store };
                success = buildGraph(start, leaves, usableActions, subGoal);
                start = leaves[leaves.Count - 1];

                GoapAction actionA = actionList.Find(e => e.GetType().Name.Equals("TradeWithA"));
                usableActions = new List<GoapAction> { actionA };
            }

            //Take out from Caravan
            subGoal = new KeyValuePair<string, object>("InCa", 4);
            GoapAction take = actionList.Find(e => e.GetType().Name.Equals("CarToInventory"));
            usableActions = new List<GoapAction> { take };
            success = buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            usableActions = new List<GoapAction>();
            foreach (GoapAction a in actions) {
                if (a.checkProceduralPrecondition(start.state)) {
                    usableActions.Add(a);
                    usedActionList.Add(a);
                }
            }
            //subGoal = new KeyValuePair<string, object>("InSu", 1);
            //buildGraph(start, leaves, usableActions, subGoal);
            //start = leaves[leaves.Count - 1];

            //subGoal = new KeyValuePair<string, object>("InCl", 1);
            //buildGraph(start, leaves, usableActions, subGoal);
            //start = leaves[leaves.Count - 1];

            //subGoal = new KeyValuePair<string, object>("InCi", 1);
            //buildGraph(start, leaves, usableActions, subGoal);
            //start = leaves[leaves.Count - 1];

        }

        buildGraph(start, leaves, usableActions, goal);
        start = leaves[leaves.Count - 1];

        //Store to Caravan
        subGoal = new KeyValuePair<string, object>("Ca" + goal.Key.Substring(2), goal.Value);
        GoapAction actionStore = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
        usableActions = new List<GoapAction> { actionStore };
        success = buildGraph(start, leaves, usableActions, subGoal);
        start = leaves[leaves.Count - 1];

        Debug.Log("End State:" + GoapAgent.prettyPrint(start.state));

        if (!success) {
            return new List<Node>();
        }

        return leaves;

    }

    public List<KeyValuePair<string, object>> findSubGoal(List<KeyValuePair<string, object>> goal) {
        List<KeyValuePair<string, object>> subGoal = new List<KeyValuePair<string, object>>() {
            new KeyValuePair<string, object>("Capacity", 2)
        };
        return subGoal;
    }


    private bool buildGraph(Node parent, List<Node> leaves, List<GoapAction> usableActions, KeyValuePair<string, object> goal) {

        List<KeyValuePair<string, object>> currentState;

        foreach (GoapAction action in usableActions) {

            if (inState(action.Preconditions, parent.state)) {
                if (action.GetType().Name.Equals("InventoryToCar")) {
                    currentState = popluateStae(parent.state);
                } else if (action.GetType().Name.Equals("CarToInventory")) {
                    currentState = popluateStae(parent.state, goal, action);
                } else {
                    currentState = populateState(parent.state, action.Effects);
                }

                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
                leaves.Add(node);
                //Debug.Log(GoapAgent.prettyPrint(currentState));
                if (inState(goal, currentState)) {
                    if (action.GetType().Name.Equals("CarToInventory")) {

                    }
                    return true;
                } else {
                    // not at a solution yet, so test all the remaining actions and branch out the tree                    
                    List<GoapAction> subset = new List<GoapAction>();

                    subset.AddRange(findActions(currentState, usableActions));
                    subset.AddRange(usableActions);
                    subset.Remove(action);
                    subset.Add(action);

                    //Debug.Log("New actions: " + GoapAgent.prettyPrint(subset));
                    return buildGraph(node, leaves, subset, goal);
                }
            }
        }
        return false;
    }

    /**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */
    private List<GoapAction> findActions(List<KeyValuePair<string, object>> currentState, List<GoapAction> exclude) {
        List<GoapAction> newActions = new List<GoapAction>();

        foreach (GoapAction a in actionList) {
            if (!exclude.Contains(a) && !usedActionList.Contains(a)) {
                if (a.checkProceduralPrecondition(currentState)) {
                    newActions.Add(a);
                    usedActionList.Add(a);
                }
            }
        }

        foreach (GoapAction a in actionList) {
            if (!exclude.Contains(a) && !newActions.Contains(a)) {
                if (a.checkProceduralPrecondition(currentState)) {
                    newActions.Add(a);
                    usedActionList.Add(a);
                }
            }
        }

        return newActions;
    }

    /**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */

    private bool inState(KeyValuePair<string, object> t, List<KeyValuePair<string, object>> state) {

        bool match = false;
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals(t.Key)) {
                match = (int)s.Value >= (int)t.Value;
                if (match)
                    break;
            }
        }
        return match;
    }

    private bool inState(List<KeyValuePair<string, object>> test, List<KeyValuePair<string, object>> state) {
        bool allMatch = true;
        foreach (KeyValuePair<string, object> t in test) {
            bool match = false;
            foreach (KeyValuePair<string, object> s in state) {
                if (s.Key.Equals(t.Key)) {
                    match = (int)s.Value >= (int)t.Value;
                    if (match)
                        break;
                }
            }
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }
    private List<KeyValuePair<string, object>> popluateStae(List<KeyValuePair<string, object>> state) {

        List<KeyValuePair<string, object>> tempState = new List<KeyValuePair<string, object>>();
        List<KeyValuePair<string, object>> currentState = new List<KeyValuePair<string, object>>();

        foreach (KeyValuePair<string, object> s in state) {
            tempState.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> inState in tempState) {
            KeyValuePair<string, object> updated;
            if (inState.Key.Contains("In")) {
                int inValue = (int)inState.Value;
                string item = "Ca" + inState.Key.Substring(2);

                foreach (KeyValuePair<string, object> caState in tempState) {
                    if (caState.Key.Equals(item)) {
                        updated = new KeyValuePair<string, object>(caState.Key, (int)caState.Value + inValue);
                        currentState.Add(updated);
                        break;
                    }
                }
                updated = new KeyValuePair<string, object>(inState.Key, 0);
                currentState.Add(updated);

            } else if (inState.Key.Equals("Capacity")) {
                currentState.Add(new KeyValuePair<string, object>(inState.Key, 4));
            }
        }
        return currentState;
    }

    private List<KeyValuePair<string, object>> popluateStae(List<KeyValuePair<string, object>> state, KeyValuePair<string, object> goal, GoapAction action) {

        List<KeyValuePair<string, object>> currentState = new List<KeyValuePair<string, object>>();

        foreach (KeyValuePair<string, object> s in state) {
            currentState.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        KeyValuePair<string, object> oldState = state.Find(e => e.Key.Equals(goal.Key));
        KeyValuePair<string, object> inState = state.Find(e => e.Key.Equals("Ca" + goal.Key.Substring(2)));
        KeyValuePair<string, object> capa = state.Find(e => e.Key.Equals("Capacity"));

        if ((int)inState.Value >= (int)goal.Value) {
            int inValue = (int)inState.Value;
            int capacity = (int)capa.Value;
            int takeValue = inValue <= capacity ? inValue : capacity;

            KeyValuePair<string, object> updatedG = new KeyValuePair<string, object>(goal.Key, takeValue);
            KeyValuePair<string, object> updatedI = new KeyValuePair<string, object>(inState.Key, inValue - takeValue);
            KeyValuePair<string, object> updatedC = new KeyValuePair<string, object>("Capacity", capacity - takeValue);

            currentState.Remove(oldState);
            currentState.Remove(inState);
            currentState.Remove(capa);

            currentState.Add(updatedG);
            currentState.Add(updatedI);
            currentState.Add(updatedC);

            action.takeout.Add(new KeyValuePair<SpiceName, int>(SpiceNames.Get(goal.Key.Substring(2)), takeValue));
        }

        return currentState;
    }

    private List<KeyValuePair<string, object>> populateState(
        List<KeyValuePair<string, object>> currentState, List<KeyValuePair<string, object>> stateChange) {

        List<KeyValuePair<string, object>> state = new List<KeyValuePair<string, object>>();
        // copy the KVPs over as new objects
        foreach (KeyValuePair<string, object> s in currentState) {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> change in stateChange) {
            // if the key exists in the current state, update the Value
            bool exists = false;
            int sValue = 0;
            foreach (KeyValuePair<string, object> s in state) {
                if (s.Key.Equals(change.Key)) {
                    sValue = (int)s.Value;
                    exists = true;
                    break;
                }
            }

            if (exists) {
                state.RemoveAll((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, (int)change.Value + sValue);
                state.Add(updated);
            }
            // if it does not exist in the current state, add it
            else {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    /**
	 * Used for building up the graph and holding the running costs of actions.
	 */
    private class Node {
        public Node parent;
        public float runningCost;
        public List<KeyValuePair<string, object>> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, List<KeyValuePair<string, object>> state, GoapAction action) {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

}


