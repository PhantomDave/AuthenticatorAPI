using System.Text.RegularExpressions;

namespace AuthenticatorAPI.Regexs
{
    public partial class Rgex
    {
        [GeneratedRegex("\\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\\Z", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex EmailRegex();
    }
}
