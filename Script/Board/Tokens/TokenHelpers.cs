
public static class TokenHelpers
{
    public static float SizeEnumToNavRadius(this TokenSizes size)
    {
        return size switch
        {
            TokenSizes.Small => 0.5f,
            TokenSizes.Large => 2f,
            TokenSizes.Huge => 4f,
            TokenSizes.Gargantuan => 8f,

            _ or TokenSizes.Medium => 1,
        };
    }

    public static float SizeEnumToCollisionRadius(this TokenSizes size)
    {
        return size switch
        {
            TokenSizes.Small => 0.5f,
            TokenSizes.Large => 2f,
            TokenSizes.Huge => 4f,
            TokenSizes.Gargantuan => 8f,

            _ or TokenSizes.Medium => 1,
        };
    }

    public static float SpeedEnumToMoveSpeed(this TokenSpeeds speed)
    {
        return speed switch
        {
            TokenSpeeds.VerySlow => 4f,
            TokenSpeeds.Slow => 7f,
            TokenSpeeds.Fast => 13f,
            TokenSpeeds.VeryFast => 18f,

            _ or TokenSpeeds.Normal => 10f,
        };
    }
}
