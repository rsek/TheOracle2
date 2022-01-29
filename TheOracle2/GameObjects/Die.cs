﻿namespace TheOracle2.GameObjects;

/// <summary>
/// Represents a single game die with an arbitrary number of sides and the number it currently shows.
/// </summary>
public class Die : IComparable<int>, IComparable<Die>
{
    /// <summary>
    /// Rolls or sets a game die.
    /// </summary>
    /// <param name="sides">The number of sides the die has (minimum 2)</param>
    /// <param name="value">A optional preset value for the die. If this is omitted, the die will be rolled and show a random side.</param>
    /// <exception cref="ArgumentOutOfRangeException">If sides is less than 2; if value is less than 1; if value exceeds sides</exception>
    public Die(Random random, int sides, int? value = null)
    {
        if (sides < 2) { throw new ArgumentOutOfRangeException(nameof(sides), "Die must have at least 2 sides."); }
        if (value != null && (value > sides || value < 1)) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be null, or a positive integer less than the number of sides on the die."); }

        Random = random;
        Sides = sides;
        Value = value ?? Roll();
    }

    public int CompareTo(int other)
    {
        return this.Value.CompareTo(other);
    }

    public int CompareTo(Die other)
    {
        return this.Value.CompareTo(other.Value);
    }

    private readonly Random Random;

    public static implicit operator int(Die die)
    {
        return die.Value;
    }

    public static implicit operator string(Die die)
    {
        return die.Value.ToString();
    }

    public int Sides { get; }
    public int Value { get; set; }

    private int Roll()
    {
        return Random.Next(1, Sides + 1);
    }

    /// <summary>
    /// Re-roll the die to randomize its Value.
    /// </summary>
    public void Reroll()
    {
        Value = Roll();
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
