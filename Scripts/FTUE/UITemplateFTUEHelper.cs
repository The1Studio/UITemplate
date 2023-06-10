namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateFTUEHelper
    {
        private readonly ScreenManager                 screenManager;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;
        private readonly UITemplateFTUEBlueprint       ftueBlueprint;
        private readonly UITemplateFTUEControllerData  uiTemplateFtueControllerData;

        public UITemplateFTUEHelper(ScreenManager screenManager, UITemplateLevelDataController uiTemplateLevelDataController, UITemplateFTUEBlueprint ftueBlueprint,
            UITemplateFTUEControllerData uiTemplateFtueControllerData
        )
        {
            this.screenManager                 = screenManager;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
            this.ftueBlueprint                 = ftueBlueprint;
            this.uiTemplateFtueControllerData  = uiTemplateFtueControllerData;
        }

        public bool IsAnyFtueActive() => this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen);

        public bool IsAnyFtueActive(IScreenPresenter screenPresenter)
        {
            var currentScreen = screenPresenter.GetType().Name;

            foreach (var stepBlueprintRecord in this.ftueBlueprint.Values)
            {
                if (!currentScreen.Equals(stepBlueprintRecord.ScreenLocation)) continue;

                if (!this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(stepBlueprintRecord.RequireTriggerComplete)) continue;
                if (this.uiTemplateFtueControllerData.IsFinishedStep(stepBlueprintRecord.Id)) continue;

                var isPassedCondition = this.IsPassedCondition(stepBlueprintRecord.Id);

                if (!isPassedCondition) continue;

                return true;
            }

            return false;
        }

        public bool IsPassedCondition(string stepId)
        {
            foreach (var requireCondition in this.ftueBlueprint[stepId].GetRequireCondition())
            {
                var isPass = requireCondition.RequireId switch
                {
                    FTUEStaticValue.RequireConditionId.RoundLevelRequire => this.IsPassedRoundLevelRequire(requireCondition.RequireValue, requireCondition.Condition),
                    FTUEStaticValue.RequireConditionId.PlayerLevelRequire => this.IsPassedPlayerLevelRequire(requireCondition.RequireValue, requireCondition.Condition),
                    _ => false
                };

                if (!isPass)
                {
                    return false;
                }
            }

            return true;
        }

        #region Check Pass Condition

        private bool IsPassedRoundLevelRequire(string requireLevel, string condition)
        {
            var level = int.Parse(requireLevel);

            return condition switch
            {
                FTUEStaticValue.FTUECondition.Equal => this.uiTemplateLevelDataController.GetCurrentLevelData.Level == level,
                FTUEStaticValue.FTUECondition.NotEqual => this.uiTemplateLevelDataController.GetCurrentLevelData.Level != level,
                FTUEStaticValue.FTUECondition.Higher => this.uiTemplateLevelDataController.GetCurrentLevelData.Level > level,
                FTUEStaticValue.FTUECondition.Lower => this.uiTemplateLevelDataController.GetCurrentLevelData.Level < level,
                FTUEStaticValue.FTUECondition.HighEqual => this.uiTemplateLevelDataController.GetCurrentLevelData.Level >= level,
                FTUEStaticValue.FTUECondition.LowEqual => this.uiTemplateLevelDataController.GetCurrentLevelData.Level <= level,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }

        private bool IsPassedPlayerLevelRequire(string requireLevel, string condition)
        {
            var level = int.Parse(requireLevel);

            //Todo Fill player Level
            return condition switch
            {
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }

        #endregion
    }
}