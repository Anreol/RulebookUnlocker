using System;

//IMPORTANT TODO:
//Fix ANY RULES with more than 2 options into a new category which is a strip vote
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

        [RoR2.SystemInitializer(new Type[]
{
            typeof(RoR2.RuleCatalog)
})]
        public static void Initialize()
        {
            RoR2.PreGameController.onPreGameControllerSetRuleBookGlobal += RuleBookGlobal;
            RoR2.PreGameController.onPreGameControllerSetRuleBookServerGlobal += ServerRuleBook;
            RoR2.PreGameController.onServerRecalculatedModifierAvailability += onServerRecalculatedAvailabiltiy;
            RoR2.PreGameRuleVoteController.onVotesUpdated += OnVotesUpdated;
        }

        private static bool allowRuleChanging(RoR2.PreGameController preGameController = null)
        {
            if (preGameController == null)
            {
                if (RoR2.PreGameController.instance == null)
                {
                    return false;
                }
                preGameController = RoR2.PreGameController.instance;
            }
            return (RoR2.GameModeCatalog.FindGameModeIndex("ClassicRun") == preGameController.gameModeIndex) || (RoR2.GameModeCatalog.FindGameModeIndex("InfiniteTowerRun") == preGameController.gameModeIndex);
        }

        private static void onServerRecalculatedAvailabiltiy(RoR2.PreGameController obj)
        {
            FixRules();
        }

        private static void OnVotesUpdated()
        {
            if (allowRuleChanging())
            {
                FixRules();
            }
        }

        private static void ServerRuleBook(RoR2.PreGameController arg1, RoR2.RuleBook arg2)
        {
            if (allowRuleChanging(arg1))
            {
                arg1.networkRuleBookComponent.onRuleBookUpdated += NetworkRuleBookComponent_onRuleBookUpdated;
                ExposeRulebookCategoryDefs(arg1.networkRuleBookComponent.ruleBook);
                FixRules();
            }
            else
            {
                UndoRulebookCategoryDefExposure(arg1.networkRuleBookComponent.ruleBook);
            }
        }

        private static void NetworkRuleBookComponent_onRuleBookUpdated(RoR2.NetworkRuleBook obj)
        {
            FixRules();
        }

        private static void RuleBookGlobal(RoR2.PreGameController arg1, RoR2.RuleBook arg2)
        {
            if (allowRuleChanging(arg1))
            {
                ExposeRulebookCategoryDefs(arg1.networkRuleBookComponent.ruleBook);
                FixRules();
            }
            else
            {
                UndoRulebookCategoryDefExposure(arg1.networkRuleBookComponent.ruleBook);
            }
        }

        private static void ExposeRulebookCategoryDefs(RoR2.RuleBook rulebook)
        {
            foreach (var ruleChoiceDef in rulebook.choices)
            {
                //Debug.LogWarning("ruleChoiceDef: " + ruleChoiceDef + " in " + rulebook.choices + " from " + rulebook);
                RoR2.RuleCategoryDef currentCategory = ruleChoiceDef.ruleDef.category;
                if (currentCategory.displayToken == "RULE_HEADER_ITEMS")
                {
                    //Debug.LogWarning("changed RULE_HEADER_ITEMS");
                    currentCategory.hiddenTest = new Func<bool>(ConfigItemCatalog);
                }
                if (currentCategory.displayToken == "RULE_HEADER_EQUIPMENT")
                {
                    //Debug.LogWarning("changed RULE_HEADER_EQUIPMENT");
                    currentCategory.hiddenTest = new Func<bool>(ConfigEquipmentCatalog);
                }
                if (currentCategory.displayToken == "RULE_HEADER_MISC")
                {
                    //Debug.LogWarning("changing RULE_HEADER_MISC");
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

        private static void UndoRulebookCategoryDefExposure(RoR2.RuleBook rulebook)
        {
            foreach (var ruleChoice in rulebook.choices)
            {
                RoR2.RuleCategoryDef currentCategory = ruleChoice.ruleDef.category;
                if (currentCategory.displayToken == "RULE_HEADER_ITEMS")
                {
                    currentCategory.hiddenTest = new Func<bool>(RoR2.RuleCatalog.HiddenTestItemsConvar);
                }
                if (currentCategory.displayToken == "RULE_HEADER_EQUIPMENT")
                {
                    currentCategory.hiddenTest = new Func<bool>(RoR2.RuleCatalog.HiddenTestItemsConvar);
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
                                        foreach (var ruleChoiceDef in ruleDef.choices)
                                        {
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
        }

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
