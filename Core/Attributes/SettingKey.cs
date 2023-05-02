namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class SettingKeyAttribute : Attribute
{
    public SettingKeyAttribute(string keyName)
    {
        KeyName = keyName.ToLower();
    }

    public string KeyName { get; set; }
}
