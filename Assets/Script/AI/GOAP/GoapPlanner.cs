using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner {

    private List<GoapAction> actionList;
    private HashSet<GoapAction> usedActionList;

    public GoapPlanner() {
        actionList = new List<GoapAction>();
        usedActionList = new HashSet<GoapAction>();
    }

    public Queue<GoapAction> plan(List<GoapAction> actions, List<KeyValuePair<string, object>> worldState, KeyValuePair<string, object> goal) {

        List<Node> leaves = new List<Node>();
        leaves = doPlan(leaves, actions, worldState, goal);

        if (leaves.Count == 0) {
            Debug.Log("NO PLAN");
            return null;
        }

        //Store to Caravan
        KeyValuePair<string, object> subGoal = new KeyValuePair<string, object>("Ca" + goal.Key.Substring(2), goal.Value);
        GoapAction actionStore = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
        buildGraph(leaves[leaves.Count - 1], leaves, new List<GoapAction> { actionStore }, subGoal);

        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (Node a in leaves) {
            queue.Enqueue(a.action);
        }

        return queue;
    }

    private List<Node> doPlan(List<Node> leaves, List<GoapAction> actions, List<KeyValuePair<string, object>> worldState, KeyValuePair<string, object> goal) {

        foreach (GoapAction a in actions) {
            actionList.Add(a);
            a.doReset();
        }

        //Debug.Log("Start World:" + GoapAgent.prettyPrint(worldState));

        List<GoapAction> usableActions = findActions(worldState, new List<GoapAction>());

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
            leaves = doPlan(leaves, actions, worldState, subGoal);
            if (leaves.Count > 0) {
                start = leaves[leaves.Count - 1];
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
        }

        success = buildGraph(start, leaves, usableActions, goal);
        //Store to Caravan
        //subGoal = new KeyValuePair<string, object>("Ca" + goal.Key.Substring(2), goal.Value);
        //GoapAction actionStore = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
        //usableActions = new List<GoapAction> { actionStore };
        //success = buildGraph(start, leaves, usableActions, subGoal);
        //start = leaves[leaves.Count - 1];

        //Debug.Log("End State:" + GoapAgent.prettyPrint(start.state));

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

                if (inState(goal, currentState)) {
                    return true;
                } else {
                    List<GoapAction> subset = new List<GoapAction>();

                    subset.AddRange(findActions(currentState, usableActions));
                    subset.AddRange(usableActions);
                    subset.Remove(action);
                    subset.Add(action);

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


