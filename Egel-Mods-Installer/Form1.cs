using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;

namespace Egel_Mods_Installer
{
    public partial class Form1 : Form
    {
        #region Initialization
        readonly static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        static dynamic versions;
        static string selectedVersion;
        static string loadedVersion;

        readonly string remoteData = "https://egelbank.nl/EgelMods/data.json";

        readonly string path = $"{appData}/.minecraft/";
        readonly string egelPath = $"{appData}/.minecraft/.egel/";
        readonly string modsPath = $"{appData}/.minecraft/mods/";
        readonly string modsPathUser = $"{appData}/.minecraft/mods/user_mods/";
        readonly string versionsPathRoot = $"{appData}/.minecraft/versions/";
        string versionsPath;

        static string[] downloadUrls;

        int linkCount;

        string fabricClientJar;
        string fabricClientJson;

        public Form1()
        {
            InitializeComponent();

            string json;
            using (WebClient webClient = new WebClient())
            {
                json = webClient.DownloadString(remoteData);
            }

            dynamic data = JsonConvert.DeserializeObject(json);

            versionSelect.Items.Add("Jouw mods");

            // Take latest version as default
            selectedVersion = data.latest.ToString();

            if (Directory.Exists(egelPath))
            {
                selectedVersion = File.ReadAllText(egelPath + "loadedVersion.json");

                if (String.IsNullOrEmpty(selectedVersion))
                {
                    selectedVersion = "Jouw mods";
                }
            }

            // Get all possible versions
            versions = data.versions;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(versions);
            foreach (PropertyDescriptor property in properties)
            {
                versionSelect.Items.Add(property.Name);
            }

            if (Directory.Exists(egelPath))
            {
                string defaultSelected = File.ReadAllText(egelPath + "loadedVersion.json");

                if (!String.IsNullOrEmpty(defaultSelected))
                {
                    loadedVersion = defaultSelected;
                } else
                {
                    loadedVersion = "Jouw mods";
                }

                versionSelect.Items[versionSelect.FindStringExact(loadedVersion)] += " ✔";
            }

            ChangeSelectedVersion(versions, selectedVersion);

            if (!Directory.Exists(egelPath)) {
                DirectoryInfo di = Directory.CreateDirectory(egelPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            if (!File.Exists(egelPath + "loadedVersion.json")) {
                File.Create(egelPath + "loadedVersion.json").Close();
            }
        }
        #endregion

        #region Button Click Events
        private void install_Click(object sender, EventArgs e)
        {
            try
            {
                progress.Focus();
                if (String.IsNullOrEmpty(selectedVersion)) throw new Exception("No version set");

                if (selectedVersion == "Jouw mods") return;

                DisableButtons();

                error.Text = "";
                error.Update();

                if (appData.Length == 0) return;

                if (GetInstalledVersions().Contains(selectedVersion)) throw new Exception("Deze versie is al geïnstalleerd");

                //Move all existing mods to ../.minecraft/mods/user_mods/
                if (Directory.Exists(modsPath) && !Directory.Exists(modsPathUser))
                {
                    progress.Text = "Bestaande mods gevonden";
                    progress.Update();

                    Directory.CreateDirectory(modsPathUser);

                    if (Directory.EnumerateFileSystemEntries(modsPath).Any())
                    {
                        progress.Text = "Bestaande mods verplaatsen...";
                        progress.Update();
                        string[] oldMods = Directory.GetFiles(modsPath, "*.jar", SearchOption.TopDirectoryOnly);
                        foreach (string modPath in oldMods)
                        {
                            File.Copy(modPath, modPath.Replace(modsPath, modsPathUser));
                            File.Delete(modPath);
                        }
                    }

                    Directory.CreateDirectory(modsPath + selectedVersion);
                    ChangeLoadedVersion(selectedVersion);
                }
                // If you already have some other version(s) installed
                else if (Directory.Exists(modsPath) && Directory.Exists(modsPathUser) && Directory.EnumerateFileSystemEntries(modsPath).Any())
                {
                    Directory.CreateDirectory(modsPath + selectedVersion);
                    ChangeLoadedVersion(selectedVersion);
                }
                else
                {
                    progress.Text = "Geen bestaande mods gevonden";
                    progress.Update();
                    Directory.CreateDirectory(modsPath);
                }

                progress.Text = "Wachten op mods downloaden...";
                progress.Update();
                int i = 1;

                using (WebClient webClient = new WebClient())
                {
                    //Download mods one by one and put them in the mods folder
                    foreach (string url in downloadUrls)
                    {
                        //Progress + 1
                        progress.Text = "Mods aan het downloaden: " + Convert.ToString(i) + " van " + Convert.ToString(linkCount);
                        progress.Update();
                        i++;

                        string fileName = url.Substring(url.LastIndexOf('/') + 1);
                        fileName = fileName.Replace("%2B", "-");

                        webClient.DownloadFile(url, modsPath + fileName);
                    }

                    progress.Text = "Custom minecraft installatie toevoegen";
                    progress.Update();

                    //Prepare custom MC version (Egel-version) in ../.minecraft/versions/

                    Directory.CreateDirectory(versionsPath);
                    webClient.DownloadFile(fabricClientJar, versionsPath + $"Egel-{selectedVersion}.jar");
                    webClient.DownloadFile(fabricClientJson, versionsPath + $"Egel-{selectedVersion}.json");
                }

                progress.Text = "Launcher profiel toevoegen";
                progress.Update();

                // Edit the launcher_profiles.json file
                string json;
                using (StreamReader sr = new StreamReader(path + "launcher_profiles.json"))
                {
                    json = sr.ReadToEnd();
                    sr.Close();
                }

                JObject config = JObject.Parse(json);

                JObject profiles = config["profiles"] as JObject;

                if (profiles[$"Egel-{selectedVersion}"] != null)
                {
                    profiles.Remove($"Egel-{selectedVersion}");
                }

                dynamic newProfile = new JObject();
                newProfile.lastUsed = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                newProfile.lastVersionId = $"Egel-{selectedVersion}";
                newProfile.created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                newProfile.icon = "data:image/png;base64,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";
                newProfile.name = "Egel Mods";
                newProfile.type = "custom";

                profiles.Add($"Egel-{selectedVersion}", newProfile);

                // Get new json
                string newJson = config.ToString();

                // Write to file
                File.WriteAllText(path + "launcher_profiles.json", newJson);
                progress.Text = "Succesvol geïnstalleerd!";
                progress.Update();

                EnableButtons();
            } catch (Exception ex)
            {
                EnableButtons();

                error.Text = ex.Message;
                error.Update();
                progress.Text = "Klik op installeren om te beginnen";
                progress.Update();
            }
        }

        private void uninstall_Click(object sender, EventArgs e)
        {
            try
            {
                progress.Focus();
                if (selectedVersion == "Jouw mods") return;

                DisableButtons();

                error.Text = "";
                error.Update();


                progress.Text = "Custom Minecraft versie verwijderen...";
                progress.Update();

                if (!Directory.Exists(versionsPath))
                {
                    throw new Exception("Kan geen installatie vinden");
                }

                Directory.Delete(versionsPath, true);

                progress.Text = "Launcher profiel verwijderen...";
                progress.Update();

                //Remove launcher profile
                string json;
                using (StreamReader sr = new StreamReader(path + "launcher_profiles.json"))
                {
                    json = sr.ReadToEnd();
                    sr.Close();
                }

                JObject config = JObject.Parse(json);
                JObject profiles = config["profiles"] as JObject;

                if (profiles[$"Egel-{selectedVersion}"] != null)
                {
                    profiles.Remove($"Egel-{selectedVersion}");
                }

                // Get new json
                string newJson = config.ToString();

                // Write to file
                File.WriteAllText(path + "launcher_profiles.json", newJson);

                progress.Text = "Jouw mods terugzetten";
                progress.Update();

                // Mods alleen terugzetten als de verwijderde versie de geladen versie was
                if (selectedVersion == loadedVersion) {

                    progress.Text = "Mods verwijderen...";
                    progress.Update();

                    string[] deletableMods = Directory.GetFiles(modsPath, "*.jar", SearchOption.TopDirectoryOnly);
                    foreach (string modPath in deletableMods)
                    {
                        File.Delete(modPath);
                    }

                    if (Directory.Exists(modsPathUser))
                    {
                        if (Directory.EnumerateFileSystemEntries(modsPathUser).Any())
                        {
                            string[] oldMods = Directory.GetFiles(modsPathUser, "*.jar", SearchOption.TopDirectoryOnly);
                            foreach (string modPath in oldMods)
                            {
                                File.Copy(modPath, modPath.Replace(modsPathUser, modsPath));
                                File.Delete(modPath);
                            }
                        }

                        // Alleen deleten als dit je laatste Egel Mod was
                        string[] installedVersions = GetInstalledVersions();
                        if (installedVersions.Length == 1)
                        {
                            Directory.Delete(modsPathUser);
                        }
                    }
                }

                if (Directory.Exists(modsPath + selectedVersion)) {
                    progress.Text = "Mods subfolder verwijderen";
                    progress.Update();
                    Directory.Delete(modsPath + selectedVersion, true);
                }

                // Als de verwijderde versie de geladen versie was
                if (selectedVersion == loadedVersion)
                {
                    ChangeLoadedVersion("");
                }

                progress.Text = "Succesvol verwijderd!";
                progress.Update();

                EnableButtons();
            }
            catch (Exception ex)
            {
                EnableButtons();

                error.Text = ex.Message;
                error.Update();
                progress.Text = "Klik op installeren om te beginnen";
                progress.Update();
            }
        }

        private void select_Click(object sender, EventArgs e)
        {
            try
            {
                progress.Focus();
                DisableButtons();

                error.Text = "";
                error.Update();

                ChangeLoadedVersion(selectedVersion);

                EnableButtons();

                if (loadedVersion == "user_mods") DisableInstall();
            }
            catch (Exception ex)
            {
                EnableButtons();

                if (loadedVersion == "user_mods") DisableInstall();

                error.Text = ex.Message;
                error.Update();
                progress.Text = "Klik op installeren om te beginnen";
                progress.Update();
            }
        }
        #endregion

        #region Helper Functions
        private void versionSelect_SelectedValueChanged(object sender, EventArgs e)
        {
            progress.Focus();
            selectedVersion = versionSelect.SelectedItem.ToString().Replace(" ✔", "");
            ChangeSelectedVersion(versions, selectedVersion);
        }

        void ChangeSelectedVersion(dynamic versions, string version)
        {
            string newVersion = version.Replace(" ✔", "");

            if (String.IsNullOrEmpty(newVersion))
            {
                newVersion = versionSelect.Items[0].ToString();
            }

            // Put it in the combobox as the default value
            versionSelect.SelectedIndex = versionSelect.FindString(version);

            // Get all mod URL's (latest version's by default)
            if (newVersion == "Jouw mods")
            {
                downloadUrls = new string[0];
                linkCount = 0;
                versionsPath = "";
                fabricClientJar = "";
                fabricClientJson = "";


                DisableInstall();

                return;
            }
            EnableInstall();

            downloadUrls = (versions[newVersion].mods).ToObject<string[]>();

            linkCount = downloadUrls.Length;

            // Get all dynamic paths and URL's
            versionsPath = $"{appData}/.minecraft/versions/Egel-{newVersion}/";
            fabricClientJar = $"https://egelbank.nl/EgelMods/{newVersion}/Egel-{newVersion}.jar";
            fabricClientJson = $"https://egelbank.nl/EgelMods/{newVersion}/Egel-{newVersion}.json";
        }

        void ChangeLoadedVersion(string version)
        {
            string newVersion = version.Replace(" ✔", "");
            string oldLoadedVersion = File.ReadAllText(egelPath + "loadedVersion.json");

            newVersion = newVersion == "Jouw mods" ? "user_mods" : newVersion;

            if (loadedVersion == newVersion) throw new Exception("Deze versie is al geladen");
            
            // Check if you have the new version installed
            if (!Directory.Exists(modsPath + newVersion) && newVersion != "user_mods") throw new Exception($"Deze versie is nog niet geïnstalleerd");

            for (int i = 0; i < versionSelect.Items.Count; i++)
            {
                versionSelect.Items[i] = versionSelect.Items[i].ToString().Replace(" ✔", "");
            }

            // Dit gebeurt niet wanneer de loaded version wordt gedeïnstalleerd
            if (!String.IsNullOrEmpty(newVersion) && newVersion != "user_mods")
                versionSelect.Items[versionSelect.FindStringExact(newVersion)] += " ✔";
            else versionSelect.Items[versionSelect.FindStringExact("Jouw mods")] += " ✔";

            if (newVersion != "user_mods")
                File.WriteAllText(egelPath + "loadedVersion.json", newVersion);
            else File.WriteAllText(egelPath + "loadedVersion.json", "");

            // Return als er geen niewe versie is (dit gebeurt wanneer de loaded version wordt gedeïnstalleerd)
            if (String.IsNullOrEmpty(newVersion)) {
                loadedVersion = "";
                return;
            }

            if (String.IsNullOrEmpty(oldLoadedVersion))
            {
                progress.Text = $"Aan het overstappen op {newVersion}...";
                progress.Update();
            }

            string oldModsPath = modsPath + oldLoadedVersion + "/";
            string newModsPath = modsPath + newVersion + "/";

            // If there wasn't anything loaded, move the files to user_mods
            if (String.IsNullOrEmpty(oldLoadedVersion))
            {
                oldModsPath = modsPathUser;
            }

            Directory.CreateDirectory(oldModsPath);
            Directory.CreateDirectory(newModsPath);

            string[] movableMods = Directory.GetFiles(modsPath, "*.jar", SearchOption.TopDirectoryOnly);
            foreach (string modPath in movableMods)
            {
                File.Copy(modPath, modPath.Replace(modsPath, oldModsPath));
                File.Delete(modPath);
            }

            movableMods = Directory.GetFiles(newModsPath, "*.jar", SearchOption.TopDirectoryOnly);
            foreach (string modPath in movableMods)
            {
                File.Copy(modPath, modPath.Replace(newModsPath, modsPath));
                File.Delete(modPath);
            }

            loadedVersion = newVersion;

            if (newVersion != "user_mods")
            progress.Text = $"Overgestapt op {newVersion}!";
            else progress.Text = $"Overgestapt op jouw mods!";
            progress.Update();
        }

        string[] GetInstalledVersions()
        {
            string[] modVersions = Directory.GetDirectories(versionsPathRoot, "Egel-*");
            for (int i = 0; i < modVersions.Length; i++)
            {
                modVersions[i] = modVersions[i].Substring(modVersions[i].LastIndexOf('/') + 6);
            }
            return modVersions;
        }
        #endregion

        #region GUI Functions
        void DisableButtons()
        {
            if (install.Enabled)
            {
                install.Click -= install_Click;
                uninstall.Click -= uninstall_Click;
                

                install.Enabled = false;
                uninstall.Enabled = false;
                
                install.Update();
                uninstall.Update();
                
            }
            if (select.Enabled)
            {
                select.Click -= select_Click;
                select.Enabled = false;
                select.Update();
            }
        }

        void EnableButtons()
        {
            if (!install.Enabled)
            {
                install.Click += install_Click;
                uninstall.Click += uninstall_Click;

                install.Enabled = true;
                uninstall.Enabled = true;

                install.Update();
                uninstall.Update();
            }
            if (!select.Enabled)
            {
                select.Click += select_Click;
                select.Enabled = true;
                select.Update();
            }
        }

        void DisableInstall()
        {
            if (install.Enabled)
            {
                install.Click -= install_Click;
                uninstall.Click -= uninstall_Click;

                install.Enabled = false;
                uninstall.Enabled = false;
                install.Update();
                uninstall.Update();
            }
        }

        void EnableInstall()
        {
            if (!install.Enabled)
            {
                install.Click += install_Click;
                uninstall.Click += uninstall_Click;

                install.Enabled = true;
                uninstall.Enabled = true;
                install.Update();
                uninstall.Update();
            }
        }

        private void versionSelect_DrawItem(object sender, DrawItemEventArgs e)
        {
            string[] installedVersions = GetInstalledVersions();

            e.DrawBackground();

            Brush brush;

            if (e.Index < 0) return;

            if (installedVersions.Contains(versionSelect.Items[e.Index].ToString().Replace(" ✔", "")) || versionSelect.Items[e.Index].ToString().Replace(" ✔", "") == "Jouw mods")
            {
                brush = new SolidBrush(Color.Black);
            }
            else
            {
                brush = new SolidBrush(Color.Gray);
            }


            e.Graphics.DrawString(versionSelect.Items[e.Index].ToString(), versionSelect.Font, brush, e.Bounds);
        }

        private void versionSelect_DropDownClosed(object sender, EventArgs e)
        {
            progress.Focus();
        }

        private void versionSelect_DropDown(object sender, EventArgs e)
        {
            progress.Focus();
        }
        #endregion
    }
}
