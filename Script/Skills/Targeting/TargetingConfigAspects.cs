
public static class TargetingConfigAspects
{
    public interface IHasRange
    {
        float Range { get; set; }
    }

    public interface IHasRadius
    {
        float Radius { get; set; }
    }

    public interface IHasLength
    {
        float Length { get; set; }
    }

    public interface IHasWidth
    {
        float Width { get; set; }
    }
}
