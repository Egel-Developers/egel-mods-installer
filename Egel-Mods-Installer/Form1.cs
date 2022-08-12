using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Egel_Mods_Installer
{
    public partial class Form1 : Form
    {
        readonly static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        readonly string path = $"{appData}/.minecraft/";
        readonly string modsPath = $"{appData}/.minecraft/mods/";
        readonly string modsPathOld = $"{appData}/.minecraft/mods/old_mods/";
        readonly string versionsPath = $"{appData}/.minecraft/versions/Egel-1.19.2/";

        static readonly string[] downloadUrls = new string[]
        {
                "https://cdn.modrinth.com/data/P7dR8mSH/versions/0.59.0+1.19.2/fabric-api-0.59.0%2B1.19.2.jar",
                "https://cdn.modrinth.com/data/mOgUt4GM/versions/4.0.6/modmenu-4.0.6.jar",
                "https://cdn.modrinth.com/data/Orvt0mRa/versions/1.0.7+mc1.19/indium-1.0.7%2Bmc1.19.jar",
                "https://cdn.modrinth.com/data/FWumhS4T/versions/1.19-1.7.1/smoothboot-fabric-1.19-1.7.1.jar",
                "https://cdn.modrinth.com/data/ZfQ3kTvR/versions/4.0.0+1.19/dashloader-4.0.0%2B1.19.jar",
                "https://cdn.modrinth.com/data/kCpssoSb/versions/1.1.5.fabric.1.19/Fastload-1.1.5.jar",
                "https://cdn.modrinth.com/data/fQEb0iXm/versions/0.2.1/krypton-0.2.1.jar",
                "https://cdn.modrinth.com/data/AANobbMI/versions/mc1.19-0.4.2/sodium-fabric-mc1.19-0.4.2%2Bbuild.16.jar",
                "https://cdn.modrinth.com/data/gvQqBUqZ/versions/mc1.19.2-0.8.3/lithium-fabric-mc1.19.2-0.8.3.jar",
                "https://cdn.modrinth.com/data/hvFnDODi/versions/0.1.3/lazydfu-0.1.3.jar",
                "https://cdn.modrinth.com/data/OVuFYfre/versions/0.7.1+1.19/enhancedblockentities-0.7.1%2B1.19.jar",
                "https://cdn.modrinth.com/data/NNAgCjsB/versions/1.5.2-fabric-1.19/entityculling-fabric-1.5.2-mc1.19.jar",
                "https://cdn.modrinth.com/data/H8CaAYZC/versions/1.1.1+1.19/starlight-1.1.1%2Bfabric.ae22326.jar",
                "https://cdn.modrinth.com/data/51shyZVL/versions/v0.9.1/moreculling-1.19.1-0.9.1.jar",
                "https://cdn.modrinth.com/data/NRjRiSSD/versions/v0.7.0-1.19/memoryleakfix-1.19-0.7.0.jar",
                "https://cdn.modrinth.com/data/uXXizFIs/versions/5.0.0-fabric/ferritecore-5.0.0-fabric.jar",
                "https://cdn.modrinth.com/data/YL57xq9U/versions/1.19.x-v1.2.6/iris-mc1.19.1-1.2.6.jar",
                "https://cdn.modrinth.com/data/yBW8D80W/versions/2.1.2+1.19/lambdynamiclights-2.1.2%2B1.19.jar",
                "https://cdn.modrinth.com/data/1IjD5062/versions/2.0.1+1.19/continuity-2.0.1%2B1.19.jar",
                "https://cdn.modrinth.com/data/qQyHxfxd/versions/Fabric-1.19.2-v1.9.1/NoChatReports-FABRIC-1.19.2-v1.9.1.jar",
                "https://egelbank.nl/EgelMods/farsight-fabric-1.19.1-2.0.jar",
                "https://cdn.modrinth.com/data/QwxR6Gcd/versions/2.4.1/Debugify-2.4.1.jar",
                "https://cdn.modrinth.com/data/yM94ont6/versions/4.1.6+1.19-fabric/notenoughcrashes-4.1.6%2B1.19-fabric.jar",
                "https://cdn.modrinth.com/data/9eGKb6K1/versions/fabric-1.19.2-2.3.5/voicechat-fabric-1.19.2-2.3.5.jar",
                "https://cdn.modrinth.com/data/8bOImuGU/versions/0.0.17/logical_zoom-0.0.17.jar"
        };

        int linkCount = downloadUrls.Length;

        string fabricClientJar = "https://egelbank.nl/EgelMods/fabric-loader-0.14.9-1.19.2.jar";
        string fabricClientJson = "https://egelbank.nl/EgelMods/fabric-loader-0.14.9-1.19.2.json";

        public Form1()
        {
            InitializeComponent();
        }

        private void install_Click(object sender, EventArgs e)
        {
            if (appData.Length == 0) return;
            
            //Move all existing mods to ../.minecraft/mods/old_mods/
            if (Directory.Exists(modsPath) && !Directory.Exists(modsPathOld)) {
                progress.Text = "Existing mods found";

                Directory.CreateDirectory(modsPathOld);

                progress.Text = "Moving existing mods...";
                string[] oldMods = Directory.GetFiles(modsPath, "*.jar", SearchOption.TopDirectoryOnly);
                foreach (string modPath in oldMods) 
                {
                    File.Copy(modPath, modPath.Replace(modsPath, modsPathOld));
                    File.Delete(modPath);
                }

            } else {
                progress.Text = "No existing mods found";
                Directory.CreateDirectory(modsPath);
            }

            progress.Text = "Preparing to download mods...";
            int i = 1;

            WebClient webClient = new WebClient();
            //Download mods one by one and put them in the mods folder
            foreach (string url in downloadUrls) {
                //Progress + 1
                progress.Text = "Downloading mods: " + Convert.ToString(i) + " of " + Convert.ToString(linkCount);
                i++;

                string fileName = url.Substring(url.LastIndexOf('/') + 1);
                fileName = fileName.Replace("%2B", "-");

                webClient.DownloadFile(url, modsPath + fileName);
            }

            progress.Text = "Creating custom minecraft install";
            //Prepare custom MC version (Egel-1.19.2) in ../.minecraft/versions/
            Directory.CreateDirectory(versionsPath);
            webClient.DownloadFile(fabricClientJar, versionsPath + "fabric-loader-0.14.9-1.19.2.jar");
            webClient.DownloadFile(fabricClientJson, versionsPath + "fabric-loader-0.14.9-1.19.2.json");

            progress.Text = "Adding launcher profile";
            // Edit the launcher_profiles.json file
            using (StreamReader sr = new StreamReader(path + "launcher_profiles.json"))
            {
                string json = sr.ReadToEnd();
                JObject config = JObject.Parse(json);

                JObject profiles = config["profiles"] as JObject;

                dynamic newProfile = new JObject();
                newProfile.lastUsed = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                newProfile.lastVersionId = "Egel-1.19.2";
                newProfile.created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                newProfile.icon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIAAAACABAMAAAAxEHz4AAAAGFBMVEUAAAA4NCrb0LTGvKW8spyAem2uppSakn5SsnMLAAAAAXRSTlMAQObYZgAAAJ5JREFUaIHt1MENgCAMRmFWYAVXcAVXcAVXcH3bhCYNkYjcKO8dSf7v1JASUWdZAlgb0PEmDSMAYYBdGkYApgf8ER3SbwRgesAf0BACMD1gB6S9IbkEEBfwY49oNj4lgLhA64C0o9R9RABTAvp4SX5kB2TA5y8EEAK4pRrxB9QcA4QBWkj3GCAMUCO/xwBhAI/kEsCagCHDY4AwAC3VA6t4zTAMj0OJAAAAAElFTkSuQmCC";
                newProfile.name = "Egel Mods";
                newProfile.type = "custom";

                profiles.Add("EgelMods", newProfile);

                // Get new json
                string newJson = config.ToString();

                // Write to file
                File.WriteAllText(path + "launcher_profiles.json", newJson);
            }
            progress.Text = "Finished installing!";
        }
    }
}
