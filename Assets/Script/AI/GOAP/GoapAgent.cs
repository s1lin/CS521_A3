using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class GoapAgent : MonoBehaviour {

    private FSM stateMachine;

    private FSM.FSMState idleState; // finds something to do
    private FSM.FSMState moveToState; // moves to a target
    private FSM.FSMState actionState; // performs an action

    private List<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private IGoap goapAgent; 

    private GoapPlanner planner;

    void Start() {
        stateMachine = new FSM();
        availableActions = new List<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        
        LoadAgent();

        //FSM:
        IdleState();
        MoveState();
        ActionState();

        stateMachine.pushState(idleState);

        LoadActions();
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

    private void IdleState() {
        idleState = (fsm, gameObj) => {

            List<KeyValuePair<string, object>> worldState = goapAgent.GetWorldState();

            KeyValuePair<string, object> goal = goapAgent.GetSubGoals();

            //Debug.Log(prettyPrint(worldState));

            if (!goal.Equals(new KeyValuePair<string, object>())) {
                // Plan
                Queue<GoapAction> plan = planner.Plan(availableActions, worldState, goal);

                if (plan != null) {
                    // we have a plan, hooray!
                    currentActions = plan;
                    goapAgent.PlanFound(goal, plan);

                    fsm.popState(); // move to Action state
                    fsm.pushState(actionState);

                } else {

                    goapAgent.PlanFailed(goal);
                    fsm.popState(); // move back to IdleAction state
                    fsm.pushState(idleState);
                }
            } else {
                goapAgent.GameFinished();
            }

        };
    }

    private void MoveState() {
        moveToState = (fsm, gameObj) => {
            // move the game object
            GoapAction action = currentActions.Peek();
            // get the agent to move itself
            if (goapAgent.MoveAgent(action)) {
                fsm.popState();
            }
        };
    }

    private void ActionState() {

        actionState = (fsm, gameObj) => {
            // perform the action

            if (!hasActionPlan()) {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(idleState);

                return;
            }

            GoapAction action = currentActions.Peek();

            if (!action.init) {
                if (!action.inWait) {
                    bool success = action.IsSucc();
                    if (!success) {
                        // action failed, we need to plan again
                        fsm.popState();
                        fsm.pushState(idleState);
                        goapAgent.PlanAborted(action);
                        action.DoReset();
                        return;
                    }

                    if (action.IsDone()) {
                        // the action is done. Remove it so we can perform the next one
                        action.DoReset();
                        currentActions.Dequeue();
                    }
                } else {
                    return;
                }                                            
            }

            if (hasActionPlan()) {
                // perform the next action             
                action = currentActions.Peek();

                if (action.IsInRange()) {
                    action.DoAction(gameObj);
                    action.init = false;
                } else
                    fsm.pushState(moveToState);           

            } else {
                fsm.popState();
                fsm.pushState(idleState);
            }


        };
    }

    private void LoadAgent() {
        foreach (Component comp in gameObject.GetComponents(typeof(Component))) {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType())) {
                goapAgent = (IGoap)comp;
                return;
            }
        }
    }

    private void LoadActions() {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions) {
            availableActions.Add(a);
        }
        Debug.Log("Found actions: " + Display(actions));
    }

    public static string Display(List<KeyValuePair<string, object>> state) {
        String s = "";
        foreach (KeyValuePair<string, object> kvp in state) {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    public static string Display(GoapAction[] actions) {
        String s = "";
        foreach (GoapAction a in actions) {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string Display(GoapAction action) {
        String s = "" + action.GetType().Name;
        return s;
    }
}
