namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine;

    public interface IDecorationItem
    {
        Vector3 PositionUI { get; }
        string  Category   { get; }
        void    Init(UITemplateDecorCategoryRecord record);
        void    ScaleItem();
        UniTask ChangeItem(string addressItem);
    }
}