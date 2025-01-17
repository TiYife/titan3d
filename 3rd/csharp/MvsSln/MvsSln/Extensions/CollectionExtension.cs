﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Foreach in Linq manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="act">The action that should be executed for each item.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> act)
        {
            return items?.ForEach((x, i) => act(x));
        }

        /// <summary>
        /// Foreach in Linq manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="act">The action that should be executed for each item.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T, long> act)
        {
            if(items == null) {
                return null;
            }

            long n = 0;
            foreach(var item in items) {
                act(item, n++);
            }
            return items;
        }

        /// <summary>
        /// Adds/Updates data in source via data from `items`.
        /// 
        /// Any duplicates will be just updated:
        /// ie. similar to `Concat()` except internal restriction for `Insert()`.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns>Updated source.</returns>
        public static IDictionary<string, string> AddOrUpdate(this IDictionary<string, string> source, IEnumerable<KeyValuePair<string, string>> items)
        {
            if(source == null || items == null) {
                return source;
            }

            foreach(var i in items) {
                source[i.Key] = i.Value;
            }
            return source;
        }

        /// <summary>
        /// Returns either value from dictionary or configured default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="def">Use this if key is not found.</param>
        /// <returns></returns>
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> data, TKey key, TVal def = default)
        {
            if(data == null) {
                return def;
            }
            return data.ContainsKey(key) ? data[key] : def;
        }

        /// <summary>
        /// Removes element from list by using specific comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="elem"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool Remove<T>(this IList<T> src, T elem, Func<T, T, bool> comparer)
        {
            for(int i = 0; i < src.Count; ++i)
            {
                if(comparer(src[i], elem)) {
                    src.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}