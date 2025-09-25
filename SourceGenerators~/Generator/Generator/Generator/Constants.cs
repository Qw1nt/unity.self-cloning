namespace Generator;

public class Constants
{
    public class Namespaces
    {
        public const string Attribute = "SelfCloning.Attributes";
        public const string Interfaces = "SelfCloning.Interfaces";
    }

    public class Attributes
    {
        public const string SelfCloneableAttribute = Namespaces.Attribute + "." + nameof(SelfCloneableAttribute);
    }

    public class FullPath
    {
        public const string ISelfCloneable = Namespaces.Interfaces + "." + nameof(ISelfCloneable);
    }
}