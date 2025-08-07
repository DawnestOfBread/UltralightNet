namespace UltralightNet.Structs;

public struct UlRenderBuffer
{
	public uint TextureId;
	public uint Width;
	public uint Height;

	private byte _hasStencilBuffer;

	public bool HasStencilBuffer
	{
		readonly get => UltralightNet.Methods.BitCast<byte, bool>(_hasStencilBuffer);
		set => _hasStencilBuffer = UltralightNet.Methods.BitCast<bool, byte>(value);
	}

	private byte _hasDepthBuffer;

	public bool HasDepthBuffer
	{
		readonly get => UltralightNet.Methods.BitCast<byte, bool>(_hasDepthBuffer);
		set => _hasDepthBuffer = UltralightNet.Methods.BitCast<bool, byte>(value);
	}
}
