// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

namespace VindemiatrixCollective.Terminalizer
{
    public static class StringExtensions
    {
        public static string Bold(this string text) => "<b>" + text + "</b>";

        public static string Colour(this string text, string colour) => $"<color={colour}>{text}</color>";

        public static string Highlight(this string text) => text.Colour(Colours.Highlight);

        public static string KeyColour(this string text) => text.Colour(Colours.BrightInfo);

        public static string Accent(this string text) => text.Colour(Colours.Accent);
        public static string Warning(this string text) => text.Colour(Colours.Warning);

        public static string Error(this string text) => text.Colour(Colours.Error);
        public static string Success(this string text) => text.Colour(Colours.Success);
        public static string Info(this string text) => text.Colour(Colours.Info);

        public static string Colour(this int value, string colour) => $"<color={colour}>{value.ToString()}</color>";

        public static string Italic(this string text) => "<i>" + text + "</i>";

        public static string Size(this string text, int size) => $"<size={size}>{text}</size>";

        public static string Plural(this string text) => text + "s";

        public static string Prepend(this string text, string prefix) => prefix + text;
    }
}