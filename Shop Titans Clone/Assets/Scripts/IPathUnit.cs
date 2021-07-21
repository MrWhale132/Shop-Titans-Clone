using System;

public interface IPathUnit
{
    bool OnPath { get; }
    GridNode GetDestinationNode();
    void StepAwayFrom(GridNode from, Action<Action> stepAwayCompleted);
}