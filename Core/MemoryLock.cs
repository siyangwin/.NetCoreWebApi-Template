using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace Project.Core
{
	/// <summary>
	/// 内存锁
	/// </summary>
	public class MemoryLock : IDisposable
	{
		private string key { get; set; }
		private int timeOut { get; set; }
		/// <summary>
		/// 内存锁
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="timeOut">超时时间</param>
		public MemoryLock(string key, int timeOut = 60)
		{
			this.key = "MemoryLock_" + key;
			this.timeOut = timeOut;
			this.Wait();
		}
		/// <summary>
		/// 等待
		/// </summary>
		public void Wait()
		{
			object obj = MemoryCacheHelper.Get(key);
			if (obj != null)
			{
				//此处不能使用Task.Delay(500);
				Thread.Sleep(300);
				Wait();
			}
			else
			{
				MemoryCacheHelper.Set(key, key as object, TimeSpan.FromSeconds(timeOut));
				Thread.Sleep(100);
			}
		}
		/// <summary>
		/// 释放锁
		/// </summary>
		public void Dispose()
		{
			MemoryCacheHelper.Remove(key);
		}
	}

    /// <summary>
    /// 緩存-给锁用
    /// </summary>
    public static class MemoryCacheHelper
	{
		private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
		/// <summary>
		/// 驗證緩存項是否存在
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <returns></returns>
		public static bool Exists(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			return Cache.TryGetValue(key, out _);
		}

		/// <summary>
		/// 添加緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <param name="value">緩存Value</param>
		/// <param name="expiresSliding">滑動過期時長（如果在過期時間內有操作，則以當前時間點延長過期時間）</param>
		/// <param name="expiressAbsoulte">絕對過期時長</param>
		/// <returns></returns>
		public static bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			Cache.Set(key, value,
				new MemoryCacheEntryOptions().SetSlidingExpiration(expiresSliding)
					.SetAbsoluteExpiration(expiressAbsoulte));
			return Exists(key);
		}

		/// <summary>
		/// 添加緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <param name="value">緩存Value</param>
		/// <param name="expiresIn">緩存時長</param>
		/// <param name="isSliding">是否滑動過期（如果在過期時間內有操作，則以當前時間點延長過期時間）</param>
		/// <returns></returns>
		public static bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			Cache.Set(key, value,
				isSliding
					? new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn)
					: new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));

			return Exists(key);
		}
		/// <summary>
		/// 添加不过期緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <param name="value">緩存Value</param>
		/// <returns></returns>
		public static bool Set<T>(string key, T value)
			 where T : new()
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				value = new T();

			Cache.Set(key, value
				);
			return Exists(key);
		}

		#region 刪除緩存

		/// <summary>
		/// 刪除緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <returns></returns>
		public static void Remove(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			Cache.Remove(key);
		}

		/// <summary>
		/// 批量刪除緩存
		/// </summary>
		/// <returns></returns>
		public static void RemoveAll(IEnumerable<string> keys)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));

			keys.ToList().ForEach(item => Cache.Remove(item));
		}
		#endregion

		#region 獲取緩存

		/// <summary>
		/// 獲取緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <returns></returns>
		public static T Get<T>(string key) where T : class
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return Cache.Get(key) as T;
		}

		private static object getMemoryLock = new object();
		/// <summary>
		/// 獲取緩存
		/// </summary>
		/// <param name="key">緩存Key</param>
		/// <returns></returns>
		public static object Get(string key)
		{
			//获取缓存时上锁
			lock (getMemoryLock)
			{
				if (key == null)
					throw new ArgumentNullException(nameof(key));

				return Cache.Get(key);
			}
		}

		/// <summary>
		/// 獲取緩存集合
		/// </summary>
		/// <param name="keys">緩存Key集合</param>
		/// <returns></returns>
		public static IDictionary<string, object> GetAll(IEnumerable<string> keys)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));

			var dict = new Dictionary<string, object>();
			keys.ToList().ForEach(item => dict.Add(item, Cache.Get(item)));
			return dict;
		}
		#endregion

		/// <summary>
		/// 刪除所有緩存
		/// </summary>
		public static void RemoveCacheAll()
		{
			var l = GetCacheKeys();
			foreach (var s in l)
			{
				Remove(s);
			}
		}

		/// <summary>
		/// 刪除匹配到的緩存
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static void RemoveCacheRegex(string pattern)
		{
			IList<string> l = SearchCacheRegex(pattern);
			foreach (var s in l)
			{
				Remove(s);
			}
		}

		/// <summary>
		/// 搜索 匹配到的緩存
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static IList<string> SearchCacheRegex(string pattern)
		{
			var cacheKeys = GetCacheKeys();
			var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
			return l.AsReadOnly();
		}

		/// <summary>
		/// 獲取所有緩存鍵
		/// </summary>
		/// <returns></returns>
		public static List<string> GetCacheKeys()
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
			var cacheItems = entries as IDictionary;
			var keys = new List<string>();
			if (cacheItems == null) return keys;
			foreach (DictionaryEntry cacheItem in cacheItems)
			{
				keys.Add(cacheItem.Key.ToString());
			}
			return keys;
		}


	}
}
