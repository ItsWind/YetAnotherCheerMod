using System;
using System.Collections.Generic;
using System.IO;

namespace YetAnotherCheerMod
{
    internal class Config
    {
        private string configFilePath = string.Empty;
        private readonly string configFileString =
            "Set a COOLDOWN timer in SECONDS for how long you must wait to cheer again\n" +
            "cooldownTimer=10\n\n" +

            "Minimum troop morale GAIN on cheer\n" +
            "minTroopMoraleGain=0\n\n" +

            "Maximum troop morale GAIN on cheer\n" +
            "maxTroopMoraleGain=20\n\n" +

            "Minimum ENEMY troop morale LOSS on cheer. This is also affected by the number of your troops affected divided by number of enemy troops affected.\n" +
            "minEnemyTroopMoraleLoss=0\n\n" +

            "Maximum ENEMY troop morale LOSS on cheer. This is also affected by the number of your troops affected divided by number of enemy troops affected.\n" +
            "maxEnemyTroopMoraleLoss=20\n\n" +

            "The amount of morale a troop needs to gain to cheer back, set to -1 to always make them cheer back\n" +
            "troopMoraleGainCheerThreshold=5\n\n" +

            "The amount of distance a troop needs to be within the player to be affected by cheer. What is this measured in btw who knows???? is it meters????\n" +
            "troopMaxDistanceToCheer=50\n\n" +

            "Minimum troop HP gain on cheer\n" +
            "minTroopHPChange=15\n\n" +

            "Maximum troop HP gain on cheer\n" +
            "maxTroopHPChange=30\n\n";
        private Dictionary<string, int> configValues = new();

        private void CreateConfigFile(string filePath)
        {
            StreamWriter sw = new(filePath);
            sw.WriteLine(this.configFileString);
            sw.Close();
        }

        public Config(string modPath)
        {
            string configFilePath = modPath + "\\config.txt";

            if (!File.Exists(configFilePath))
            {
                CreateConfigFile(configFilePath);
            }

            StreamReader sr = new(configFilePath);
            string line;
            // Read and display lines from the file until the end of
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                int indexOfEqualSign = line.IndexOf('=');
                if (indexOfEqualSign != -1)
                {
                    string key = line.Substring(0, indexOfEqualSign);
                    string value = line.Substring(indexOfEqualSign + 1);
                    configValues.Add(key, Convert.ToInt32(value));
                }
            }
            sr.Close();
        }
        public Dictionary<string, int> GetConfigValues()
        {
            return configValues;
        }
    }
}
