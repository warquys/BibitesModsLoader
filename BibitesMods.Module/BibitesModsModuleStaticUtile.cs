using System.Numerics;
using HarmonyLib;
using Version = Utility.Version;

namespace BibitesMods;

public partial class BibitesModsModule
{
    static BibitesModsModule()
    {
        Version = Version.Parse(ModeInfo.VERSION);
        SubVersion = Version.Parse(ModeInfo.SUB_VERSION);
        BasedGameVersion = Version.Parse(ModeInfo.GAME_VERSION);
    }

    public static Version Version;
    public static Version SubVersion;

#if CUSTOM_VERSION
    public const VersionType Type = VersionType.Beta;
#elif DEBUG
    public const VersionType Type = VersionType.Debug;
#elif DEV
    public const VersionType Type = VersionType.Dev;
#elif RELEASE
    public const VersionType Type = VersionType.None;
#endif

    public static Version BasedGameVersion;

    /// <summary>
    /// Creates a string with the full combined version
    /// </summary>
    public static string GetVersion()
    {
        var version = $"{Version}";

#if CUSTOM_VERSION
        if (Type != VersionType.None)
            version += $"-{Type}-{SubVersion}";
#endif
#if DEBUG
        version += "-Debug";
#endif

        return version;
    }

    public enum VersionType
    {
        None,
        Debug,
        Beta,
        Pre,
        Dev
    }
}
