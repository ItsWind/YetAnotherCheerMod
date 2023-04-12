using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace YetAnotherCheerMod
{
    internal class Utils
    {
        public static void UpdateAgentHP(Agent a, float mod)
        {
            float newHealth = a.Health + mod;
            a.Health = newHealth <= a.HealthLimit ? newHealth : a.HealthLimit;
        }

        public static string Pluralize(string str, int numOfThings)
        {
            if (numOfThings == 1)
                return str;
            return str + "s";
        }

        public static void PrintToMessages(string str, int r=255, int g=255, int b=255)
        {
            float[] newValues = { (float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f };
            Color col = new(newValues[0], newValues[1], newValues[2]);
            InformationManager.DisplayMessage(new InformationMessage(str, col));
        }
    }
}
