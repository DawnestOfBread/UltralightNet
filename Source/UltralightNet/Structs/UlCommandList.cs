using System.Runtime.CompilerServices;

namespace UltralightNet.Structs;

public unsafe ref struct UlCommandList
{
	public uint Size;
	public UlCommand* CommandsPtr;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<UlCommand> AsSpan()
	{
		return new ReadOnlySpan<UlCommand>(CommandsPtr, checked((int)Size));
	}
}
