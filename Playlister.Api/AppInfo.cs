using System;
using System.IO;
using System.Reflection;

// ReSharper disable UnusedMember.Global
namespace Playlister.Api
{
    public class AppInfo
    {
        private static readonly Assembly Assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        public DateTime AssemblyBuildTime { get; } =
            File.GetLastWriteTime(Assembly.Location);

        public string Location { get; } = Assembly.Location;
    }
}
