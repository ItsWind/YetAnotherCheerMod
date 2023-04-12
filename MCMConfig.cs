using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using TaleWorlds.InputSystem;

namespace YetAnotherCheerMod {
    internal sealed class MCMConfig : AttributeGlobalSettings<MCMConfig> {
		public override string Id => "YetAnotherCheerMod";
		public override string DisplayName => "Yet Another Cheer Mod";
		public override string FolderName => "Yet Another Cheer Mod";
		public override string FormatType => "xml";

		public static InputKey GetCheerKey() {
			InputKey key;
            try {
				string toUse = GlobalSettings<MCMConfig>.Instance.CheerKey;
				toUse = toUse.Length == 1 ? toUse.ToUpper() : toUse;
				key = (InputKey)Enum.Parse(typeof(InputKey), toUse);
			}
			catch (Exception) { return InputKey.V; }
			return key;
        }

		[SettingPropertyText("Cheer Key", HintText = "Key to press to cheer in battle. If this value is not set correctly, it will default to V.", Order = 1, RequireRestart = false)]
		[SettingPropertyGroup("General")]
		public string CheerKey { get; set; } = "V";

		[SettingPropertyFloatingInteger("Cheer Cooldown", 1f, 30f, "0.00", HintText = "Set the amount of seconds needed to wait after cheering to cheer again.", Order = 2, RequireRestart = false)]
		[SettingPropertyGroup("General")]
		public float CheerCooldownInSeconds { get; set; } = 10f;

		[SettingPropertyFloatingInteger("Cheer Threshold", 0f, 100f, "0.00", HintText = "Set the amount of morale gain needed for a troop to cheer with you.", Order = 3, RequireRestart = false)]
		[SettingPropertyGroup("General")]
		public float MoraleCheerThreshold { get; set; } = 5f;

		[SettingPropertyFloatingInteger("Troop Cheer Time", 1f, 5f, "0.00", HintText = "Set the amount of seconds a troop spends cheering", Order = 4, RequireRestart = false)]
		[SettingPropertyGroup("General")]
		public float TroopCheerTimeInSeconds { get; set; } = 2f;

		[SettingPropertyFloatingInteger("Troop Max Distance To Cheer", 0f, 100f, "0.00", HintText = "Set the maximum distance that a troop will recognize your cheer.", Order = 5, RequireRestart = false)]
		[SettingPropertyGroup("General")]
		public float MaxDistanceToCheer { get; set; } = 50f;

		// MORALE CHANGES

		[SettingPropertyInteger("Minimum Morale Gain", 0, 100, HintText = "Set the minimum troop morale gain from cheer.", Order = 1, RequireRestart = false)]
		[SettingPropertyGroup("Morale Changes")]
		public int MinMoraleGain { get; set; } = 0;

		[SettingPropertyInteger("Maximum Morale Gain", 0, 100, HintText = "Set the maximum troop morale gain from cheer. Make sure this is a higher number than the minimum.", Order = 2, RequireRestart = false)]
		[SettingPropertyGroup("Morale Changes")]
		public int MaxMoraleGain { get; set; } = 20;

		[SettingPropertyInteger("Minimum Enemy Morale Loss", 0, 100, HintText = "Set the minimum enemy troop morale loss from cheer.", Order = 3, RequireRestart = false)]
		[SettingPropertyGroup("Morale Changes")]
		public int MinEnemyMoraleLoss { get; set; } = 0;

		[SettingPropertyInteger("Maximum Enemy Morale Loss", 0, 100, HintText = "Set the maximum enemy troop morale loss from cheer. Make sure this is a higher number than the minimum.", Order = 4, RequireRestart = false)]
		[SettingPropertyGroup("Morale Changes")]
		public int MaxEnemyMoraleLoss { get; set; } = 20;

		// HP CHANGES

		[SettingPropertyBool("HP Gain Affects Player", HintText = "Enables player HP gain as soon as you cheer. Is affected by min and max below.", Order = 1, RequireRestart = false)]
		[SettingPropertyGroup("HP Changes")]
		public bool HPGainAffectsPlayer { get; set; } = true;

		[SettingPropertyInteger("Minimum Troop HP Gain", 0, 100, HintText = "Set the minimum hp a troop will gain from cheer.", Order = 2, RequireRestart = false)]
		[SettingPropertyGroup("HP Changes")]
		public int MinTroopHPGain { get; set; } = 10;

		[SettingPropertyInteger("Maximum Troop HP Gain", 0, 100, HintText = "Set the maximum hp a troop will gain from cheer. Make sure this is a higher number than the minimum.", Order = 3, RequireRestart = false)]
		[SettingPropertyGroup("HP Changes")]
		public int MaxTroopHPGain { get; set; } = 30;
	}
}
