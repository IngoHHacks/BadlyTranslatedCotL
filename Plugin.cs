using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using System.Collections.Generic;
using System.IO;

namespace BadlyTranslatedCotL
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "IngoH.cotl.BadlyTranslatedCotL";
        public const string PluginName = "BadlyTranslatedCotL";
        public const string PluginVer = "1.0.0";

        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;
        
        internal static Dictionary<string, string> localization = new();

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            Plugin.Log = base.Logger;

            PluginPath = Path.GetDirectoryName(Info.Location);
            
            string localizationPath = Path.Combine(Plugin.PluginPath, "Assets", "BadlyTranslated.language");
            
            // Load localization
            if (File.Exists(localizationPath))
            {
                string[] lines = File.ReadAllLines(localizationPath);
                // Split lines into key and value. Key is the part before the first comma not inside quotes, value is the rest.
                bool isInsideQuotes = false;
                foreach (string line in lines)
                {
                    string key = "";
                    string value = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '"')
                        {
                            isInsideQuotes = !isInsideQuotes;
                        }
                        else if (line[i] == ',' && !isInsideQuotes)
                        {
                            key = line.Substring(1, i-2);
                            value = line.Substring(i+3, line.Length-i-4);
                            break;
                        }
                    }
                    localization.Add(key, value);
                }
            }
            else
            {
                Logger.LogError("Localization file not found! Please make sure that BadlyTranslated.language is in the Assets folder of the plugin.");
            }
        }

        private void OnEnable()
        {
            Harmony.PatchAll();
            Logger.LogInfo($"Loaded {PluginName}!");
        }

        private void OnDisable()
        {
            Harmony.UnpatchSelf();
            Logger.LogInfo($"Unloaded {PluginName}!");
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TermData), "GetTranslation")]
        public static bool GetTranslation(ref string __result, TermData __instance)
        {
            if (localization.ContainsKey(__instance.Term))
            {
                __result = localization[__instance.Term];
                return false;
            }
            return true;
        }
    }
}