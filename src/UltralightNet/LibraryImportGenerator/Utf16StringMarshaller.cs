#if !NET7_0_OR_GREATER
namespace System.Runtime.InteropServices.Marshalling;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(Utf16StringMarshaller))]
public static unsafe class Utf16StringMarshaller
{
	public static string? ConvertToManaged(ushort* unmanaged)
	{
		return Marshal.PtrToStringUni((nint)unmanaged);
	}

	public static ushort* ConvertToUnmanaged(string? managed)
	{
		return (ushort*)Marshal.StringToHGlobalUni(managed);
	}

	public static void Free(ushort* unmanaged)
	{
		Marshal.FreeHGlobal((nint)unmanaged);
	}

	public static ref readonly char GetPinnableReference(string? str)
	{
		return ref str.AsSpan().GetPinnableReference();
	}
}
#endif
