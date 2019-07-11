﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NumSharp.Memory.Pooling;
using NumSharp.Utilities;

namespace NumSharp.Backends.Unmanaged
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UnmanagedMemoryBlock<T> : IMemoryBlock, IEnumerable<T>, IEquatable<UnmanagedMemoryBlock<T>> where T : unmanaged
    {
        private Action _dispose;
        private GCHandle? _gcHandle;
        public int Count;
        public T* Address;
        public int BytesCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        [MethodImpl((MethodImplOptions)512)]
        public UnmanagedMemoryBlock(int length)
        {
            //var bytes = length * SizeOf<T>.Size;
            //if (false)
            //{
            //    //bytes > AllocHGlobalAfter
            //    Address = (T*)Marshal.AllocHGlobal(bytes).ToPointer();
            //    _gcHandle = null;
            //}
            //else
            //{
            var handle = GCHandle.Alloc(new T[length], GCHandleType.Pinned);
            _gcHandle = handle;
            Address = (T*)handle.AddrOfPinnedObject();
            //}

            _dispose = null;
            Count = length;
            BytesCount = InfoOf<T>.Size * Count;
        }

        /// <summary>
        ///     Construct with externally allocated memory and a custom <paramref name="dispose"/> function.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        /// <param name="dispose"></param>
        [MethodImpl((MethodImplOptions)768)]
        public UnmanagedMemoryBlock(T* start, int length, Action dispose)
        {
            Count = length;
            BytesCount = InfoOf<T>.Size * Count;
            _dispose = dispose;
            Address = start;
            _gcHandle = null;
        }

        /// <summary>
        ///     Construct with externally allocated memory and a custom <paramref name="dispose"/> function.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        [MethodImpl((MethodImplOptions)768)]
        internal UnmanagedMemoryBlock(GCHandle handle, int length)
        {
            Count = length;
            BytesCount = InfoOf<T>.Size * Count;
            _dispose = null;
            Address = (T*)handle.AddrOfPinnedObject();
            _gcHandle = handle;
        }

        /// <summary>
        ///     Construct with externally allocated memory and a custom <paramref name="dispose"/> function.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        /// <param name="dipose"></param>
        [MethodImpl((MethodImplOptions)768)]
        internal UnmanagedMemoryBlock(GCHandle handle, int length, Action dipose)
        {
            Count = length;
            BytesCount = InfoOf<T>.Size * Count;
            _dispose = dipose;
            Address = (T*)handle.AddrOfPinnedObject();
            _gcHandle = handle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        /// <param name="fill"></param>
        [MethodImpl((MethodImplOptions)768)]
        public UnmanagedMemoryBlock(int length, T fill) : this(length)
        {
            new Span<T>(Address, length).Fill(fill);
        }

        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> FromArray(T[] arr)
        {
            return new UnmanagedMemoryBlock<T>(GCHandle.Alloc(arr, GCHandleType.Pinned), arr.Length);
        }

        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> FromArray(T[] arr, bool copy)
        {
            if (!copy)
                return new UnmanagedMemoryBlock<T>(GCHandle.Alloc(arr, GCHandleType.Pinned), arr.Length);

            var ret = new UnmanagedMemoryBlock<T>(arr.Length);
            new Span<T>(arr).CopyTo(ret.AsSpan());

            return ret;
        }

        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> FromBuffer(byte[] arr)
        {
            return new UnmanagedMemoryBlock<T>(GCHandle.Alloc(arr, GCHandleType.Pinned), arr.Length / InfoOf<T>.Size);
        }

        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> FromBuffer(byte[] arr, bool copy)
        {
            return new UnmanagedMemoryBlock<T>(GCHandle.Alloc(copy ? arr.Clone() : arr, GCHandleType.Pinned), arr.Length / InfoOf<T>.Size);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="length">The length in objects of <typeparamref name="T"/> and not in bytes.</param>
        /// <param name="manager"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> FromPool(int length, InternalBufferManager manager)
        {
            //TODO! Upgrade InternalBufferManager to use pre-pinned arrays.
            var buffer = manager.TakeBuffer(length * InfoOf<T>.Size);
            return new UnmanagedMemoryBlock<T>(GCHandle.Alloc(buffer, GCHandleType.Pinned), length, () => manager.ReturnBuffer(buffer));
        }

        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> Copy(UnmanagedMemoryBlock<T> source)
        {
            var itemCount = source.Count;
            var len = itemCount * InfoOf<T>.Size;
            var ret = new UnmanagedMemoryBlock<T>(itemCount);
            Buffer.MemoryCopy(source.Address, ret.Address, len, len);
            //source.AsSpan().CopyTo(ret.AsSpan()); //TODO! Benchmark at netcore 3.0, it should be faster than buffer.memorycopy.
            return ret;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="address">The address of the first <typeparamref name="T"/></param>
        /// <param name="count">How many <typeparamref name="T"/> to copy, not how many bytes.</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> Copy(void* address, int count)
        {
            var len = count * InfoOf<T>.Size;
            var ret = new UnmanagedMemoryBlock<T>(count);
            Buffer.MemoryCopy(address, ret.Address, len, len);
            //source.AsSpan().CopyTo(ret.AsSpan()); //TODO! Benchmark at netcore 3.0, it should be faster than buffer.memorycopy.
            return ret;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count">How many <typeparamref name="T"/> to copy, not how many bytes.</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> Copy(IntPtr address, int count)
        {
            return Copy((void*)address, count);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="address">The address of the first <typeparamref name="T"/></param>
        /// <param name="count">How many <typeparamref name="T"/> to copy, not how many bytes.</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public static UnmanagedMemoryBlock<T> Copy(T* address, int count)
        {
            return Copy((void*)address, count);
        }

        public T this[int index]
        {
            [MethodImpl((MethodImplOptions)768)] get => *(Address + index);
            [MethodImpl((MethodImplOptions)768)] set => *(Address + index) = value;
        }

        [MethodImpl((MethodImplOptions)512)]
        public void Reallocate(int length, bool copyOldValues = false)
        {
            if (copyOldValues)
            {
                var @new = new UnmanagedMemoryBlock<T>(length);
                var bytes = Math.Min(Count, length) * InfoOf<T>.Size;
                Buffer.MemoryCopy(Address, @new.Address, bytes, bytes);
                Free();
                this = @new;
            }
            else
            {
                //we do not have to allocate first in we do not copy values.
                Free();
                this = new UnmanagedMemoryBlock<T>(length);
            }
        }

        [MethodImpl((MethodImplOptions)512)]
        public void Reallocate(int length, T fill, bool copyOldValues = false)
        {
            if (copyOldValues)
            {
                var @new = new UnmanagedMemoryBlock<T>(length);
                var bytes = Math.Min(Count, length) * InfoOf<T>.Size;
                Buffer.MemoryCopy(Address, @new.Address, bytes, bytes);

                if (length > Count)
                {
                    new Span<T>(@new.Address + Count, length - Count).Fill(fill);
                }

                Free();
                this = @new;
            }
            else
            {
                //we do not have to allocate first in we do not copy values.
                Free();
                this = new UnmanagedMemoryBlock<T>(length, fill);
            }
        }

        [MethodImpl((MethodImplOptions)768)]
        public T GetIndex(int index)
        {
            return *(Address + index);
        }

        [MethodImpl((MethodImplOptions)768)]
        public ref T GetRefTo(int index)
        {
            return ref *(Address + index);
        }

        [MethodImpl((MethodImplOptions)768)]
        public void SetIndex(int index, ref T value)
        {
            *(Address + index) = value;
        }

        [MethodImpl((MethodImplOptions)768)]
        public void SetIndex(int index, T value)
        {
            *(Address + index) = value;
        }

        [MethodImpl((MethodImplOptions)512)]
        public void Free()
        {
            if (_dispose != null)
            {
                _dispose();
                _dispose = null;
                return;
            }

            if (_gcHandle.HasValue)
            {
                _gcHandle.Value.Free();
                _gcHandle = null;
                Address = null;
                return;
            }

            if (Address != null)
            {
                Marshal.FreeHGlobal((IntPtr)Address);
                Address = null;
                return;
            }
        }

        [MethodImpl((MethodImplOptions)768)]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) yield return GetIndex(i);
        }

        [MethodImpl((MethodImplOptions)768)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// NotSupported
        /// </summary>
        [MethodImpl((MethodImplOptions)512)]
        public void Clear()
        {
            throw new NotSupportedException();
        }

        [MethodImpl((MethodImplOptions)768)]
        public bool Contains(T item)
        {
            int len = Count;
            for (var i = 0; i < len; i++)
            {
                if ((*(Address + i)).Equals(item)) return true;
            }

            return false;
        }

        [MethodImpl((MethodImplOptions)512)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            int len = Count;
            for (var i = 0; i < len; i++)
            {
                array[i + arrayIndex] = *(Address + i);
            }
        }

        [MethodImpl((MethodImplOptions)512)]
        public void CopyTo(UnmanagedMemoryBlock<T> memoryBlock, int arrayIndex)
        {
            Buffer.MemoryCopy(Address + arrayIndex, memoryBlock.Address, InfoOf<T>.Size * memoryBlock.Count, InfoOf<T>.Size * (Count - arrayIndex));
        }

        [MethodImpl((MethodImplOptions)512)]
        public void CopyTo(T* array, int arrayIndex, int lengthToCopy)
        {
            Buffer.MemoryCopy(Address + arrayIndex, array, InfoOf<T>.Size * lengthToCopy, InfoOf<T>.Size * lengthToCopy);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing. </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        [MethodImpl((MethodImplOptions)512)]
        public void CopyTo(Array array, int arrayIndex)
        {
            if (!(array is T[] arr))
                throw new InvalidCastException("Unable to CopyTo a type that is not " + typeof(T).Name + "[]");

            CopyTo(arr, arrayIndex);
        }

        [MethodImpl((MethodImplOptions)768)]
        public Span<T> AsSpan() => new Span<T>(Address, Count);

        #region Explicit Implementations

        /// <summary>
        ///     The size of a single item stored in <see cref="IMemoryBlock.Address"/>.
        /// </summary>
        int IMemoryBlock.ItemLength => InfoOf<T>.Size;

        /// <summary>
        ///     The start address of this memory block.
        /// </summary>
        unsafe void* IMemoryBlock.Address => Address;

        /// <summary>
        ///     How many items are stored in <see cref="IMemoryBlock.Address"/>?
        /// </summary>
        /// <remarks></remarks>
        int IMemoryBlock.Count => Count;

        /// <summary>
        ///     The items with length of <see cref="IMemoryBlock.TypeCode"/> are present in <see cref="IMemoryBlock.Address"/>.
        /// </summary>
        /// <remarks>Calculated by <see cref="IMemoryBlock.Count"/>*<see cref="IMemoryBlock.ItemLength"/></remarks>
        int IMemoryBlock.BytesLength => BytesCount;

        /// <summary>
        ///     The <see cref="NPTypeCode"/> of the type stored inside this memory block.
        /// </summary>
        NPTypeCode IMemoryBlock.TypeCode => InfoOf<T>.NPTypeCode;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref T GetPinnableReference()
        {
            return ref *Address;
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        [MethodImpl((MethodImplOptions)768)]
        public bool Equals(UnmanagedMemoryBlock<T> other)
        {
            return Count == other.Count && Address == other.Address;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />. </returns>
        [MethodImpl((MethodImplOptions)768)]
        public override bool Equals(object obj)
        {
            return obj is UnmanagedMemoryBlock<T> other && Equals(other);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [MethodImpl((MethodImplOptions)768)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (Count * 397) ^ unchecked((int)(long)Address);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:NumSharp.Backends.Unmanaged.UnmanagedArray`1" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        [MethodImpl((MethodImplOptions)768)]
        public static bool operator ==(UnmanagedMemoryBlock<T> left, UnmanagedMemoryBlock<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:NumSharp.Backends.Unmanaged.UnmanagedArray`1" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        [MethodImpl((MethodImplOptions)768)]
        public static bool operator !=(UnmanagedMemoryBlock<T> left, UnmanagedMemoryBlock<T> right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}