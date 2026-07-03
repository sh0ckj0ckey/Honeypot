using System;
using System.Security.Cryptography;
using System.Text;

namespace Honeypot.Helpers;

public static class PasswordGenerator
{
    private static readonly char[] LetterCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static readonly char[] NumberCharacters = "0123456789".ToCharArray();

    private static readonly char[] SymbolCharacters = "!@#$%&*".ToCharArray();

    /// <summary>
    /// Generates a random password by selecting characters from the enabled character groups.
    /// </summary>
    /// <param name="includeLetters">Whether letters are included.</param>
    /// <param name="includeNumbers">Whether numbers are included.</param>
    /// <param name="includeSymbols">Whether symbols are included.</param>
    /// <param name="length">The password length.</param>
    /// <returns>A randomly generated password.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="length"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when no character group is selected.
    /// </exception>
    public static string GeneratePassword(bool includeLetters, bool includeNumbers, bool includeSymbols, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        StringBuilder characterPoolBuilder = new();

        if (includeLetters)
        {
            characterPoolBuilder.Append(LetterCharacters);
        }

        if (includeNumbers)
        {
            characterPoolBuilder.Append(NumberCharacters);
        }

        if (includeSymbols)
        {
            characterPoolBuilder.Append(SymbolCharacters);
        }

        if (characterPoolBuilder.Length == 0)
        {
            throw new ArgumentException("At least one character group must be selected.");
        }

        char[] characterPool = characterPoolBuilder.ToString().ToCharArray();
        StringBuilder passwordBuilder = new(length);

        for (int i = 0; i < length; i++)
        {
            int index = RandomNumberGenerator.GetInt32(characterPool.Length);
            passwordBuilder.Append(characterPool[index]);
        }

        return passwordBuilder.ToString();
    }
}
