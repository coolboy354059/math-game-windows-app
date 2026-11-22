using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathGameWindowsApp
{
    /// <summary>
    /// Checks GitHub for a newer version of the game and offers to update.
    ///
    /// How it works:
    /// 1. Ask GitHub for the latest release of the math-game-windows-app repo.
    /// 2. If the release version is newer than this app, ask the player
    ///    "Update now?" with a Yes/No box.
    /// 3. If they say yes, download the new app zip, unzip it, and run a
    ///    little update.bat that copies the new files over the old ones
    ///    and starts the game again.
    ///
    /// If anything goes wrong (no internet, GitHub is down, no releases yet)
    /// the check just quietly gives up so the game still works.
    /// </summary>
    internal static class UpdateChecker
    {
        // The GitHub page that tells us about the newest release.
        private const string LatestReleaseUrl =
            "https://api.github.com/repos/coolboy354059/math-game-windows-app/releases/latest";

        // The file attached to each release that contains the whole app.
        private const string ZipAssetName = "math-game-windows-app.zip";

        public static async Task CheckForUpdatesAsync(Form owner)
        {
            try
            {
                // --- Step 1: ask GitHub for the latest release. ---
                string tagName;
                string downloadUrl;

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MathGameWindowsApp");
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

                    string json = await client.GetStringAsync(LatestReleaseUrl);
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    tagName = root.GetProperty("tag_name").GetString() ?? "";

                    // Find the zip file in the release's list of files.
                    downloadUrl = "";
                    foreach (JsonElement asset in root.GetProperty("assets").EnumerateArray())
                    {
                        if (asset.GetProperty("name").GetString() == ZipAssetName)
                        {
                            downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? "";
                            break;
                        }
                    }
                }

                if (downloadUrl == "")
                {
                    return; // No zip attached to the release, nothing to update with.
                }

                // --- Step 2: is the release newer than this app? ---
                if (!TryParseVersion(tagName, out Version latestVersion))
                {
                    return;
                }

                Version currentVersion = Normalize(
                    Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0));

                if (latestVersion <= currentVersion)
                {
                    return; // Already up to date.
                }

                // --- Step 3: ask the player. ---
                DialogResult answer = MessageBox.Show(
                    owner,
                    $"A new version of Math Game is ready (v{latestVersion}). Update now?",
                    "Math Game Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                {
                    return;
                }

                // --- Step 4: download and install the update. ---
                await DownloadAndInstallAsync(owner, downloadUrl);
            }
            catch
            {
                // No internet, GitHub is down, no releases yet, anything at
                // all: stay quiet and let the game keep running.
            }
        }

        private static async Task DownloadAndInstallAsync(Form owner, string downloadUrl)
        {
            try
            {
                // Work in a temp folder so we never touch the app folder
                // until the new files are fully downloaded and unzipped.
                string tempFolder = Path.Combine(Path.GetTempPath(), "MathGameUpdate");
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, recursive: true);
                }
                Directory.CreateDirectory(tempFolder);

                string zipPath = Path.Combine(tempFolder, ZipAssetName);
                string extractFolder = Path.Combine(tempFolder, "newapp");

                // Download the zip (streamed to a file, it can be big).
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MathGameWindowsApp");

                    using HttpResponseMessage response = await client.GetAsync(
                        downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using Stream download = await response.Content.ReadAsStreamAsync();
                    using FileStream file = File.Create(zipPath);
                    await download.CopyToAsync(file);
                }

                // Unzip the new app files.
                ZipFile.ExtractToDirectory(zipPath, extractFolder);

                // Write a little script that waits for this app to close,
                // copies the new files over the old ones, starts the game
                // again, and then deletes itself.
                string appFolder = AppContext.BaseDirectory;
                string exePath = Path.Combine(appFolder, "MathGameWindowsApp.exe");
                string batPath = Path.Combine(tempFolder, "update.bat");

                string batText =
                    "@echo off\r\n" +
                    "timeout /t 2 /nobreak >nul\r\n" +
                    $"xcopy /E /Y \"{extractFolder}\\*\" \"{appFolder}\"\r\n" +
                    $"start \"\" \"{exePath}\"\r\n" +
                    "del \"%~f0\"\r\n";
                File.WriteAllText(batPath, batText);

                // Run the script hidden, then close this app so the script
                // can replace the files.
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batPath}\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                };
                Process.Start(startInfo);

                Application.Exit();
            }
            catch
            {
                // The update failed before anything was replaced, so the old
                // version is still fine. Just let the player know.
                MessageBox.Show(
                    owner,
                    "Update failed, try again later.",
                    "Math Game Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // Turns a release tag like "v1.0.1" into a Version we can compare.
        private static bool TryParseVersion(string tagName, out Version version)
        {
            string text = tagName.Trim();
            if (text.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(1);
            }

            if (Version.TryParse(text, out Version? parsed) && parsed != null)
            {
                version = Normalize(parsed);
                return true;
            }

            version = new Version(0, 0, 0);
            return false;
        }

        // Makes every version exactly three numbers (major.minor.build) so
        // comparisons are fair. A missing number counts as 0.
        private static Version Normalize(Version version)
        {
            return new Version(
                Math.Max(version.Major, 0),
                Math.Max(version.Minor, 0),
                Math.Max(version.Build, 0));
        }
    }
}
