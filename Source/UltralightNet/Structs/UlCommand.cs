using UltralightNet.Enums;

namespace UltralightNet.Structs;

public struct UlCommand : IEquatable<UlCommand>
{
	private byte _commandType;

	public CommandType CommandType
	{
		readonly get => Methods.BitCast<byte, CommandType>(_commandType);
		set => _commandType = Methods.BitCast<CommandType, byte>(value);
	}

	public UlGpuState GpuState;

	/// <remarks>Only used when <see cref="CommandType" /> is <see cref="Enums.CommandType.DrawGeometry" /></remarks>
	public uint GeometryId;

	/// <remarks>Only used when <see cref="CommandType" /> is <see cref="Enums.CommandType.DrawGeometry" /></remarks>
	public uint IndicesCount;

	/// <remarks>Only used when <see cref="CommandType" /> is <see cref="Enums.CommandType.DrawGeometry" /></remarks>
	public uint IndicesOffset;

	public readonly bool Equals(UlCommand other)
	{
		return CommandType == other.CommandType && GpuState.Equals(other.GpuState) && GeometryId == other.GeometryId &&
		       IndicesCount == other.IndicesCount && IndicesOffset == other.IndicesOffset;
	}
}
