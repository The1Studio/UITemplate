namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TMPro;
    using UnityEngine;

    public class UITemplateLevelItemWithStarView : UITemplateLevelItemView
    {
        
        public List<GameObject> StarList;
        
        public override void InitView(LevelData data, UITemplateUserLevelData userLevelData)
        {
            base.InitView(data, userLevelData);
            data.StarCount = data.LevelStatus != Models.LevelData.Status.Passed ? 0 : data.StarCount;
            for (int i = 0; i < this.StarList.Count; i++)
            {
                this.StarList[i].SetActive(i < data.StarCount);
            }
            

            if (data.StarCount == 0)
                this.LevelText.alignment = TextAlignmentOptions.Center;
            else
                this.LevelText.alignment = TextAlignmentOptions.Top;
        }
        
    }
}