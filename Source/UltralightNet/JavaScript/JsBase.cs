// JSBase.h

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Those are 1:1 definitions")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Compatibility")]
internal static unsafe partial class JavaScriptMethods
{
	private const string LibWebCore = "WebCore";

	static JavaScriptMethods()
	{
		Methods.Preload();
	}

	internal static Exception UnsupportedMethodException => new NotSupportedException("Method is not supported");

	internal static void ThrowUnsupportedConstructor()
	{
		throw new NotSupportedException("Constructor is not supported");
	}

	[LibraryImport(LibWebCore)]
	public static partial JsValueRef JSEvaluateScript(JsContextRef context, JsStringRef script, JsObjectRef thisObject,
		JsStringRef sourceURL, int startingLineNumber, JsValueRef* exception = null);

	[LibraryImport(LibWebCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool JSCheckScriptSyntax(JsContextRef context, JsStringRef script, JsStringRef sourceURL,
		int startingLineNumber, JsValueRef* exception = null);

	[LibraryImport(LibWebCore)]
	public static partial void JSGarbageCollect(JsContextRef context);
}

public abstract unsafe class JsNativeContainer<TNativeHandle> : NativeContainer where TNativeHandle : unmanaged
{
	public TNativeHandle JsHandle
	{
		get => Methods.BitCast<nuint, TNativeHandle>((nuint)Handle);
		protected init => Handle = (void*)Methods.BitCast<TNativeHandle, nuint>(value);
	}
}
