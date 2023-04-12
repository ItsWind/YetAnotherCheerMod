using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace YetAnotherCheerMod.AddonHelpers {
    public static class CheerMissionEvents {
        // PLAYER CHEER EVENT
        private static List<Action<Agent>> _onPlayerCheerActions = new();
        internal static void Fire_OnPlayerCheer(Agent playerAgent) {
            foreach (Action<Agent> action in _onPlayerCheerActions)
                action(playerAgent);
        }
        /// <summary>
        /// Adds a method/action to be run when the player cheers.
        /// </summary>
        /// <param name="action">Action takes one parameter. Agent is for the main agent of the player.</param>
        public static void AddAction_OnPlayerCheer(Action<Agent> action) {
            _onPlayerCheerActions.Add(action);
        }
    }
}
