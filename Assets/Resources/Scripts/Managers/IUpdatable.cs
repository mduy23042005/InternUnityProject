public interface IUpdatable
{
    public void OnEnable();
    public void OnDisable();

    public void OnUpdate();
    public void OnLateUpdate();
    public void OnFixedUpdate();
}