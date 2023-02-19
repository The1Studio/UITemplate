namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TMPro;
    using UnityEngine;

    public class UITemplateLevelItemWithStarModel : UITemplateLevelItemModel
    {
        public int StarCount;
        public UITemplateLevelItemWithStarModel(UITemplateLevelRecord record, int level, Status levelStatus, int starCount) : base(record, level, levelStatus)
        {
            this.StarCount = starCount;
        }
    }
    public class UITemplateLevelItemWithStarView : UITemplateLevelItemView
    {
        
        public List<GameObject> StarList;
        
        public void InitView(UITemplateLevelItemWithStarModel data, UITemplateLevelData levelData)
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