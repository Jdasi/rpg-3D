
public enum BarrierHandle : int { }

public class BarrierInstance
{
    public readonly BarrierHandle Handle;
    public readonly int Priority;
    public readonly ElementTypes AbsorbTypes;

    public int Health;

    public BarrierInstance(BarrierHandle handle, int priority, int health, ElementTypes absorbTypes)
    {
        Handle = handle;
        Priority = priority;
        AbsorbTypes = absorbTypes;
        Health = health;
    }
}
