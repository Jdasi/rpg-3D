
public static class LocalEvents
{
    public static EventCallback ServerStarted;
    public static EventCallback Disconnected;
    public static EventCallback<PlayerId> PlayerAdded;
    public static EventCallback<PlayerId> PlayerRemoved;
    public static EventCallback<NetworkEntity> EntityDestroyed;

    public static EventCallback<TabletopCamera> TabletopCameraMoved;
    public static EventCallback<Token> TurnStarted;
    public static EventCallback<LocalEventData.SkillStarted> SkillStarted;
    public static EventCallback<LocalEventData.SkillFinished> SkillFinished;
    public static EventCallback<LocalEventData.BarrierBroken> BarrierBroken;
    public static EventCallback<LocalEventData.CharacterHealthChanged> CharacterHealthChanged;
    public static EventCallback<LocalEventData.CharacterHealthEffect> CharacterHealthEffect;
    public static EventCallback<LocalEventData.CharacterConditionAdded> CharacterConditionAdded;
    public static EventCallback<LocalEventData.CharacterConditionRemoved> CharacterConditionRemoved;
}
