using System.Reflection;

namespace Playlister.Models;

public class AppInfo {
    private static readonly Assembly s_assembly = Assembly.GetEntryAssembly( ) ?? Assembly.GetCallingAssembly( );

    public DateTime AssemblyBuildTime { get; } =
        File.GetLastWriteTime( s_assembly.Location );

    public string Location { get; } = s_assembly.Location;
}
