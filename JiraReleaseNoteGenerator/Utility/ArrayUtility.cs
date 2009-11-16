// Copyright (c) 2009 rubicon informationstechnologie gmbh
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using System;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.Utility
{
  /// <summary>
  /// Provides utility functions that make common array operations easier.
  /// </summary>
  public static class ArrayUtility
  {
    public static bool Contains (Array array, object value)
    {
      return Array.IndexOf (array, value) >= 0;
    }

    public static bool IsNullOrEmpty (Array array)
    {
      return (array == null) || (array.Length == 0);
    }

    public static bool IsNullOrEmpty (ICollection collection)
    {
      return (collection == null) || (collection.Count == 0);
    }

    public static T[] Combine<T> (params T[][] arrays)
    {
      ArgumentUtility.CheckNotNull ("arrays", arrays);
      if (arrays.Length == 0)
        return new T[0];

      int newLength = 0;
      for (int i = 0; i < arrays.Length; ++i)
        newLength += arrays[i].Length;

      T[] result = new T[newLength];
      int start = 0;
      for (int i = 0; i < arrays.Length; ++i)
      {
        arrays[i].CopyTo (result, start);
        start += arrays[i].Length;
      }
      return result;
    }

    public static T[] Combine<T> (T[] array, T item)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      T[] result = new T[array.Length + 1];
      for (int i = 0; i < array.Length; ++i)
        result[i] = array[i];
      result[array.Length] = item;
      return result;
    }

    public static T[] Combine<T> (T item, T[] array)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      T[] result = new T[array.Length + 1];
      result[0] = item;
      for (int i = 0; i < array.Length; ++i)
        result[i + 1] = array[i];
      return result;
    }

    public static TResult[] Convert<TSource, TResult> (ICollection<TSource> collection)
        where TResult: TSource
    {
      if (collection == null)
        return null;

      TResult[] result = (TResult[]) Array.CreateInstance (typeof (TResult), collection.Count);
      collection.CopyTo ((TSource[]) (Array) result, 0);
      return result;
    }

    public static Array Convert (Array array, Type elementType)
    {
      int rank = array.Rank;
      int[] lengths = new int[rank];
      int[] lowerBounds = new int[rank];
      for (int dimension = 0; dimension < rank; ++dimension)
      {
        lengths[dimension] = array.GetLength (dimension);
        lowerBounds[dimension] = array.GetLowerBound (dimension);
      }

      Array result = Array.CreateInstance (elementType, lengths, lowerBounds);
      array.CopyTo (result, 0);
      return result;
    }

    public static Array Convert (ICollection collection, Type elementType)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("elementType", elementType);

      Array result = Array.CreateInstance (elementType, collection.Count);
      collection.CopyTo (result, 0);
      return result;
    }

    public static T[] Convert<T> (ICollection<T> collection)
    {
      return Convert<T, T> (collection);
    }

    public static T[] Insert<T> (T[] original, int index, T value)
    {
      ArgumentUtility.CheckNotNull ("original", original);
      T[] result = new T[original.Length + 1];

      for (int i = 0; i < index; ++i)
        result[i] = original[i];

      result[index] = value;

      for (int i = index; i < original.Length; ++i)
        result[i + 1] = original[i];

      return result;
    }

    public static T[] Skip<T> (T[] array, int num)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      if (num > array.Length)
        throw new ArgumentOutOfRangeException ("num", "Number of items to skip greater than array size.");
      T[] result = new T[array.Length - num];
      for (int i = num; i < array.Length; ++i)
        result[i - num] = array[i];
      return result;
    }
  }
}