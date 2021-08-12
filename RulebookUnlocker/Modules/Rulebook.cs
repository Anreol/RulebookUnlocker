using System;

namespace RuleBookEditor
{
    internal class Rulebook
    {
        private static bool ConfigItemCatalog()
        {
            return !Config.unlockItemCatalog.Value;
        }

        private static bool ConfigEquipmentCatalog()
        {
            return !Config.unlockEquipmentCatalog.Value;
        }

        private static bool ReturnFalse()
        {
            return false;
        }

        private static bool ReturnTrue()
        {
            return true;
        }

        public static void Initialize()
        {
            RoR2.PreGameController.onPreGameControllerSetRuleBookGlobal += PreGameController_onPreGameControllerSetRuleBookGlobal;
            RoR2.PreGameController.onPreGameControllerSetRuleBookServerGlobal += PreGameController_onPreGameControllerSetRuleBookServerGlobal;
            RoR2.PreGameRuleVoteController.onVotesUpdated += PreGameRuleVoteController_onVotesUpdated;
        }

        private static void PreGameRuleVoteController_onVotesUpdated()
        {
            if (RoR2.GameModeCatalog.FindGameModeIndex("ClassicRun") == RoR2.PreGameController.instance.gameModeIndex)
            {
                FixRules();
            }
        }

        private static void PreGameController_onPreGameControllerSetRuleBookServerGlobal(RoR2.PreGameController arg1, RoR2.RuleBook arg2)
        {
            //RoR2.PreGameController.GameModeConVar.instance.GetString() == "ClassicRun"
            if (RoR2.GameModeCatalog.FindGameModeIndex("ClassicRun") == arg1.gameModeIndex)
            {
                //Me on my way to subscribe the most amount of events so this shit gets fixed
                arg1.networkRuleBookComponent.onRuleBookUpdated += NetworkRuleBookComponent_onRuleBookUpdated;
                ChangeRuleCatalogRuleCategoryDef(arg1.networkRuleBookComponent.ruleBook);
                FixRules();
            }
        }

        private static void NetworkRuleBookComponent_onRuleBookUpdated(RoR2.NetworkRuleBook obj)
        {
            FixRules();
        }

        private static void PreGameController_onPreGameControllerSetRuleBookGlobal(RoR2.PreGameController arg1, RoR2.RuleBook arg2)
        {
            if (RoR2.GameModeCatalog.FindGameModeIndex("ClassicRun") == arg1.gameModeIndex)
            {
                ChangeRuleCatalogRuleCategoryDef(arg1.networkRuleBookComponent.ruleBook);
                FixRules();
            }
        }

        private static void ChangeRuleCatalogRuleCategoryDef(RoR2.RuleBook rulebook)
        {
            foreach (var ruleChoiceDef in rulebook.choices)
            {
                RoR2.RuleCategoryDef currentCategory = ruleChoiceDef.ruleDef.category;
                if (currentCategory.displayToken == "RULE_HEADER_ITEMS")
                {
                    currentCategory.hiddenTest = new Func<bool>(ConfigItemCatalog);
                }
                if (currentCategory.displayToken == "RULE_HEADER_EQUIPMENT")
                {
                    currentCategory.hiddenTest = new Func<bool>(ConfigEquipmentCatalog);
                }
                if (currentCategory.displayToken == "RULE_HEADER_MISC")
                {
                    foreach (var ruleDef in currentCategory.children)
                    {
                        switch (ruleDef.displayToken) //We could just don't care and enable everything, however, lets be specific in case a mod adds a new rule.
                        {
                            case "RULE_MISC_STARTING_MONEY":
                                {
                                    if (Config.unlockMiscMoneyRule.Value)
                                    {
                                        foreach (var subRuleChoiceDef in ruleDef.choices)
                                        {
                                            subRuleChoiceDef.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney"; //Fixes em not having icons
                                            subRuleChoiceDef.excludeByDefault = false;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_STAGE_ORDER":
                                {
                                    if (Config.unlockMiscStageOrder.Value)
                                    {
                                        foreach (var subRuleChoiceDef in ruleDef.choices)
                                        {
                                            subRuleChoiceDef.spritePath = "Textures/MiscIcons/texRuleMapIsRandom"; //See above!
                                            subRuleChoiceDef.excludeByDefault = false;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_KEEP_MONEY_BETWEEN_STAGES":
                                {
                                    if (Config.unlockMiscKeepMoneyBetweenStages.Value)
                                    {
                                        foreach (var subRuleChoiceDef in ruleDef.choices)
                                        {
                                            subRuleChoiceDef.excludeByDefault = false;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_ALLOW_DROP_IN":
                                {
                                    if (Config.unlockMiscAllowDropin.Value)
                                    {
                                        foreach (var subRuleChoiceDef in ruleDef.choices)
                                        {
                                            subRuleChoiceDef.excludeByDefault = false;
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
        }

        //We are changing the catalog directly so we gotta undo it
        /* No Longer needed, we are changing a PreGameController instance's rulebook instead of the catalog...
        private static void UndoRuleCatalogRuleCategoryDef()
        {
            foreach (var categoryDef in RoR2.RuleCatalog.allCategoryDefs)
            {
                if (categoryDef.displayToken == "RULE_HEADER_ITEMS")
                {
                    categoryDef.hiddenTest = new Func<bool>(RoR2.RuleCatalog.HiddenTestItemsConvar);
                }
                if (categoryDef.displayToken == "RULE_HEADER_EQUIPMENT")
                {
                    categoryDef.hiddenTest = new Func<bool>(RoR2.RuleCatalog.HiddenTestItemsConvar);
                }
                if (categoryDef.displayToken == "RULE_HEADER_MISC")
                {
                    foreach (var ruleDef in categoryDef.children)
                    {
                        switch (ruleDef.displayToken) //We could just don't care and enable everything, however, lets be specific in case a mod adds a new rule.
                        {
                            case "RULE_MISC_STARTING_MONEY":
                                {
                                    if (Config.unlockMiscMoneyRule.Value)
                                    {
                                        foreach (var ruleChoiceDef in ruleDef.choices)
                                        {
                                            ruleChoiceDef.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney"; //Fixes em not having icons
                                            ruleChoiceDef.excludeByDefault = true;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_STAGE_ORDER":
                                {
                                    if (Config.unlockMiscStageOrder.Value)
                                    {
                                        foreach (var ruleChoiceDef in ruleDef.choices)
                                        {
                                            ruleChoiceDef.spritePath = "Textures/MiscIcons/texRuleMapIsRandom"; //See above!
                                            ruleChoiceDef.excludeByDefault = true;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_KEEP_MONEY_BETWEEN_STAGES":
                                {
                                    if (Config.unlockMiscKeepMoneyBetweenStages.Value)
                                    {
                                        foreach (var ruleChoiceDef in ruleDef.choices)
                                        {
                                            ruleChoiceDef.excludeByDefault = true;
                                        }
                                    }
                                    break;
                                }
                            case "RULE_MISC_ALLOW_DROP_IN":
                                {
                                    if (Config.unlockMiscAllowDropin.Value)
                                    {
                                        foreach (var ruleChoiceDef in ruleDef.choices)
                                        {
                                            ruleChoiceDef.excludeByDefault = true;
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
        }*/

        private static void FixRules() //Check RuleCategoryController's line 265, I think it's hilarious
        {
            //Will probably break something in Eclipse or other gamemode that has forced rules...
            foreach (var choiceController in RoR2.UI.RuleChoiceController.instancesList)
            {
                //Cannot do choiceController.choiceDef.ruleDef.AvailableChoiceCount as we dont have access to masks..
                if (choiceController.choiceDef.ruleDef.choices.Count > 1)
                {
                    choiceController.canVote = true;
                }
            }
        }
    }
}