using Common.Mod.Common.Core;
using Vintagestory.API.Config;

namespace Common.Mod.Core;

public class Translations : ITranslations
{
    private readonly string _modId;
    private readonly string? _languageCode;

    public Translations(string modId)
    {
        _modId = modId;
        _languageCode = null;
    }

    public Translations(string modId, string languageCode)
    {
        _modId = modId;
        _languageCode = languageCode;
    }

    public string GetL(string languageCode, string key, params object[] args)
    {
        var translation = Lang.GetL(languageCode, $"{_modId}:{key}", args);

        if (string.IsNullOrWhiteSpace(translation) || translation == key)
        {
            translation = Lang.GetL(Lang.CurrentLocale, $"{_modId}:{key}", args);
        }

        return translation;
    }

    public string Get(string key, params object[] args) => GetL(_languageCode!, key, args);
}
