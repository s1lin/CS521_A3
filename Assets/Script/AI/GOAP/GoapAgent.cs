﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class GoapAgent : MonoBehaviour {

    private FSM stateMachine;

    private FSM.FSMState idleState; // finds something to do
    private FSM.FSMState moveToState; // moves to a target
    private FSM.FSMState performActionState; // performs an action

    private List<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    private GoapPlanner planner;


    void Start() {
        stateMachine = new FSM();
        availableActions = new List<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        findDataProvider();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.pushState(idleState);
        loadActions();
    }


    void Update() {
        stateMachine.Update(this.gameObject);
    }


    public void addAction(GoapAction a) {
        availableActions.Add(a);
    }

    public GoapAction getAction(Type action) {
        foreach (GoapAction g in availableActions) {
            if (g.GetType().Equals(action))
                return g;
        }
        return null;
    }

    public void removeAction(GoapAction action) {
        availableActions.Remove(action);
    }

    private bool hasActionPlan() {
        return currentActions.Count > 0;
    }

    private void createIdleState() {
        idleState = (fsm, gameObj) => {

            List<KeyValuePair<string, object>> worldState = dataProvider.GetWorldState();

            KeyValuePair<string, object> goal = dataProvider.GetSubGoals();


            if (!goal.Equals(new KeyValuePair<string, object>())) {
                // Plan
                Queue<GoapAction> plan = planner.plan(availableActions, worldState, goal);

                if (plan != null) {
                    // we have a plan, hooray!
                    currentActions = plan;
                    dataProvider.PlanFound(goal, plan);

                    fsm.popState(); // move to PerformAction state
                    fsm.pushState(performActionState);

                } else {

                    dataProvider.PlanFailed(goal);
                    fsm.popState(); // move back to IdleAction state
                    fsm.pushState(idleState);
                }
            } else {
                dataProvider.GameFinished();
            }

        };
    }

    private void createMoveToState() {
        moveToState = (fsm, gameObj) => {
            // move the game object
            GoapAction action = currentActions.Peek();
            // get the agent to move itself
            if (dataProvider.MoveAgent(action)) {
                fsm.popState();
            }
        };
    }

    private void createPerformActionState() {

        performActionState = (fsm, gameObj) => {
            // perform the action

            if (!hasActionPlan()) {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(idleState);

                return;
            }

            GoapAction action = currentActions.Peek();



            if (action.isDone()) {
                // the action is done. Remove it so we can perform the next one
                action.doReset();
                currentActions.Dequeue();
            }

            if (hasActionPlan()) {
                // perform the next action
                action = currentActions.Peek();
                bool inRange = action.isInRange();

                if (!action.inWait) {
                    if (inRange) {
                        //if (!action.inWait) {
                        // we are in range, so perform the action
                        bool success = action.perform(gameObj);
                        if (!action.inWait) {
                            print(action + success.ToString());
                            if (!success) {
                                // action failed, we need to plan again
                                fsm.popState();
                                fsm.pushState(idleState);
                                dataProvider.PlanAborted(action);
                            }
                        }

                    } else {
                        // we need to move there first
                        // push moveTo state                   
                        fsm.pushState(moveToState);
                    }
                }


            } else {
                Debug.Log(prettyPrint(action));
                // no actions left, move to Plan state
                fsm.popState();
                fsm.pushState(idleState);
            }


        };
    }

    private void findDataProvider() {
        foreach (Component comp in gameObject.GetComponents(typeof(Component))) {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType())) {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void loadActions() {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions) {
            availableActions.Add(a);
        }
        Debug.Log("Found actions: " + prettyPrint(actions));
    }

    public static string prettyPrint(List<KeyValuePair<string, object>> state) {
        String s = "";
        foreach (KeyValuePair<string, object> kvp in state) {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(GoapAction[] actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string prettyPrint(Queue<GoapAction> actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string prettyPrint(List<GoapAction> actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(GoapAction action) {
        String s = "" + action.GetType().Name;
        return s;
    }
}
