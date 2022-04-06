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
            a.Health += mod;
            if (a.Health > a.HealthLimit)
            {
                a.Health = a.HealthLimit;
            }
        }
        public static string Pluralize(string str, int numOfThings)
        {
            if (numOfThings == 1)
            {
                return str;
            }
            return str + "s";
        }
        public static void PrintToMessages(string str, int r, int g, int b)
        {
            float[] newValues = { (float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f };
            Color col = new(newValues[0], newValues[1], newValues[2]);
            InformationManager.DisplayMessage(new InformationMessage(str, col));
        }
        public static void PrintToMessages(string str)
        {
            InformationManager.DisplayMessage(new InformationMessage(str));
        }
    }
}
