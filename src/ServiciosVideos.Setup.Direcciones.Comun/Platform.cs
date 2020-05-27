using System.IO;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class Platform
    {
        public static bool IsWindows => System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
                

        public static bool IsLinuxSystem(DriveInfo driveInfo)
        {
            var mount = driveInfo.Name;
            return mount == "/"
                || mount == "/var"
                || mount == "/usr" || mount == "/local" || mount == "/lib" || mount == "/opt"
                || mount == "/dev" || mount == "/run" || mount == "/sys" || mount == "/boot"
                || mount == "/proc" || mount == "/snap"
                || mount.StartsWith("/var/")
                || mount.StartsWith("/usr/") || mount.StartsWith("/local/") || mount.StartsWith("/lib/") || mount.StartsWith("/opt/")
                || mount.StartsWith("/dev/") || mount.StartsWith("/run/") || mount.StartsWith("/sys/") || mount.StartsWith("/boot/")
                || mount.StartsWith("/proc") || mount.StartsWith("/snap/");
        }
    }
}



