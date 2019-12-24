namespace PickAll
{
    static class StringExtensions
    {
        public static bool IsAlphanumeric(this string value)
        {
            var chars = value.ToCharArray();
            foreach (var c in chars) {
                if (!char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)) {
                    return false;
                }
            }
            return true;
        }
    }
}