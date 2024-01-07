using System;

namespace BibitesModsLoader;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ModAttribute : Attribute, IComparable<ModAttribute>
{
    public ModAttribute(string name, string authors, string version)
    {
        Name = name ?? "Unknow";
        Authors = authors ?? "Unknow";
        if (Version != null)
            Version = new(version);
        else
            Version = new(1, 0, 0);
    }

    public string Name { get; }
    public string Authors { get; }
    public Version Version { get; }

    public string Description { get; set; } = "Just a mod ...";
    public byte Nicess { get; set; } = 0;

    public int CompareTo(ModAttribute other) => other.Nicess - Nicess;
}