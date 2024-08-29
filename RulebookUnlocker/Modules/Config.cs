
using BepInEx.Configuration;

namespace RuleBookEditor
{
    internal static class Config
    {
        internal static ConfigEntry<bool> unlockItemCatalog;
        internal static ConfigEntry<bool> unlockEquipmentCatalog;
        internal static ConfigEntry<bool> unlockMiscMoneyRule;
        internal static ConfigEntry<bool> unlockMiscStageOrder;
        internal static ConfigEntry<bool> unlockMiscKeepMoneyBetweenStages;
        internal static ConfigEntry<bool> unlockMiscAllowDropin;

        internal static void Initialize()
        {
            unlockItemCatalog =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Item Catalog",
                "Enabled",
                true,
                "Forces the game to show up the item catalog while in a Lobby for enabling / disabling items.");
            unlockEquipmentCatalog =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Equipment Catalog",
                "Enabled",
                 true,
                "Forces the game to show up the equipment catalog while in a Lobby for enabling / disabling equipment.");
            unlockMiscMoneyRule =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Misc Money Rules",
                "Enabled",
                true,
                "Forces the game to show up the misc rule of starting money.");
            unlockMiscStageOrder =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Misc Stage Order Rules",
                "Enabled",
                true,
                "Forces the game to show up the misc rule of the run's stage order.");
            unlockMiscKeepMoneyBetweenStages =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Misc Money between stages rule",
                "Enabled",
                 true,
                "Forces the game to show up the misc rule of keeping money between stages.");
            unlockMiscAllowDropin =
                RuleBookUnlocker.instance.Config.Bind("RuleBookUnlocker :: Pre-Run Misc Allow Dropin",
                "Enabled",
                true,
                "Forces the game to show up the misc rule of allowing new players to join the game. If enabled, will only show up if current run does not allow new participants.");
        }
    }
}