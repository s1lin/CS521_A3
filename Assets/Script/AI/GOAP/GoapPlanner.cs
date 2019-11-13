using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */
public class GoapPlanner {

    private List<GoapAction> actionList;

    private HashSet<GoapAction> usedActionList;

    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */
    public Queue<GoapAction> plan(List<GoapAction> availableActions,
                                  List<KeyValuePair<string, object>> worldState,
                                  KeyValuePair<string, object> goal) {
        this.actionList = new List<GoapAction>();
        this.usedActionList = new HashSet<GoapAction>();

        // reset the actions so we can start fresh with them
        foreach (GoapAction a in availableActions) {
            actionList.Add(a);
            a.doReset();
        }
             
        Debug.Log("Start World:" + GoapAgent.prettyPrint(worldState));

        // check what actions can run using their checkProceduralPrecondigoaltion
        List<GoapAction> usableActions = new List<GoapAction>();
        foreach (GoapAction a in availableActions) {
            if (a.checkProceduralPrecondition(worldState)) {
                usableActions.Add(a);
                usedActionList.Add(a);
            }
        }
        // build up the tree and record the leaf nodes that provide a solution to the goal.
        List<Node> leaves = new List<Node>();

        // build graph
        Node start = new Node(null, 0, worldState, null);
        bool success = false;
        KeyValuePair<string, object> subGoal;

        if (goal.Key.Contains("Pe")) {
            subGoal = new KeyValuePair<string, object>("InCi", 1);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];
        }

        if (goal.Key.Contains("Su")) {

            subGoal = new KeyValuePair<string, object>("InSa", 1);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            subGoal = new KeyValuePair<string, object>("InCl", 1);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

            subGoal = new KeyValuePair<string, object>("InCi", 1);
            buildGraph(start, leaves, usableActions, subGoal);
            start = leaves[leaves.Count - 1];

        }

        buildGraph(start, leaves, usableActions, goal);
        start = leaves[leaves.Count - 1];
        
        //Store to Caravan
        subGoal = new KeyValuePair<string, object>("Ca" + goal.Key.Substring(2), goal.Value);
        GoapAction actionStore = actionList.Find(e => e.GetType().Name.Equals("InventoryToCar"));
        usableActions = new List<GoapAction>{ actionStore };
        success = buildGraph(start, leaves, usableActions, subGoal);
        start = leaves[leaves.Count - 1];

        Debug.Log("End State:" + GoapAgent.prettyPrint(start.state));

        if (!success) {
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

    public List<KeyValuePair<string, object>> findSubGoal(List<KeyValuePair<string, object>> goal) {
        List<KeyValuePair<string, object>> subGoal = new List<KeyValuePair<string, object>>() {
            new KeyValuePair<string, object>("Capacity", 2)
        };
        return subGoal;
    }

    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest cost will be the best action
	 * sequence.
	 */

    private bool buildGraph(Node parent, List<Node> leaves, List<GoapAction> usableActions, KeyValuePair<string, object> goal) {

        List<KeyValuePair<string, object>> currentState;

        if (goal.Key.Equals("InCi")) {
            GoapAction action = usableActions[0];
            currentState = populateState(parent.state, action.Effects);
            Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
            leaves.Add(node);

            if ((int)currentState.Find(e => e.Key.Equals("InTu")).Value >= 4) {
                // we found a solution!        
                GoapAction actionD = actionList.Find(e => e.GetType().Name.Equals("TradeWithD"));
                List<GoapAction> subset = new List<GoapAction> {
                    actionD
                };

                //Debug.Log("New actions: " + GoapAgent.prettyPrint(subset));
                return buildGraph(node, leaves, subset, goal);
            }
            if (inState(goal, currentState)) {
                return true;
            } else {
                GoapAction actionA = actionList.Find(e => e.GetType().Name.Equals("TradeWithA"));
                List<GoapAction> subset = new List<GoapAction> {
                    actionA
                };

                //Debug.Log("New actions: " + GoapAgent.prettyPrint(subset));
                return buildGraph(node, leaves, subset, goal);
            }
        }

        foreach (GoapAction action in usableActions) {

            if (inState(action.Preconditions, parent.state)) {
                if (action.GetType().Name.Equals("InventoryToCar")) {
                    currentState = popluateStateForInventory(parent.state);
                } else if (action.GetType().Name.Equals("CarToInventory")) {
                    currentState = populateState(parent.state, action.Effects);
                } else {
                    currentState = populateState(parent.state, action.Effects);
                }

                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
                leaves.Add(node);
                //Debug.Log(GoapAgent.prettyPrint(currentState));
                if (inState(goal, currentState)) {
                    // we found a solution!                    
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
        /**
         * List<KeyValuePair<string, object>> currentState;

        //for (int i = 0; i < usableActions.Count; i++) {
        //    GoapAction action = usableActions[i];

        //    List<KeyValuePair<string, object>> subGoal = action.findSubGoal(goal);
        //    KeyValuePair<string, object> match = findMatchSubGoal(subGoal, parent.state);
        //    if (match.Equals(new KeyValuePair<string, object>()){

        //    }

        //    // if the parent state has the conditions for this action's preconditions, we can use it here
        //    if (inState(action.Preconditions, parent.state)) {

        //        // apply the action's effects to the parent state
        //        if (action.GetType().Name.Equals("InventoryToCar")) {
        //            currentState = popluateStateForInventory(parent.state);
        //        } else if (action.GetType().Name.Equals("CarToInventory")) {
        //            currentState = populateState(parent.state, action.Effects);
        //        } else {
        //            currentState = populateState(parent.state, action.Effects);
        //        }
        //        //Debug.Log(GoapAgent.prettyPrint(currentState));
        //        Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
        //        leaves.Add(node);

        //        if (inState(goal, currentState)) {
        //            bool found = buildGraph(node, leaves, subset, goal);
        //            if (found)
        //                foundOne = true;
        //            foundOne = true;
        //        } else {
        //            // not at a solution yet, so test all the remaining actions and branch out the tree
        //            List<GoapAction> subset = findActions(parent.state, action);
        //            node = new Node(parent, parent.runningCost + action.cost, parent.state, action);
        //            bool found = buildGraph(node, leaves, subset, goal);
        //            if (found)
        //                foundOne = true;
        //        }
        //    }                   

        //}
        return foundOne;
        */
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

    //private bool inState(List<KeyValuePair<string, object>> test, List<KeyValuePair<string, object>> state) {
    //    bool allMatch = true;
    //    foreach (KeyValuePair<string, object> t in test) {
    //        bool match = false;
    //        foreach (KeyValuePair<string, object> s in state) {
    //            if (s.Key.Contains(t.Key.Substring(2))) {
    //                //match = (int)s.Value >= (int)t.Value;
    //                if (match)
    //                    break;
    //            }
    //        }
    //        if (!match)
    //            allMatch = false;
    //    }
    //    return allMatch;
    //}

    private KeyValuePair<string, object> findMatchSubGoal(List<KeyValuePair<string, object>> goal, List<KeyValuePair<string, object>> state) {

        foreach (KeyValuePair<string, object> t in goal) {

            foreach (KeyValuePair<string, object> s in state) {
                if (s.Key.Equals(t.Key)) {
                    if ((int)s.Value >= (int)t.Value)
                        return s;
                }
            }
        }

        return new KeyValuePair<string, object>();
    }

    private List<KeyValuePair<string, object>> popluateStateForInventory(List<KeyValuePair<string, object>> state) {

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

    //private List<KeyValuePair<string, object>> popluateStateForCaravan(List<KeyValuePair<string, object>> state) {

    //    List<KeyValuePair<string, object>> tempState = new List<KeyValuePair<string, object>>();
    //    List<KeyValuePair<string, object>> currentState = new List<KeyValuePair<string, object>>();

    //    foreach (KeyValuePair<string, object> s in state) {
    //        tempState.Add(new KeyValuePair<string, object>(s.Key, s.Value));
    //    }

    //    foreach (KeyValuePair<string, object> inState in tempState) {
    //        KeyValuePair<string, object> updated;
    //        if (inState.Key.Contains("In")) {
    //            int inValue = (int)inState.Value;
    //            string item = "Ca" + inState.Key.Substring(2);

    //            foreach (KeyValuePair<string, object> caState in tempState) {
    //                if (caState.Key.Equals(item)) {
    //                    updated = new KeyValuePair<string, object>(caState.Key, (int)caState.Value + inValue);
    //                    currentState.Add(updated);
    //                    break;
    //                }
    //            }
    //            updated = new KeyValuePair<string, object>(inState.Key, 0);
    //            currentState.Add(updated);

    //        } else if (inState.Key.Equals("Capacity")) {
    //            currentState.Add(new KeyValuePair<string, object>(inState.Key, 4));
    //        }
    //    }
    //    return currentState;
    //}

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


