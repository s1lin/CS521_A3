using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class GoapAgent : MonoBehaviour {

    private FSM fsm;
    private FSM.FSMState idle;  
    private FSM.FSMState move;  
    private FSM.FSMState act;

    private List<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private IGoap goapAgent; 

    private GoapPlanner planner;

    void Start() {
        fsm = new FSM();
        availableActions = new List<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        
        LoadAgent();

        //FSM:
        IdleState();
        MoveState();
        ActionState();

        fsm.pushState(idle);

        LoadActions();
    }


    void Update() {
        fsm.Update(gameObject);
    }

    private void IdleState() {
        idle = (fsm, gameObj) => {
            List<KeyValuePair<string, object>> worldState = goapAgent.GetWorldState();

            KeyValuePair<string, object> goal = goapAgent.GetSubGoals();
            planner.Reset();
            if (!goal.Equals(new KeyValuePair<string, object>())) {
                Queue<GoapAction> plan = planner.Plan(availableActions, worldState, goal);

                if (plan != null) {
                    // we have a plan, hooray!
                    currentActions = plan;
                    goapAgent.PlanFound(goal, plan);

                    fsm.popState();
                    fsm.pushState(act);

                } else {

                    goapAgent.PlanFailed(goal);
                    fsm.popState(); 
                    fsm.pushState(idle);
                }
            } else {
                goapAgent.GameFinished();
            }

        };
    }

    private void MoveState() {
        move = (fsm, gameObj) => {
            GoapAction action = currentActions.Peek();
            if (goapAgent.MoveAgent(action)) {
                fsm.popState();
            }
        };
    }

    private void ActionState() {
        act = (fsm, gameObj) => {
            if (currentActions.Count == 0) {
                fsm.popState();
                fsm.pushState(idle);
                return;
            }

            GoapAction action = currentActions.Peek();

            if (!action.init) {
                if (!action.inWait) {
                    bool success = action.IsSucc();
                    if (!success) {
                        fsm.popState();
                        fsm.pushState(idle);
                        goapAgent.PlanAborted(action);
                        action.DoReset();
                        return;
                    }

                    if (action.IsDone()) {
                        action.DoReset();
                        goapAgent.ActionFinished(action);
                        currentActions.Dequeue();
                    }
                } else {
                    return;
                }                                            
            }

            if (currentActions.Count > 0) {       
                action = currentActions.Peek();

                if (action.IsInRange()) {
                    action.DoAction(gameObj);
                    action.init = false;
                } else
                    fsm.pushState(move);           

            } else {
                fsm.popState();
                fsm.pushState(idle);
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
        String text = "";
        foreach (KeyValuePair<string, object> s in state) {
            text += s.Key + ":" + s.Value.ToString();
            text += ", ";
        }
        return text;
    }

    public static string Display(GoapAction[] actions) {
        String text = "";
        foreach (GoapAction a in actions) {
            text += a.GetType().Name;
            text += "-> ";
        }
        text += "GOAL";
        return text;
    }

    public static string Display(GoapAction action) {
        String text = "" + action.GetType().Name;
        return text;
    }
}
