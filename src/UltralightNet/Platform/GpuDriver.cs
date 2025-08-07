using System.Collections.Generic;
using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;
using UltralightNet.Structs;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		///     <see cref="IGpuDriver" /> native definition.
		/// </summary>
		public unsafe struct UlGpuDriver
		{
			#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<void> BeginSynchronize;
			public delegate* unmanaged[Cdecl]<void> EndSynchronize;
			public delegate* unmanaged[Cdecl]<uint> NextTextureId;
			public delegate* unmanaged[Cdecl]<uint, void*, void> CreateTexture;
			public delegate* unmanaged[Cdecl]<uint, void*, void> UpdateTexture;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyTexture;
			public delegate* unmanaged[Cdecl]<uint> NextRenderBufferId;
			public delegate* unmanaged[Cdecl]<uint, UlRenderBuffer, void> CreateRenderBuffer;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyRenderBuffer;
			public delegate* unmanaged[Cdecl]<uint> NextGeometryId;
			public delegate* unmanaged[Cdecl]<uint, UlVertexBuffer, UlIndexBuffer, void> CreateGeometry;
			public delegate* unmanaged[Cdecl]<uint, UlVertexBuffer, UlIndexBuffer, void> UpdateGeometry;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyGeometry;
			public delegate* unmanaged[Cdecl]<UlCommandList, void> UpdateCommandList;
			#else
			public void* BeginSynchronize,
				EndSynchronize,
				NextTextureId,
				CreateTexture,
				UpdateTexture,
				DestroyTexture,
				NextRenderBufferId,
				CreateRenderBuffer,
				DestroyRenderBuffer,
				NextGeometryId,
				CreateGeometry,
				UpdateGeometry,
				DestroyGeometry,
				UpdateCommandList;
			#endif
		}
	}

	public interface IGpuDriver
	{
		uint NextTextureId();
		void CreateTexture(uint textureId, UlBitmap bitmap);
		void UpdateTexture(uint textureId, UlBitmap bitmap);
		void DestroyTexture(uint textureId);

		uint NextRenderBufferId();
		void CreateRenderBuffer(uint renderBufferId, UlRenderBuffer renderBuffer);
		void DestroyRenderBuffer(uint renderBufferId);

		uint NextGeometryId();
		void CreateGeometry(uint geometryId, UlVertexBuffer vertexBuffer, UlIndexBuffer indexBuffer);
		void UpdateGeometry(uint geometryId, UlVertexBuffer vertexBuffer, UlIndexBuffer indexBuffer);
		void DestroyGeometry(uint geometryId);

		void UpdateCommandList(UlCommandList commandList);

		#if !NETSTANDARD2_0
		virtual UlGpuDriver? GetNativeStruct()
		{
			return null;
		}
		#else
		ULGPUDriver? GetNativeStruct();
		#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			private readonly UlGpuDriver _nativeStruct;

			private readonly Dictionary<nint, WeakReference<UlBitmap>>? _bitmapCache;

			private readonly GCHandle[]? _handles;

			private readonly IGpuDriver _instance;
			private uint _newCachedInstanceCount;

			public Wrapper(IGpuDriver instance)
			{
				_instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				if (instance is IGpuDriverSynchronized sync)
				{
					_handles = new GCHandle[14];

					NativeStruct = NativeStruct with
					{
						BeginSynchronize =
						(delegate* unmanaged[Cdecl]<void>)Helper.AllocateDelegate(sync.BeginSynchronize,
							out _handles[12]),
						EndSynchronize =
						(delegate* unmanaged[Cdecl]<void>)Helper.AllocateDelegate(sync.EndSynchronize, out _handles[13])
					};
				}
				else
				{
					_handles = new GCHandle[12];
				}

				_bitmapCache = new Dictionary<nint, WeakReference<UlBitmap>>(32);

				NativeStruct = NativeStruct with
				{
					NextTextureId =
					(delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextTextureId,
						out _handles[0]),
					CreateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Helper.AllocateDelegate(
						(uint id, void* bitmap) => instance.CreateTexture(id, BitmapFromHandleCached(bitmap)),
						out _handles[1]),
					UpdateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Helper.AllocateDelegate(
						(uint id, void* bitmap) => instance.UpdateTexture(id, BitmapFromHandleCached(bitmap)),
						out _handles[2]),
					DestroyTexture =
					(delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(
						instance.DestroyTexture, out _handles[3]),
					NextRenderBufferId =
					(delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextRenderBufferId,
						out _handles[4]),
					CreateRenderBuffer =
					(delegate* unmanaged[Cdecl]<uint, UlRenderBuffer, void>)Helper
						.AllocateDelegate<RenderBufferCallback>(instance.CreateRenderBuffer, out _handles[5]),
					DestroyRenderBuffer =
					(delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(
						instance.DestroyRenderBuffer, out _handles[6]),
					NextGeometryId =
					(delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextGeometryId,
						out _handles[7]),
					CreateGeometry =
					(delegate* unmanaged[Cdecl]<uint, UlVertexBuffer, UlIndexBuffer, void>)Helper
						.AllocateDelegate<GeometryCallback>(instance.CreateGeometry, out _handles[8]),
					UpdateGeometry =
					(delegate* unmanaged[Cdecl]<uint, UlVertexBuffer, UlIndexBuffer, void>)Helper
						.AllocateDelegate<GeometryCallback>(instance.UpdateGeometry, out _handles[9]),
					DestroyGeometry =
					(delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(
						instance.DestroyGeometry, out _handles[10]),
					UpdateCommandList =
					(delegate* unmanaged[Cdecl]<UlCommandList, void>)Helper.AllocateDelegate<CommandListCallback>(
						instance.UpdateCommandList, out _handles[11])
				};
			}

			public UlGpuDriver NativeStruct
			{
				get
				{
					if (IsDisposed) throw new ObjectDisposedException(nameof(Wrapper));
					return _nativeStruct;
				}
				private init => _nativeStruct = value;
			}

			public bool IsDisposed { get; private set; }

			public void Dispose()
			{
				if (IsDisposed) return;
				if (_handles is not null)
					foreach (var handle in _handles)
						if (handle.IsAllocated)
							handle.Free();

				GC.SuppressFinalize(this);
				IsDisposed = true;
			}

			private UlBitmap BitmapFromHandleCached(void* ptr)
			{
				if (!_bitmapCache!.TryGetValue((nint)ptr, out var weakBitmap) ||
				    !weakBitmap.TryGetTarget(out var bitmap))
				{
					bitmap = UlBitmap.FromHandle(ptr, false);
					_bitmapCache[(nint)ptr] = new WeakReference<UlBitmap>(bitmap);
					_newCachedInstanceCount++;
				}

				if (_newCachedInstanceCount > 256)
				{
					foreach (var keyValuePair in _bitmapCache)
						if (!keyValuePair.Value.TryGetTarget(out _))
							_bitmapCache.Remove(keyValuePair.Key);
					_newCachedInstanceCount = 0;
				}

				return bitmap;
			}

			~Wrapper()
			{
				Dispose();
			}

			private delegate uint IdCallback();

			private delegate void RenderBufferCallback(uint id, UlRenderBuffer renderBuffer);

			private delegate void GeometryCallback(uint id, UlVertexBuffer vertexBuffer, UlIndexBuffer indexBuffer);

			private delegate void DestroyIdCallback(uint id);

			private delegate void CommandListCallback(UlCommandList commandList);
		}
	}

	public interface IGpuDriverSynchronized : IGpuDriver
	{
		/// <summary>Called before any commands are dispatched during a frame.</summary>
		void BeginSynchronize();

		/// <summary>Called after any commands are dispatched during a frame.</summary>
		void EndSynchronize();
	}
}
