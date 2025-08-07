using System.Runtime.InteropServices;
using UltralightNet.Enums;

namespace UltralightNet.Structs;

internal static unsafe partial class Methods
{
	[DllImport(UltralightNet.Methods.LibUltralight)]
	public static extern UlMouseEvent* ulCreateMouseEvent(MouseEventType type, int x, int y,
		MouseEventButton button);

	[DllImport(UltralightNet.Methods.LibUltralight)]
	public static extern void ulDestroyMouseEvent(UlMouseEvent* evt);
}

/// <summary>
///     Mouse Event
/// </summary>
public struct UlMouseEvent : IEquatable<UlMouseEvent>
{
	private int _type;

	public MouseEventType Type
	{
		readonly get => UltralightNet.Methods.BitCast<int, MouseEventType>(_type);
		set => _type = UltralightNet.Methods.BitCast<MouseEventType, int>(value);
	}

	public int X;
	public int Y;
	private int _button;

	public MouseEventButton Button
	{
		readonly get => UltralightNet.Methods.BitCast<int, MouseEventButton>(_button);
		set => _button = UltralightNet.Methods.BitCast<MouseEventButton, int>(value);
	}

	public readonly bool Equals(UlMouseEvent other)
	{
		return Type == other.Type && X == other.X && Y == other.Y && Button == other.Button;
	}

	public override bool Equals(object obj)
	{
		return obj is UlMouseEvent && Equals((UlMouseEvent)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_type, X, Y, _button);
	}
}
