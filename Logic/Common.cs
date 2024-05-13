namespace Logic;

public static class Common
{
    public static string GetGuid(this string text)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        var hash = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
        var guid =  new Guid(hash).ToString().Replace("-", "");
        return "c" + guid;
    }
}
