namespace TheOne.Tool.Optimization.Addressable
{
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;

    public class AddressableGroupOdin : OdinEditorWindow
    {
        [ShowInInspector] [TableList] [Title("Addressable Group", TitleAlignment = TitleAlignments.Centered)]
        private List<AddressableGroupInfo> groups = new();




        [MenuItem("TheOne/List And Optimize/Addressable Group")]
        private static void OpenWindow() { GetWindow<AddressableGroupOdin>().Show(); }

        [ButtonGroup("List all groups")]
        [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        public void GetAllGroup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var remoteBuildProfileData = settings.profileSettings.GetProfileDataByName(AddressableAssetSettings.kRemoteBuildPath);

            this.groups = settings.groups.Select(group =>
            {
                var bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
                if (bundledAssetGroupSchema == null) return null;
                return new AddressableGroupInfo
                {
                    Group      = group,
                    Schema     = bundledAssetGroupSchema,
                    IsRemotely = bundledAssetGroupSchema.BuildPath.Id.Equals(remoteBuildProfileData.Id)
                };
            }).Where(groupInfo => groupInfo is not null).ToList();

            // addressableAssetGroup.HasSchema<>()
        }

        private void SetToLocalGroup(BundledAssetGroupSchema schema) { }
    }

    public class AddressableGroupInfo
    {
        public AddressableAssetGroup   Group;
        public BundledAssetGroupSchema Schema;
        [OnValueChanged("OnChangeLoadType")]
        public bool                    IsRemotely;
        
        private void OnChangeLoadType()
        {
            this.Schema.BuildPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings, this.IsRemotely ? AddressableAssetSettings.kRemoteBuildPath : AddressableAssetSettings.kLocalBuildPath);
            this.Schema.LoadPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings,
                this.IsRemotely ? AddressableAssetSettings.kRemoteLoadPath : AddressableAssetSettings.kLocalLoadPath);
        }
    }
}