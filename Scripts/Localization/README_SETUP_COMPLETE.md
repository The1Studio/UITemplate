# Blueprint Localization - Simple & Ready! ✅

Hệ thống localization đơn giản cho Unity blueprints với Unity String Tables.

## 🚀 Setup Unity Localization

1. **Install Package**: Window > Package Manager > Localization
2. **Create Settings**: Window > Asset Management > Localization Tables > Create > Localization Settings  
3. **Add Locales**: English (en), Tiếng Việt (vi), Spanish (es)
4. **Create String Table**: New Table Collection
5. **Add Keys**: `item_001_Content`, `quest_001_Name`, etc.

## 📝 Blueprints Ready

Các blueprint đã có `[LocalizableField]`:
- ✅ `SpecialItemBlueprint.Content`
- ✅ `UITemplateCategoryItemBlueprint.Title`  
- ✅ `UITemplateQuestBlueprint.Name` & `Description`

## 🎮 Usage

```csharp
[Inject] private LocalizationManager localizationManager;

// Change language - blueprint fields auto-update
localizationManager.SetLanguage("vi");

// Access localized content
var item = specialItemBlueprint.GetDataById("item_001");
Debug.Log(item.Content); // Vietnamese text
```

## 🔧 System Architecture

**Simple & Clean**:
- `UnityStringTableLocalizationProvider` - Loads from Unity String Tables
- `LocalizationManager` - Main API for language changes
- `BlueprintLocalizationService` - Auto-updates blueprint fields
- No interfaces, no complexity!

**That's it! Super simple blueprint localization ready! 🎉**
