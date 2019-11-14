using System.Collections.Generic;

public interface IGoap {

    List<KeyValuePair<string, object>> GetWorldState();

    KeyValuePair<string, object> GetSubGoals();

    void PlanFailed(KeyValuePair<string, object> failedGoal);

    void PlanFound(KeyValuePair<string, object> goal, Queue<GoapAction> actions);

    void GameFinished();

    void ActionFinished(GoapAction action);

    void PlanAborted(GoapAction aborter);

    bool MoveAgent(GoapAction nextAction);
}

