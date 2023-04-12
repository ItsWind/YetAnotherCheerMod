﻿using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace YetAnotherCheerMod {
    public class CheerMissionLogic : MissionLogic {
        private float _cheerTimer = 0.0f;
        private float _cheerDelay = 0.0f;
        private float _clearTroopCheersTimer = 0.0f;

        public override void OnMissionTick(float dt) {
            bool doTick = Agent.Main != null && Agent.Main.IsActive() && (Mission.Current.IsFieldBattle || Mission.Current.IsSallyOutBattle || Mission.Current.IsSiegeBattle);
            if (!doTick) return;

            CheckForCheerInput(dt);
            CheckForCheerDelay(dt);
            CheckForTroopCheersToClear(dt);
        }

        private void CheckForCheerInput(float dt) {
            if (_cheerTimer > 0.0f)
                _cheerTimer -= dt;

            if (!Input.IsKeyPressed(MCMConfig.GetCheerKey())) return;

            if (_cheerTimer <= 0.0f) {
                Utils.PrintToMessages("You yell out to inspire nearby troops..", 255, 255, 102);
                Agent.Main.HandleCheer(SubModule.Random.Next(0, 3));

                if (GlobalSettings<MCMConfig>.Instance.HPGainAffectsPlayer)
                    HealAllyAgent(Agent.Main);

                _cheerDelay = 2.0f;
                _cheerTimer = GlobalSettings<MCMConfig>.Instance.CheerCooldownInSeconds;
            }
            else {
                Utils.PrintToMessages("You must still gather enough strength to inspire again...", 153, 32, 32);
            }
        }

        private void CheckForCheerDelay(float dt) {
            //Utils.PrintToMessages(_cheerDelay.ToString());
            if (_cheerDelay == 0.0f) return;

            if (_cheerDelay > 0.0f) {
                _cheerDelay -= dt;
            } else {
                _cheerDelay = 0.0f;

                int agentsInspired = 0;
                List<Agent> agentsTerrified = new();
                // Get each agent in mission
                foreach (Agent a in Mission.Current.Agents) {
                    // If the agent is null, not human, dead, or too far, etc. then stop
                    if (a == null || !a.IsHuman || a == Agent.Main || !a.IsActive() ||
                        a.GetTrackDistanceToMainAgent() > GlobalSettings<MCMConfig>.Instance.MaxDistanceToCheer) continue;

                    // If the agent is an enemy
                    if (a.Team == Mission.Current.PlayerEnemyTeam) {
                        // Store them for cower modifer later
                        agentsTerrified.Add(a);
                    }
                    else {
                        // Inspire the agent if not an enemy
                        agentsInspired++;
                        InspireAllyAgent(a);
                    }
                }

                // AND THE BIRDS SANG
                Utils.PrintToMessages("And " + agentsInspired.ToString() + " " + Utils.Pluralize("warrior", agentsInspired) + " cried back with vigor!",
                    255, 51, 51);

                _clearTroopCheersTimer = GlobalSettings<MCMConfig>.Instance.TroopCheerTimeInSeconds;

                // If no one was terrified then stop
                if (agentsTerrified.Count <= 0) return;

                // Calculate enemy morale loss modifier
                float cowerModifer = (float)agentsInspired / (float)agentsTerrified.Count;

                // Terrify enemy agents
                TerrifyEnemyAgents(agentsTerrified, cowerModifer);
            }
        }

        private void CheckForTroopCheersToClear(float dt) {
            if (_clearTroopCheersTimer == 0.0f) return;

            if (_clearTroopCheersTimer > 0.0f) {
                _clearTroopCheersTimer -= dt;
            } else {
                _clearTroopCheersTimer = 0.0f;

                foreach (Agent a in Mission.Current.Agents)
                    a.CancelCheering();
            }
        }

        private void HealAllyAgent(Agent agent) {
            // Update agent HP
            int hpChange = SubModule.Random.Next(GlobalSettings<MCMConfig>.Instance.MinTroopHPGain, GlobalSettings<MCMConfig>.Instance.MaxTroopHPGain);
            Utils.UpdateAgentHP(agent, (float)hpChange);
        }

        private void InspireAllyAgent(Agent agent) {
            HealAllyAgent(agent);

            // Update agent morale
            int moraleChange = SubModule.Random.Next(GlobalSettings<MCMConfig>.Instance.MinMoraleGain, GlobalSettings<MCMConfig>.Instance.MaxMoraleGain);
            agent.SetMorale((float)(agent.GetMorale() + moraleChange));

            // Morale cheer threshold check
            if (moraleChange > GlobalSettings<MCMConfig>.Instance.MoraleCheerThreshold)
                agent.HandleCheer(SubModule.Random.Next(0, 3));
        }

        private void TerrifyEnemyAgents(List<Agent> agents, float cowerModifier) {
            foreach (Agent agent in agents) {
                // Update agent morale
                int moraleChange = SubModule.Random.Next(GlobalSettings<MCMConfig>.Instance.MinEnemyMoraleLoss, GlobalSettings<MCMConfig>.Instance.MaxEnemyMoraleLoss);
                float appliedChange = (float)moraleChange * cowerModifier;

                agent.SetMorale(agent.GetMorale() - appliedChange);
            }
        }
    }
}
