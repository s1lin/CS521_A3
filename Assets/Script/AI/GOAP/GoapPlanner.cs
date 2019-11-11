using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */
public class GoapPlanner {

    private HashSet<GoapAction> allActions;
    private GameObject agent;
    private HashSet<KeyValuePair<string, object>> worldState;
    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */
    public Queue<GoapAction> plan(GameObject agent,
                                  HashSet<GoapAction> availableActions,
                                  HashSet<KeyValuePair<string, object>> worldState,
                                  HashSet<KeyValuePair<string, object>> goal) {
        // reset the actions so we can start fresh with them
        foreach (GoapAction a in availableActions) {
            a.doReset();
        }

        allActions = availableActions;
        this.agent = agent;
        this.worldState = worldState;

        Debug.Log(GoapAgent.prettyPrint(worldState));
        // check what actions can run using their checkProceduralPrecondition
        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
        foreach (GoapAction a in availableActions) {

            if (a.checkProceduralPrecondition(worldState)) {
                Debug.Log(a.GetType().Name + ":" + a.checkProceduralPrecondition(worldState));
                usableActions.Add(a);
            }
        }

        // we now have all actions that can run, stored in usableActions

        // build up the tree and record the leaf nodes that provide a solution to the goal.
        List<Node> leaves = new List<Node>();

        // build graph
        Node start = new Node(null, 0, worldState, null);
        bool success = buildGraph(start, leaves, usableActions, goal);

        if (!success) {
            // oh no, we didn't get a plan
            Debug.Log("NO PLAN");
            return null;
        }

        // get the cheapest leaf
        Node cheapest = null;
        foreach (Node leaf in leaves) {
            if (cheapest == null)
                cheapest = leaf;
            else {
                if (leaf.runningCost < cheapest.runningCost)
                    cheapest = leaf;
            }
        }

        // get its node and work back through the parents
        List<GoapAction> result = new List<GoapAction>();
        Node n = cheapest;
        while (n != null) {
            if (n.action != null) {
                result.Insert(0, n.action); // insert the action in the front
            }
            n = n.parent;
        }
        // we now have this action list in correct order

        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (GoapAction a in result) {
            queue.Enqueue(a);
        }

        // hooray we have a plan!
        return queue;
    }

    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest cost will be the best action
	 * sequence.
	 */
    private bool buildGraph(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, 
        HashSet<KeyValuePair<string, object>> goal) {
        bool foundOne = false;

        // go through each action available at this node and see if we can use it here
        foreach (GoapAction action in usableActions) {

            // if the parent state has the conditions for this action's preconditions, we can use it here
            if (inState(action.Preconditions, parent.state)) {

                HashSet<KeyValuePair<string, object>> currentState = new HashSet<KeyValuePair<string, object>>();
                               
                if (action.GetType().Equals("InventoryToCar")) {

                    foreach (KeyValuePair<string, object> s in currentState) {
                        currentState.Add(new KeyValuePair<string, object>(s.Key, s.Value));
                    }
                   
                    foreach (KeyValuePair<string, object> inState in currentState) {
                        KeyValuePair<string, object> updated;
                        if (inState.Key.Contains("In")) {
                            int inValue = (int) inState.Value;
                            string item = inState.Key.Substring(2);
                            
                            foreach(KeyValuePair<string, object> caState in currentState) {
                                if (caState.Key.Contains(item)) {
                                    parent.state.Remove(caState);
                                    updated = new KeyValuePair<string, object>(caState.Key, (int)caState.Value + inValue);
                                    parent.state.Add(updated);
                                    break;
                                }
                            }
                            updated = new KeyValuePair<string, object>(inState.Key, 0);
                            currentState.Remove(inState);
                            currentState.Add(updated);

                        } else if(inState.Key.Equals("Capacity")){
                            currentState.Add(new KeyValuePair<string, object>(inState.Key, 4));
                        }                       
                    }
                                   
                } else if (action.GetType().Equals("CarToInventory")) {
                    currentState = populateState(parent.state, action.Effects);
                } else {
                    // apply the action's effects to the parent state
                    currentState = populateState(parent.state, action.Effects);
                }

                Debug.Log(GoapAgent.prettyPrint(currentState));
                
                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
                Debug.Log(GoapAgent.prettyPrint(goal));

                if (inState(goal, currentState)) {
                    // we found a solution!
                    leaves.Add(node);
                    foundOne = true;
                } else {
                    // not at a solution yet, so test all the remaining actions and branch out the tree
                    HashSet<GoapAction> subset = findActions(currentState, action);
                    bool found = buildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    /**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */
    private HashSet<GoapAction> findActions(HashSet<KeyValuePair<string, object>> currentState, GoapAction exclude) {
        HashSet<GoapAction> newActions = new HashSet<GoapAction>();
        foreach (GoapAction a in allActions) {
            if (!a.Equals(exclude)) {
                if (a.checkProceduralPrecondition(currentState)) {
                    newActions.Add(a);
                }
            }
        }
        Debug.Log("New actions: " + GoapAgent.prettyPrint(newActions));
        return newActions;
    }

    /**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */
    private bool inState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state) {
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

    /**
	 * Apply the stateChange to the currentState
	 */
    private HashSet<KeyValuePair<string, object>> populateState(
        HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange) {

        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();
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
                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
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
        public HashSet<KeyValuePair<string, object>> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GoapAction action) {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

}


