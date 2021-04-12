using System;
using System.Text.RegularExpressions;

namespace CoreLibrary.Utilities
{
    public static class Create
    {
        public static string ShortUID(bool fixedLength = true)
        {
            var guid = Guid.NewGuid().ToByteArray();
            return fixedLength
                ? Base64.EncodeBase64URL(guid).Substring(0, 22)
                : Regex.Replace(Base64.EncodeBase64(guid), "[/+=]", "");
        }
    }
}
