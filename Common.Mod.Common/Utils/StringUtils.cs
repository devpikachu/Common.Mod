namespace Common.Mod.Common.Utils;

public static class StringUtils
{
    private const string CharacterPool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static readonly Random Rng = new();

    public static string Random(int length = 8) => new(Enumerable.Repeat(CharacterPool, length).Select(s => s[Rng.Next(s.Length)]).ToArray());
}
