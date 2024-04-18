using System.Diagnostics;

namespace ERDE_Terminal
{
    public class WiFiManager
    {
        public static void DisconnectAllWiFi()
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "wlan disconnect")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(errors))
                {
                    Console.WriteLine("Error disconnecting WiFi: " + errors);
                }
                else
                {
                    Console.WriteLine("Disconnected successfully. Output: " + output);
                }
            }
        }

        public static void ClearSavedWiFiProfiles()
        {
            ProcessStartInfo listProfilesPsi = new ProcessStartInfo("netsh", "wlan show profiles")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process listProcess = Process.Start(listProfilesPsi))
            {
                listProcess.WaitForExit();
                string output = listProcess.StandardOutput.ReadToEnd();

                // Extract profile names from output
                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("All User Profile"))
                    {
                        string profileName = line.Split(':')[1].Trim();

                        ProcessStartInfo deleteProfilePsi = new ProcessStartInfo("netsh", $"wlan delete profile name=\"{profileName}\"")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        using (Process deleteProcess = Process.Start(deleteProfilePsi))
                        {
                            deleteProcess.WaitForExit();
                            Console.WriteLine($"Deleted WiFi profile: {profileName}");
                        }
                    }
                }
            }
        }
    }
}
