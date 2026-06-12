namespace ManageUsers.Application.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]

public sealed class SettingsAttribute : Attribute
{
    public int Key { get; }
    public string DefaultValue { get; set; }
    public double MinimumLimitation { get; }
    public double MaximumLimitation { get; }
    public string Description { get; }


    public SettingsAttribute(
        int key,
        string defaultValue,
        double minimumLimitation,
        double maximumLimitation,
        string description)
    {
        Key = key;
        DefaultValue = defaultValue;
        MinimumLimitation = minimumLimitation;
        MaximumLimitation = maximumLimitation;
        Description = description;
    }
}
