namespace Honeypot.Helpers;

/// <summary>
/// Provides helpers for extracting a grouping initial from text.
/// </summary>
/// <remarks>
/// Chinese characters are converted by using NPinyin first and then falling back
/// to Microsoft PinYinConverter for characters that NPinyin cannot resolve.
/// For other text, the helper returns an uppercase grouping initial when possible.
/// Latin letters with diacritics are normalized to their base letter when possible,
/// while letters from other scripts, such as Cyrillic or Greek, are typically
/// returned as their uppercase first letter. If no suitable letter can be
/// determined, <c>#</c> is returned.
/// </remarks>
public static class InitialHelper
{
    /// <summary>
    /// Gets a grouping initial from the specified text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>
    /// An uppercase initial suitable for grouping when possible; otherwise, <c>#</c>.
    /// </returns>
    public static char GetGroupingInitial(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return '#';
        }

        string trimmedText = text.Trim();
        string firstElement = System.Globalization.StringInfo.GetNextTextElement(trimmedText);

        if (string.IsNullOrEmpty(firstElement))
        {
            return '#';
        }

        string normalizedElement = firstElement.Normalize(System.Text.NormalizationForm.FormC);

        if (normalizedElement.Length != 1)
        {
            return '#';
        }

        char firstChar = normalizedElement[0];

        if (char.IsDigit(firstChar))
        {
            return '#';
        }

        if (firstChar is >= 'A' and <= 'Z')
        {
            return firstChar;
        }

        if (firstChar is >= 'a' and <= 'z')
        {
            return char.ToUpperInvariant(firstChar);
        }

        if (TryGetChinesePinyinInitial(firstChar, out char chineseInitial))
        {
            return chineseInitial;
        }

        char normalizedLetter = NormalizeLetter(firstChar);

        if (char.IsLetter(normalizedLetter))
        {
            return char.ToUpperInvariant(normalizedLetter);
        }

        return '#';
    }

    /// <summary>
    /// Tries to get the uppercase pinyin initial for a Chinese character.
    /// </summary>
    /// <param name="character">The Chinese character to convert.</param>
    /// <param name="initial">When this method returns, contains the uppercase initial if conversion succeeds.</param>
    /// <returns><see langword="true"/> if a pinyin initial was resolved; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetChinesePinyinInitial(char character, out char initial)
    {
        initial = default;

        try
        {
            string pinyin = NPinyin.Pinyin.GetPinyin(character);

            if (!string.IsNullOrWhiteSpace(pinyin) && !Microsoft.International.Converters.PinYinConverter.ChineseChar.IsValidChar(pinyin[0]))
            {
                char firstLetter = NormalizeLetter(pinyin[0]);

                if (char.IsLetter(firstLetter))
                {
                    initial = char.ToUpperInvariant(firstLetter);
                    return true;
                }
            }
        }
        catch
        {
            // Ignore and continue with the fallback converter.
        }

        try
        {
            if (!Microsoft.International.Converters.PinYinConverter.ChineseChar.IsValidChar(character))
            {
                return false;
            }

            Microsoft.International.Converters.PinYinConverter.ChineseChar chineseChar = new(character);

            foreach (string value in chineseChar.Pinyins)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                // The last character is the tone number.
                string syllable = char.IsDigit(value[^1]) ? value[..^1] : value;

                if (string.IsNullOrWhiteSpace(syllable))
                {
                    continue;
                }

                char firstLetter = NormalizeLetter(syllable[0]);

                if (char.IsLetter(firstLetter))
                {
                    initial = char.ToUpperInvariant(firstLetter);
                    return true;
                }
            }
        }
        catch
        {
            // Ignore and fall back to generic letter normalization.
        }

        return false;
    }

    /// <summary>
    /// Normalizes a letter by removing diacritics when possible.
    /// </summary>
    /// <param name="character">The character to normalize.</param>
    /// <returns>
    /// The normalized base character when one can be determined; otherwise, the original character.
    /// </returns>
    private static char NormalizeLetter(char character)
    {
        string normalized = character.ToString().Normalize(System.Text.NormalizationForm.FormD);

        foreach (char ch in normalized)
        {
            System.Globalization.UnicodeCategory category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);

            if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                return ch;
            }
        }

        return character;
    }
}
