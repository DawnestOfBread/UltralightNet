using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("UltralightNet.AppCore")]
#if NET7_0_OR_GREATER
[assembly: DisableRuntimeMarshalling]
#endif

#if RELEASE
[module: SkipLocalsInit]
#endif

namespace UltralightNet;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
internal static unsafe partial class Methods
{
	public const string LibUltralight = "Ultralight";

	static Methods()
	{
		Preload();
	}

	[LibraryImport(LibUltralight)]
	public static partial byte* ulVersionString();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionMajor();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionMinor();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionPatch();

	/// <summary>
	///     Preload Ultralight binaries on OSX/MacOS
	/// </summary>
	/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
	public static void Preload()
	{
		#if NET5_0_OR_GREATER
		bool isOsx = OperatingSystem.IsMacOS();
		if (isOsx)
		{
			ReadOnlySpan<string> libsOsx =
 new[] { "libUltralightCore.dylib", "libWebCore.dylib", "libUltralight.dylib" };

			string? absoluteAssemblyLocationDir = Path.GetDirectoryName(typeof(Methods).Assembly.Location);
			if (string.IsNullOrEmpty(absoluteAssemblyLocationDir)) return;
			string absoluteRuntimeNativesDir =
 Path.Combine(absoluteAssemblyLocationDir, "runtimes", "osx-x64", "native");

			var assembly = typeof(UltralightNet.Binaries.Binaries).Assembly;
			const DllImportSearchPath searchPath = DllImportSearchPath.UseDllDirectoryForDependencies;

			foreach (string lib in libsOsx)
			{
				if (!NativeLibrary.TryLoad(lib, assembly, searchPath, out nint _))
				{
					string absoluteRuntimeNative = Path.Combine(absoluteRuntimeNativesDir, lib);
					NativeLibrary.TryLoad(absoluteRuntimeNative, assembly, searchPath, out nint _);
				}
			}
		}
		#endif
	}

	// backported from net8.0 for compatibility
	internal static TTo BitCast<TFrom, TTo>(TFrom from) where TFrom : unmanaged where TTo : unmanaged
	#if !NET8_0_OR_GREATER
	{
		Debug.Assert(sizeof(TFrom) == sizeof(TTo));
		return Unsafe.As<TFrom, TTo>(ref from);
	}
	#else
	=> Unsafe.BitCast<TFrom, TTo>(from);
	#endif
}
