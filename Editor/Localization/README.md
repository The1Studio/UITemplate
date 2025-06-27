# Simple Unity Localization Tool

A simplified Unity localization tool focused on core functionality: adding entries to string tables and GPT-powered translation.

## 🎯 Core Features

✅ **Add Entry**: Add localization entries to Unity string tables  
✅ **GPT Translation**: Automatic translation using OpenAI GPT API  
✅ **Context Menu**: Right-click text components for quick localization  
✅ **Simple Dashboard**: Clean interface for managing entries  
✅ **Build Validation**: Check for missing translations during build  

## 🚀 How to Use

### 1. Initial Setup
1. **Install Unity Localization package** (Package Manager)
2. **Create config**: `Tools > TheOne > Localization > Create Config`
3. **Get OpenAI API key**: 
   - Go to https://platform.openai.com/api-keys
   - Create new API key (starts with `sk-proj-`)
   - **Add payment method** to your OpenAI account (required for usage beyond free tier)
4. **Enter API key** in the config file
5. **Create String Table Collections** in Unity Localization Settings

### 2. Adding Entries (3 methods)

#### Method 1: Context Menu (⚡ Fastest)
- Right-click on **Text** or **TextMeshPro** component
- Select **"Add to Localization"**
- Enter key and confirm
- Component will automatically be set up with LocalizeStringEvent

#### Method 2: Dashboard
- Open `Tools > TheOne > Localization > Open Dashboard`
- Enter **Key** and **English Text**
- Click **"Add Entry to String Tables"**

#### Method 3: Quick Add
- `Tools > TheOne > Localization > Quick Add Entry`
- Quick popup window for adding entries

### 3. Translation
- In **Dashboard**, click **"Translate All Missing Entries"**
- Tool will:
  - Find all untranslated entries
  - Show count and cost estimate
  - Automatically translate using GPT API
  - Update Unity string tables

### 4. Complete Workflow
```
Right-click Text → "Add to Localization" → Open Dashboard → "Translate All Missing" → Done! 🎉
```

## 📁 File Structure

- **AutoLocalizationConfig.cs** - Configuration (OpenAI API, target languages)
- **AutoLocalizationManager.cs** - Core logic (add entry + translate)
- **SimpleLocalizationDashboard.cs** - UI dashboard
- **LocalizationMenuItems.cs** - Menu items và quick add window
- **LocalizationContextMenu.cs** - Right-click context menu
- **AutoLocalizationBuildProcessor.cs** - Build validation
- **RuntimeLocalizationHelper.cs** - Runtime support

## ⚙️ Configuration

### Target Languages
In **AutoLocalizationConfig**, enable/disable languages:
- English (en) - Source language
- Vietnamese (vi) 
- Japanese (ja)
- Korean (ko)
- Chinese (zh)
- Spanish (es)
- French (fr)
- German (de)

### Translation Settings
- **Batch Size**: 5-20 entries per request
- **Delay**: 1 second between requests
- **Game Context**: Game description for GPT context understanding
- **Translation Instructions**: Style and tone guidelines for translation

## 🔧 Requirements

- **Unity 2022.3+**
- **Unity Localization Package** (1.4.0+)
- **Newtonsoft.Json Package** (automatically added to manifest.json)
- **OpenAI API Key** with billing setup (https://platform.openai.com/api-keys)
  - ⚠️ **Important**: Free tier has very limited quota (~$5), add payment method for production use
- **Odin Inspector** (optional, for better UI)

## 💡 Best Practices

1. **Key Naming**: Use format like `ui_button_start`, `dialog_welcome_message`
2. **Source Language**: Always use English as source
3. **Context**: Add game context for accurate translation
4. **Batch Size**: Limit to 5-20 entries to avoid timeouts
5. **Cost Control**: Check translation count before running (typically $0.01-0.03 per entry)
6. **Build Check**: Enable build validation to catch missing translations
7. **API Quota**: Monitor your OpenAI usage at https://platform.openai.com/account/usage

## 🎮 Game Integration

### Setup Runtime Locale Switching
```csharp
// Use RuntimeLocalizationHelper
RuntimeLocalizationHelper.SetLocale("vi"); // Switch to Vietnamese
RuntimeLocalizationHelper.SetLocale("en"); // Switch to English
```

### Auto-detect User Language
```csharp
// Tool will automatically detect system language if available
```

## 🐛 Troubleshooting

### OpenAI API Errors

#### ❌ "insufficient_quota" or "TooManyRequests"
This means your OpenAI account has exceeded its usage quota:
- **Check Billing**: Go to https://platform.openai.com/account/billing
- **Add Payment Method**: Add a credit card to your OpenAI account
- **Check Usage**: View your current usage at https://platform.openai.com/account/usage
- **Upgrade Plan**: Free tier has very limited quota, consider upgrading
- **Wait**: If you hit rate limits, wait and try again later

#### ❌ "invalid_api_key" 
- ✅ Check API key is correctly copied (should start with `sk-proj-`)
- ✅ Regenerate key at https://platform.openai.com/api-keys if needed
- ✅ Ensure no extra spaces or characters

#### ❌ Network/Connection Errors
- ✅ Check internet connection
- ✅ Try again in a few minutes
- ✅ Check if OpenAI service is down at https://status.openai.com/

### Missing Translations
- ✅ Run build to see missing list
- ✅ Use Dashboard to translate in batches
- ✅ Check target languages are enabled

### Context Menu not showing
- ✅ Ensure Text/TextMeshPro component exists
- ✅ Check script compilation

### Build Failed
- ✅ Enable/disable auto build validation
- ✅ Fix missing translations or continue anyway

## 📊 Dashboard Features

- **Current Entries Table**: View all entries with translations
- **Missing Translations**: Show languages that need translation
- **Quick Stats**: Total entries, translated, missing counts
- **Batch Operations**: Add sample entries, translate all

## 🌟 Advantages

- **Simple**: Only 2 main functions - Add entry + Translate
- **Fast**: Right-click workflow for quick operations
- **Automatic**: GPT translation with context awareness
- **Safe**: Build validation catches errors
- **Flexible**: Support for multiple languages
- **Clean**: No complexity like Google Sheets integration

## 🔄 Upgrading from old version

If you have an old complex version:
1. Remove Google Sheets files
2. Use new simplified version
3. Existing entries will be migrated automatically

**Tool now focuses 100% on core workflow: Add Entry → Translate → Done!** 🚀
