# Auto Translation Tool for Unity

A simplified Unity localization tool focused ONLY on auto translation using ChatGPT.

## 🎯 Core Features

✅ **GPT Translation**: Automatic translation using OpenAI GPT API  
✅ **Translation Status**: View missing translations  
✅ **Build Validation**: Check for missing translations during build  

❌ **Removed Features**: Adding entries, context menu, dashboard entry management  
➡️ **Use TMPLocalization instead**: All entry adding is handled by TMPLocalization tool

## 🚀 How to Use

### 1. Initial Setup
1. **Install Unity Localization package** (Package Manager)
2. **Create config**: `Tools > TheOne > Localization > Create Config`
3. **Get OpenAI API key**: 
   - Go to https://platform.openai.com/api-keys
   - Create new API key (starts with `sk-proj-`)
   - **Add payment method** to your OpenAI account (required for usage beyond free tier)
4. **Enter API key** in the config file

### 2. Workflow
1. **Use TMPLocalization** to add entries and setup localization
2. **Open Auto Translation Tool**: `Tools > TheOne > Localization > Auto Translation Tool`  
3. **Click "Translate All Missing Entries"**
4. **Done!** 🎉

### 3. Features

#### Auto Translation Tool
- Open `Tools > TheOne > Localization > Auto Translation Tool`
- View translation status for all entries
- One-click translate all missing entries
- Progress bar with detailed status

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

1. **Use TMPLocalization**: For adding entries and setting up components
2. **Source Language**: Always use English as source
3. **Context**: Add game context for accurate translation
4. **Batch Size**: Limit to 5-20 entries to avoid timeouts
5. **Cost Control**: Check translation count before running (typically $0.01-0.03 per entry)
6. **Build Check**: Enable build validation to catch missing translations
7. **API Quota**: Monitor your OpenAI usage at https://platform.openai.com/account/usage

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
- ✅ Use Auto Translation Tool to translate in batches

## 📊 Tool Features

- **Translation Status Table**: View all entries with missing translation info
- **One-Click Translation**: Translate all missing entries at once
- **Progress Tracking**: Real-time progress bar during translation
- **Error Handling**: Detailed error messages for failed translations

## 🌟 Advantages

- **Focused**: Only handles translation, TMPLocalization handles entry management
- **Simple**: One-click translation workflow
- **Automatic**: GPT translation with context awareness
- **Safe**: Build validation catches errors
- **Flexible**: Support for multiple languages

## 🔄 Integration with TMPLocalization

1. **TMPLocalization**: Use for adding entries, setting up components
2. **Auto Translation Tool**: Use for translating entries to other languages
3. **Perfect workflow**: Add entries → Translate → Done!

**Tool now focuses 100% on translation only!** 🚀
