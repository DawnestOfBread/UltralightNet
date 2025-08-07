using UltralightNet.Enums;

namespace UltralightNet.Structs;

/// <summary>
///     Scroll event
/// </summary>
public struct UlScrollEvent : IEquatable<UlScrollEvent>
{
	private int _type;

	/// <summary>
	///     Type of event
	/// </summary>
	public ScrollEventType Type
	{
		readonly get => UltralightNet.Methods.BitCast<int, ScrollEventType>(_type);
		set => _type = UltralightNet.Methods.BitCast<ScrollEventType, int>(value);
	}

	/// <summary>
	///     horizontal scroll
	/// </summary>
	public int DeltaX;

	/// <summary>
	///     vertical scroll
	/// </summary>
	public int DeltaY;

	public readonly bool Equals(UlScrollEvent other)
	{
		return Type == other.Type && DeltaX == other.DeltaX && DeltaY == other.DeltaY;
	}

	public override bool Equals(object? obj)
	{
		return obj is UlScrollEvent @event && Equals(@event);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_type, DeltaX, DeltaY);
	}
}
