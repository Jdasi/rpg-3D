
public class CharacterManager
{
    public static CharacterId NextCharacterId => ++_lastCharacterId;

    private static CharacterId _lastCharacterId;
}
