# Blueprint Localization System

Hệ thống localization tự động cho Blueprint fields trong Unity game. Khi thay đổi ngôn ngữ, tất cả các field được đánh dấu `[LocalizableField]` sẽ được tự động cập nhật với giá trị đã localize.

## Features

- ✅ Tự động localize blueprint fields khi thay đổi ngôn ngữ
- ✅ Hỗ trợ cả `GenericBlueprintReaderByRow` và `GenericBlueprintReaderByCol`
- ✅ Signal-based architecture cho việc thông báo thay đổi
- ✅ Fallback values khi không tìm thấy localization
- ✅ DI integration với VContainer
- ✅ Performance caching cho reflection operations

## Setup

### 1. Register Services trong VContainer

```csharp
// Trong installer/bootstrap của bạn
public void Install(IContainerBuilder builder)
{
    // Register blueprint system
    builder.RegisterBlueprints();
    
    // Register localization system
    builder.RegisterLocalization();
}
```

### 2. Mark Blueprint Fields

```csharp
public class SpecialItemRecord
{
    public string Id { get; set; }
    public string Thumbnail { get; set; }
    public string Icon { get; set; }
    
    // Đánh dấu field này để tự động localize
    [LocalizableField]
    public string Content { get; set; }
    
    // Hoặc với custom localization key
    [LocalizableField("special_item_description")]
    public string Description { get; set; }
}
```

### 3. Chuẩn bị Localization Data

Tạo file JSON trong `Resources/Localization/`:

**Resources/Localization/en.json:**
```json
{
    "special_item_content_1": "This is a special item",
    "special_item_description": "A powerful artifact",
    "shop_cat_item_hat": "Fancy Hat"
}
```

**Resources/Localization/vi.json:**
```json
{
    "special_item_content_1": "Đây là một vật phẩm đặc biệt",
    "special_item_description": "Một hiện vật mạnh mẽ", 
    "shop_cat_item_hat": "Mũ Sang Trọng"
}
```

## Usage

### Thay đổi ngôn ngữ

```csharp
public class GameManager : IInitializable
{
    private readonly LocalizationManager localizationManager;

    public GameManager(LocalizationManager localizationManager)
    {
        this.localizationManager = localizationManager;
    }

    public async void ChangeToVietnamese()
    {
        await this.localizationManager.ChangeLanguageAsync("vi");
        
        // Sau khi hoàn thành, tất cả blueprint fields đã được localize
        // Bạn có thể cập nhật UI
        this.RefreshUI();
    }
    
    private void RefreshUI()
    {
        // Access blueprint data như bình thường
        // Content đã được tự động localize
        this.View.TxtContent.text = this.specialItemBlueprint.GetDataById(model.SpecialItemId).Content;
    }
}
```

### Listen localization events

```csharp
public class UIController : IInitializable, IDisposable
{
    private readonly SignalBus signalBus;

    public void Initialize()
    {
        this.signalBus.Subscribe<LanguageChangedSignal>(this.OnLanguageChanged);
        this.signalBus.Subscribe<BlueprintLocalizationCompletedSignal>(this.OnLocalizationCompleted);
    }

    private void OnLanguageChanged(LanguageChangedSignal signal)
    {
        Debug.Log($"Language changed from {signal.OldLanguage} to {signal.NewLanguage}");
    }

    private void OnLocalizationCompleted(BlueprintLocalizationCompletedSignal signal)
    {
        Debug.Log($"Localized {signal.LocalizedFieldsCount} fields in {signal.BlueprintCount} blueprints");
        
        // Refresh all UI that uses blueprint data
        this.RefreshAllUI();
    }
}
```

## How It Works

1. **Initialization**: `BlueprintLocalizationService` quét tất cả blueprints và cache các field có `[LocalizableField]`
2. **Language Change**: Khi gọi `LocalizationManager.ChangeLanguageAsync()`, hệ thống:
   - Load localization data mới
   - Fire `LanguageChangedSignal`
   - `BlueprintLocalizationService` nhận signal và update tất cả cached fields
   - Fire `BlueprintLocalizationCompletedSignal`
3. **Runtime Access**: Code game access blueprint data như bình thường, các field đã được localize

## Localization Key Rules

1. **Custom Key**: Nếu có `[LocalizableField("custom_key")]`, sử dụng `custom_key`
2. **Original Value**: Nếu không có custom key, sử dụng giá trị gốc của field làm key
3. **Fallback**: Nếu không tìm thấy localization, giữ nguyên giá trị gốc

## Advanced Features

### Multiple Languages

```csharp
// Lấy danh sách ngôn ngữ có sẵn
var languages = localizationManager.AvailableLanguages;

// Lấy ngôn ngữ hiện tại
var current = localizationManager.CurrentLanguage;
```

### Direct Localization Access

```csharp
// Lấy localized string trực tiếp
var localizedText = localizationManager.GetLocalizedString("some_key", "fallback");

// Kiểm tra key có tồn tại không
if (localizationManager.HasKey("some_key"))
{
    // ...
}
```

## Example Blueprint Setup

```csharp
[BlueprintReader("SpecialItem")] 
public class SpecialItemBlueprint : GenericBlueprintReaderByRow<string, SpecialItemRecord>
{
}

public class SpecialItemRecord
{
    public string Id { get; set; }
    public string Thumbnail { get; set; }
    public string Icon { get; set; }
    
    // Tự động localize với original value làm key
    [LocalizableField]
    public string Content { get; set; }
}
```

Khi runtime:
```csharp
// Trước khi đổi ngôn ngữ: Content = "This is content"
// Sau khi đổi sang tiếng Việt: Content = "Đây là nội dung" (nếu có trong file vi.json)
```

## File Structure

```
Assets/UITemplate/Scripts/Localization/
├── LocalizableFieldAttribute.cs          # Attribute để mark fields
├── ILocalizationProvider.cs              # Interface cho provider
├── ResourcesLocalizationProvider.cs      # Default provider (load từ Resources)
├── LocalizationManager.cs                # Main API cho game
├── BlueprintLocalizationService.cs       # Core service xử lý blueprint localization
├── LocalizationVContainer.cs             # DI registration
├── Signals/
│   └── LocalizationSignals.cs           # All localization signals
└── Examples/
    └── LocalizationUsageExample.cs      # Usage examples

Resources/Localization/
├── en.json                               # English localization
├── vi.json                               # Vietnamese localization  
├── zh.json                               # Chinese localization
└── ...                                   # Other languages
```

## Notes

- Hệ thống chỉ hoạt động với các blueprint đã được load
- Cần đảm bảo file localization có format JSON đúng
- Performance: Reflection operations được cache để tối ưu
- Thread-safe: Tất cả operations chạy trên main thread
