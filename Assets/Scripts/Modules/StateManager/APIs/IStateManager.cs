public interface IStateManager
{
    void MoveToNextState();

    void TriggerEvent(string eventKey);
}
