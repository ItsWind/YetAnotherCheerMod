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
        // Module path in explorer
        private static readonly string modPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 26) + "Modules\\YetAnotherCheerMod";

        // Config object
        private static Config config = new(modPath);

        // Random number object
        private Random rand = new();

        // Cheer delay from pressing key
        private readonly float troopCheerDelay = 2.0f;

        // Current cheer cooldown timer
        private float currentCooldown = 0.0f;

        // Configgable values
        private int maxTroopDistanceToCheer;
        private int minTroopMoraleGain;
        private int maxTroopMoraleGain;
        private int minEnemyTroopMoraleLoss;
        private int maxEnemyTroopMoraleLoss;
        private int troopMoraleCheerThreshold;
        private int minTroopHPChange;
        private int maxTroopHPChange;
        private int cooldownTimer;

        // Other values no touchy
        private float cheerDelayPassed = 0.0f;

        protected override void OnSubModuleLoad()
        {
            // Set config values
            this.maxTroopDistanceToCheer = config.GetConfigValues()["troopMaxDistanceToCheer"];

            this.minTroopMoraleGain = config.GetConfigValues()["minTroopMoraleGain"];
            this.maxTroopMoraleGain = config.GetConfigValues()["maxTroopMoraleGain"];
            this.troopMoraleCheerThreshold = config.GetConfigValues()["troopMoraleGainCheerThreshold"];

            this.minEnemyTroopMoraleLoss = config.GetConfigValues()["minEnemyTroopMoraleLoss"];
            this.maxEnemyTroopMoraleLoss = config.GetConfigValues()["maxEnemyTroopMoraleLoss"];

            this.minTroopHPChange = config.GetConfigValues()["minTroopHPChange"];
            this.maxTroopHPChange = config.GetConfigValues()["maxTroopHPChange"];

            this.cooldownTimer = config.GetConfigValues()["cooldownTimer"];

            if (this.minTroopMoraleGain > this.maxTroopMoraleGain)
            {
                this.minTroopMoraleGain = this.maxTroopMoraleGain;
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            bool isInBattle = Game.Current != null && Mission.Current != null && Mission.Current.Scene != null && Agent.Main != null && Agent.Main.IsActive();

            doTroopsCheerTick(dt, isInBattle);
            doButtonPressForCheerTick(isInBattle);

            // Cooldown timer
            if (this.currentCooldown != 0.0f)
            {
                this.currentCooldown += dt;
                if (this.currentCooldown >= this.cooldownTimer)
                {
                    this.currentCooldown = 0.0f;
                    if (isInBattle && this.cooldownTimer > 0) { Utils.PrintToMessages("You feel as if you could once again yell out to inspire your troops!", 255, 255, 51); }
                }
            }
        }

        private void doButtonPressForCheerTick(bool isInBattle)
        {
            if (Input.IsKeyPressed(InputKey.V) && this.cheerDelayPassed == 0.0f)
            {
                if (isInBattle)
                {
                    if (this.currentCooldown == 0.0f)
                    {
                        Utils.PrintToMessages("You yell out to inspire nearby troops..", 255, 255, 102);
                        this.cheerDelayPassed = 0.01f;
                        Agent.Main.HandleCheer(rand.Next(0, 3));
                    }
                    else
                    {
                        Utils.PrintToMessages("You must still gather enough strength to inspire again...", 153, 32, 32);
                    }
                }
            }
        }

        private void doEnemyTroopsCower(List<Agent> affectedAgents, float mod)
        {
            foreach (Agent a in affectedAgents)
            {
                int moraleChange = rand.Next(this.minEnemyTroopMoraleLoss, this.maxEnemyTroopMoraleLoss);
                float newMorale = (a.GetMorale() - ((float)moraleChange*mod));
                a.SetMorale(newMorale);
            }
        }

        private void doTroopsCheerTick(float dt, bool isInBattle)
        {
            // If troops are set to cheer
            if (this.cheerDelayPassed != 0.0f)
            {
                // Update delay
                this.cheerDelayPassed = this.cheerDelayPassed + dt;
                // If delay time passed
                if (this.cheerDelayPassed >= this.troopCheerDelay)
                {
                    // .. and is in battle
                    if (isInBattle)
                    {
                        int cheerBackCount = 0;
                        List<Agent> enemyAgentsAffected = new();
                        // Get each agent in mission
                        foreach (Agent a in Mission.Current.Agents)
                        {
                            // If they are human and close enough to the player
                            if (a != null && a.IsHuman && !a.Equals(Agent.Main) && a.IsActive() &&
                                a.GetTrackDistanceToMainAgent() <= (float)this.maxTroopDistanceToCheer)
                            {
                                // If they are on players team
                                if (a.Team.Equals(Mission.Current.PlayerTeam) || a.Team.Equals(Mission.Current.PlayerAllyTeam))
                                {
                                    // Update agent HP
                                    int hpChange = rand.Next(this.minTroopHPChange, this.maxTroopHPChange);
                                    Utils.UpdateAgentHP(a, (float)hpChange);
                                    // Update agent morale
                                    int moraleChange = rand.Next(this.minTroopMoraleGain, this.maxTroopMoraleGain);
                                    a.SetMorale((float)(a.GetMorale() + moraleChange));
                                    // Morale cheer threshold check
                                    if (moraleChange > this.troopMoraleCheerThreshold)
                                    {
                                        cheerBackCount++;
                                        // CHEEr!1!!
                                        a.HandleCheer(rand.Next(0, 3));
                                    }
                                }
                                // If they are on the enemy team
                                else if (a.Team.Equals(Mission.Current.PlayerEnemyTeam))
                                {
                                    // Add to affected agents list
                                    enemyAgentsAffected.Add(a);
                                }
                            }
                        }
                        Utils.PrintToMessages("And " + cheerBackCount.ToString() + " " + Utils.Pluralize("warrior", cheerBackCount) + " cried back with vigor!",
                            255, 51, 51);
                        // Send affected enemy agents to cower
                        float modForCower = 0.01f;
                        if (cheerBackCount > 0 && enemyAgentsAffected.Count > 0) { modForCower = (float)cheerBackCount / (float)enemyAgentsAffected.Count; }
                        doEnemyTroopsCower(enemyAgentsAffected, modForCower);
                    }
                    // Reset vars
                    this.currentCooldown = 0.01f;
                    this.cheerDelayPassed = 0.0f;
                }
            }
        }
    }
}