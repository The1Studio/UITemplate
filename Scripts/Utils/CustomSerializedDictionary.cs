namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using System;
    using ServiceImplementation.Configs.CustomTypes;
    using TMPro;
    using UnityEngine.UI;

    [Serializable]
    public class StringToTextDictionary : SerializableDictionary<string, TMP_Text>
    {
    }
    
    [Serializable]
    public class StringToButtonDictionary : SerializableDictionary<string, Button>
    {
    }
}