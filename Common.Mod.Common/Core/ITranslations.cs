namespace Common.Mod.Common.Core;

public interface ITranslations
{
    public string GetL(string languageCode, string key, params object[] args);
    public string Get(string key, params object[] args);
}
