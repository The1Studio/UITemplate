## Các component chính

### 1. UITemplateAutoLocalization
- **Mục đích**: Tự động setup LocalizeStringEvent cho TextMeshPro components
- **Cách sử dụng**: Attach vào GameObject có TextMeshPro, bấm nút "SetUpStringEvent"
- **Vị trí**: `UITemplate/Scripts/Localization/Utils/UITemplateAutoLocalization.cs`

### 2. TMPLocalization (Editor)
- **Mục đích**: Tools trong Editor để extract và add localization keys
- **Features**:
  - Setup localization cho text của  prefabs được ref trên addressable
- **Vị trí**: `UITemplate/Editor/Localization/TMPLocalization.cs`

### 3. LocalizationHelper
- **Mục đích**: Utility methods để làm việc với localization
- **Methods**:
  - `GetLocalizationStringAsync()` - Lấy string async
  - `GetLocalizationString()` - Lấy string sync
  - `SetTextLocalize()` - Set text với localization
  - `SetTextLocalizeFormat()` - Set text với format params
- **Vị trí**: `UITemplate/Scripts/Utils/LocalizationHelper.cs`

## ⚠️ VẤN ĐỀ HIỆN TẠI VÀ TODO

### 🐛 Blueprint Localization Issue
**Vấn đề**: Blueprint localize các field tự động nhưng KHÔNG có tác dụng khi không SetText/BindData.

**Chi tiết**:
- Khi blueprint field được extract và add vào localization table, các text component trong prefab được setup LocalizeStringEvent
- Tuy nhiên, localization chỉ hoạt động sau khi:
  - Gọi `SetText()` method trên text component, HOẶC  
  - Gọi `BindData()` để refresh UI binding

**Root cause**: LocalizeStringEvent không dùng sau khi setup với auto localize blueprint => không tự động apply ngay lập tức.

### 📋 TODO cho developer tiếp theo:

1. **Fix Blueprint Auto-Localization**:
   ```csharp
   // Cần thêm code trigger localization sau khi đổi ngôn ngữ mà không cần phải settext lại (bindata):
   ```

2. **Alternative Solutions**:
   - Sử dụng `LocalizationHelper.SetTextLocalize()` thay vì rely vào auto-setup
   
3. **Enhance TMPLocalization.cs**:
   - Thêm option auto-refresh sau khi setup
   - Add validation check xem localization có hoạt động không

### 💡 WORKAROUND hiện tại:

**Sử dụng LocalizationHelper**
```csharp
using TheOneStudio.UITemplate.UITemplate.Utils;

// Thay vì rely vào auto-localization:
textComponent.SetTextLocalize("Your_Localization_Key", "Game");

```