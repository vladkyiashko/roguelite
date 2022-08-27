public interface IGameEvent<T>
{
    public void RegisterListener(IGameEventListener<T> l);

    public void UnregisterListener(IGameEventListener<T> l);
}
