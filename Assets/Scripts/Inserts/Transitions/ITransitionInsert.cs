using System;


public enum TransitionState
{
    In,
    Out,
    Idle,
    None
}

public interface ITransitionInsert : IInsert
{
    void Initialize(); // initialize to a default state
    void TransitionIn(float duration, Action onComplete = null);
    void TransitionOut(float duration, Action onComplete = null);
    void CancelTransition();
    float PercentComplete { get; }
    TransitionState State { get; }
}