using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner {

    private List<GoapAction> actionList;
    private HashSet<GoapAction> usedActionList;

    public GoapPlanner() {
        actionList = new List<GoapAction>();
        usedActionList = new HashSet<GoapAction>();
    }

    public Queue<GoapAction> Plan(List<GoapAction> actions, List<KeyValuePair<string, object>> worldState, KeyValuePair<string, object> goal) {

        List<GoapNode> leaves = new List<GoapNode>();
        leaves = DoPlan(leaves, actions, worldState, goal);

        if (leaves.Count == 0) {
            Debug.Log("NO PLAN");
            return null;
        }

        //Store to Caravan
        KeyValuePair<string, object> subGoal = new KeyValuePair<string, object>("Ca" + goal.Key.Substring(2), goal.Value);
        GoapAction actionStore = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
        BuildGOAP(leaves[leaves.Count - 1], leaves, new List<GoapAction> { actionStore }, subGoal);

        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (GoapNode a in leaves) {
            queue.Enqueue(a.action);
        }

        return queue;
    }

    private List<GoapNode> DoPlan(List<GoapNode> leaves, List<GoapAction> actions, List<KeyValuePair<string, object>> worldState, KeyValuePair<string, object> goal) {

        foreach (GoapAction a in actions) {
            actionList.Add(a);
            a.DoReset();
        }

        Debug.Log("Start World:" + GoapAgent.Display(worldState));

        List<GoapAction> usableActions = FindUsableActions(worldState, new List<GoapAction>());

        // build graph
        GoapNode start = new GoapNode(null, 0, worldState, null);
        bool success = false;
        KeyValuePair<string, object> subGoal;

        if (goal.Key.Contains("Ci")) {
            subGoal = new KeyValuePair<string, object>("InTu", 2);
            BuildGOAP(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            subGoal = new KeyValuePair<string, object>("InTu", 4);
            BuildGOAP(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            GoapAction actionD = actionList.Find(e => e.GetType().Name.Equals("TradeWithD"));
            usableActions = new List<GoapAction> { actionD };
        }

        if (goal.Key.Contains("Pe")) {
            subGoal = new KeyValuePair<string, object>("InCi", 1);
            leaves = DoPlan(leaves, actions, worldState, subGoal);
            if (leaves.Count > 0) {
                start = leaves[leaves.Count - 1];
            } else
                return new List<GoapNode>();
        }

        if (goal.Key.Contains("Su")) {

            for (int i = 0; i < 2; i++) {
                subGoal = new KeyValuePair<string, object>("InCa", 2);
                BuildGOAP(start, leaves, usableActions, subGoal);
                start = leaves[leaves.Count - 1];

                //Store to Caravan
                subGoal = new KeyValuePair<string, object>("Ca" + subGoal.Key.Substring(2), (int)subGoal.Value * (i + 1));
                GoapAction store = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
                usableActions = new List<GoapAction> { store };
                success = BuildGOAP(start, leaves, usableActions, subGoal);
                start = leaves[leaves.Count - 1];

                GoapAction actionA = actionList.Find(e => e.GetType().Name.Equals("TradeWithA"));
                usableActions = new List<GoapAction> { actionA };
            }

            //Take out from Caravan
            subGoal = new KeyValuePair<string, object>("InCa", 4);
            GoapAction take = actionList.Find(e => e.GetType().Name.Equals("CarToInventory"));
            usableActions = new List<GoapAction> { take };
            success = BuildGOAP(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            usableActions = new List<GoapAction>();
            foreach (GoapAction a in actions) {
                if (a.IsActionUsable(start.state)) {
                    usableActions.Add(a);
                    usedActionList.Add(a);
                }
            }
        }

        success = BuildGOAP(start, leaves, usableActions, goal);

        if (!success) {
            return new List<GoapNode>();
        }

        return leaves;

    }

    private bool BuildGOAP(GoapNode parent, List<GoapNode> graph, List<GoapAction> usableActions, KeyValuePair<string, object> goal) {

        List<KeyValuePair<string, object>> currentState;

        foreach (GoapAction action in usableActions) {

            if (IsStateSatisfied(action.preconditions, parent.state)) {
                if (action.GetType().Name.Equals("InventoryToCar")) {
                    currentState = ApplyStateChange(parent.state);
                } else if (action.GetType().Name.Equals("CarToInventory")) {
                    currentState = ApplyStateChange(parent.state, goal, action);
                } else {
                    currentState = ApplyStateChange(parent.state, action.effects);
                }

                GoapNode node = new GoapNode(parent, parent.runningCost + action.cost, currentState, action);
                graph.Add(node);

                if (IsStateSatisfied(goal, currentState)) {
                    return true;
                } else {
                    List<GoapAction> subset = new List<GoapAction>();

                    subset.AddRange(FindUsableActions(currentState, usableActions));
                    subset.AddRange(usableActions);
                    subset.Remove(action);
                    subset.Add(action);

                    return BuildGOAP(node, graph, subset, goal);
                }
            }
        }
        return false;
    }

    private List<GoapAction> FindUsableActions(List<KeyValuePair<string, object>> currentState, List<GoapAction> exclude) {
        List<GoapAction> newActions = new List<GoapAction>();

        foreach (GoapAction a in actionList) {
            if (!exclude.Contains(a) && !usedActionList.Contains(a)) {
                if (a.IsActionUsable(currentState)) {
                    newActions.Add(a);
                    usedActionList.Add(a);
                }
            }
        }

        foreach (GoapAction a in actionList) {
            if (!exclude.Contains(a) && !newActions.Contains(a)) {
                if (a.IsActionUsable(currentState)) {
                    newActions.Add(a);
                    usedActionList.Add(a);
                }
            }
        }

        return newActions;
    }

    private bool IsStateSatisfied(KeyValuePair<string, object> test, List<KeyValuePair<string, object>> state) {

        bool match = false;
        foreach (KeyValuePair<string, object> s in state) {
            if (s.Key.Equals(test.Key)) {
                match = (int)s.Value >= (int)test.Value;
                if (match)
                    break;
            }
        }
        return match;
    }

    private bool IsStateSatisfied(List<KeyValuePair<string, object>> test, List<KeyValuePair<string, object>> state) {
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

    private List<KeyValuePair<string, object>> ApplyStateChange(List<KeyValuePair<string, object>> state) {

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

    private List<KeyValuePair<string, object>> ApplyStateChange(List<KeyValuePair<string, object>> state, KeyValuePair<string, object> goal, GoapAction action) {

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

    private List<KeyValuePair<string, object>> ApplyStateChange(List<KeyValuePair<string, object>> currentState, List<KeyValuePair<string, object>> changeState) {

        List<KeyValuePair<string, object>> state = new List<KeyValuePair<string, object>>();

        foreach (KeyValuePair<string, object> s in currentState) {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> change in changeState) {

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
                state.RemoveAll(e => { return e.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, (int)change.Value + sValue);
                state.Add(updated);
            } else {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    private class GoapNode {
        public GoapNode parent;
        public float runningCost;
        public List<KeyValuePair<string, object>> state;
        public GoapAction action;

        public GoapNode(GoapNode parent, float runningCost, List<KeyValuePair<string, object>> state, GoapAction action) {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

}