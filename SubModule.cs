using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace YetAnotherCheerMod
{
    public class SubModule : MBSubModuleBase
    {
        // Rand object
        public static Random Random = new();

        public override void OnMissionBehaviorInitialize(Mission mission) {
            mission.AddMissionBehavior(new CheerMissionLogic());
        }
    }
}