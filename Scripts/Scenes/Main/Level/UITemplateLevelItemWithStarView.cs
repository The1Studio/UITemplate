namespace UITemplate.Scripts.Scenes.Main.Level
{
    using UnityEngine;
    using TMPro;
    using System.Collections.Generic;
    using UITemplate.Scripts.Models;

    public class UITemplateLevelItemWithStarView : UITemplateLevelItemView
    {
        
        public List<GameObject> StarList;
        
        public override void InitView(UITemplateLevelItemModel data, UITemplateLevelData levelData)
        {
            base.InitView(data, levelData);
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