using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.RedisObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 作者：Ark
    /// 时间：2016.12.20
    /// 描述：RedisCache公用方法
    /// </summary>
    public class RedisCacheHelper
    {
        /// <summary>
        /// redis连接字符串
        /// </summary>
        public static readonly string RedisConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];//  "RedisConnectionString".GetAppSetting<string>();

        /// <summary>
        /// RedisCache上下文对象延迟
        /// </summary>
        private static readonly Lazy<RedisContext> LazyContext = new Lazy<RedisContext>(() => new RedisContext(RedisConnectionString));

        /// <summary>
        /// RedisCache上下文对象
        /// </summary>
        public static RedisContext Context => LazyContext.Value;

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：Redis连接状态
        /// </summary>
        /// <returns>true：成功</returns>
        private static bool CanConnected()
        {
            try
            {
                return Context.GetConnectionMultiplexer().IsConnected;
            }
            catch (Exception ex)
            {
                // 连接失败日志
                //LogHelper.Error("Redis 连接异常", ex);
            }

            return false;
        }

        #region Object

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：设置redis
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="value">redis value</param>
        /// <param name="minutes">过期时间（分钟）</param>
        /// <returns>true：成功</returns>
        public static bool SetByMin(string key, object value, int? minutes = null)
        {
            TimeSpan? expiry = null;

            if (minutes.HasValue)
                expiry = TimeSpan.FromMinutes(minutes.Value);

            return Set(key, value, expiry);
        }

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：设置redis
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="value">redis value</param>
        /// <param name="expiry">过期时间</param>
        /// <returns>true：成功</returns>
        public static bool Set(string key, object value, TimeSpan? expiry = null)
        {
            if (!CanConnected() || string.IsNullOrWhiteSpace(key))
                return false;

            try
            {
                Context.Cache.SetObject(key, value, expiry); //value.GetType().IsValueType
                return true;
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis Set异常，[key={key}]", ex);
            }

            return false;
        }

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：获取redis
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">redis key</param>
        /// <returns>redis value</returns>
        public static T Get<T>(string key)
        {
            bool flag;
            return Get<T>(key, out flag);
        }

        /// <summary>
        /// 作者：Ark
        /// 时间：2015.10.20
        /// 描述：获取redis
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="flag">返回空值或错误为false，否则为true</param>
        /// <returns></returns>
        public static T Get<T>(string key, out bool flag)
        {
            if (CanConnected() && !string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    var result = Context.Cache.GetObject<T>(key);
                    flag = null != result;
                    return result;
                }
                catch (Exception ex)
                {
                    // 异常日志
                    //LogHelper.Error($"Redis Get异常，[key={key}]", ex);
                }
            }

            flag = false;
            return default(T);
        }

        #endregion

        #region Hash

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：设置hash值
        /// </summary>
        /// <param name="redisKey">redis key</param>
        /// <param name="fieldKey">field key</param>
        /// <param name="value">redis value</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public static bool SetHashed(string redisKey, object fieldKey, object value, TimeSpan? expiry = null)
        {
            if (!CanConnected() || string.IsNullOrWhiteSpace(redisKey) || fieldKey == null)
                return false;

            try
            {
                //暂时不处理标签
                Context.Cache.SetHashed(redisKey, fieldKey, value, null, expiry);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.Error($"Redis SetHashed Exception [key={redisKey}]", ex);
            }

            return false;
        }

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：获取hash值
        /// </summary>
        /// <param name="redisKey">redis key</param>
        /// <param name="fieldKey">field key</param>
        /// <param name="flag">true成功，否则失败</param>
        /// <returns></returns>
        public static T GetHashed<TK, T>(string redisKey, TK fieldKey, out bool flag)
        {
            if (CanConnected())
            {
                try
                {
                    flag = true;
                    return Context.Cache.GetHashed<TK, T>(redisKey, fieldKey);
                }
                catch (Exception ex)
                {
                    // 异常日志
                    //LogHelper.Error($"Redis GetHashed Exception [key={redisKey}]", ex);
                }
            }

            flag = false;
            return default(T);
        }

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：获取hash值
        /// </summary>
        /// <param name="redisKey">redis key</param>
        /// <param name="fieldKeys">field key list</param>
        /// <param name="flag">true成功，否则失败</param>
        /// <returns></returns>
        public static List<T> GetHashed<TK, T>(string redisKey, IEnumerable<TK> fieldKeys, out bool flag)
        {
            if (CanConnected())
            {
                try
                {
                    var list = new ConcurrentQueue<T>();
                    Parallel.ForEach(fieldKeys, fieldKey =>
                    {
                        var cache = Context.Cache.GetHashed<TK, T>(redisKey, fieldKey);
                        if (cache != null)
                        {
                            list.Enqueue(cache);
                        }
                    });

                    flag = true;
                    return list.ToList();
                }
                catch (Exception ex)
                {
                    // 异常日志
                    //LogHelper.Error($"Redis GetHashed Exception [key={redisKey}]", ex);
                }
            }

            flag = false;
            return new List<T>();
        }

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：获取所有hash值
        /// </summary>
        /// <param name="redisKey">redis key</param>
        /// <param name="flag">true成功，否则失败</param>
        /// <returns></returns>
        public static List<T> GetHashedAll<T>(string redisKey, out bool flag)
        {
            if (CanConnected())
            {
                try
                {
                    var cache = Context.Cache.GetHashedAll<T>(redisKey);
                    if (cache != null)
                    {
                        flag = true;
                        return cache.Values.ToList();
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.Error($"Redis GetHashedAll Exception [key={redisKey}]", ex);
                }
            }

            flag = false;
            return new List<T>();
        }


        #endregion

        #region  Set

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：设置set值
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="value">redis value</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public static bool AddToSet(string key, object value, TimeSpan? expiry = null)
        {
            if (!CanConnected() || string.IsNullOrWhiteSpace(key))
                return false;

            try
            {
                Context.Cache.AddToSet(key, value, null, expiry);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.Error($"Redis AddToSet Exception [key={key}]", ex);
            }

            return false;
        }

        /// <summary>
        /// 作者：Elvis
        /// 时间：2017.09.25
        /// 描述：获取set值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public static IRedisSet<T> GetRedisSet<T>(string key)
        {
            if (!CanConnected()) return null;

            try
            {
                return Context.Collections.GetRedisSet<T>(key);
            }
            catch (Exception ex)
            {
                //LogHelper.Error($"Redis GetRedisSet Exception [key={key}]", ex);
            }

            return null;
        }

        #endregion

        #region Check

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：redis key是否存在
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>true存在</returns>
        public static bool KeyExist(string key)
        {
            if (!CanConnected()) return false;

            try
            {
                return Context.Cache.KeyExists(key);
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis KeyExist异常，[key={key}]", ex);
            }

            return false;
        }

        #endregion

        #region Remove

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：移除redis
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            if (!CanConnected()) return false;

            try
            {
                return Context.Cache.Remove(key);
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis Remove异常，[key={key}]", ex);
            }

            return false;
        }

        /// <summary>
        /// 作者：Ark
        /// 时间：2016.12.20
        /// 描述：移除redis
        /// </summary>
        /// <param name="keys">redis keys</param>
        public static void Remove(string[] keys)
        {
            if (!CanConnected()) return;
            if (null == keys || keys.Length == 0) return;

            try
            {
                Context.Cache.Remove(keys);
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis Remove异常，[keys={string.Join(",", keys)}]", ex);
            }
        }

        /// <summary>
        /// 作者：Ark
        /// 时间：2015.01.23
        /// 描述：移除redis所有符合给定模式 pattern 的 key 
        /// </summary>
        /// <param name="pattern">
        /// 给定模式（像正则表达式）
        ///     * 匹配数据库中所有 key
        ///     ? 匹配一个任意字符
        ///     h?llo 匹配 hello ， hallo 和 hxllo 等
        ///     h*llo 匹配 hllo 和 heeeeello 等
        ///     h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo
        ///     特殊符号用 \ 隔开。
        /// </param>
        /// <returns></returns>
        public static bool RemoveByPattern(string pattern)
        {
            if (!CanConnected()) return false;

            try
            {
                var keys = Context.Cache.GetKeysByPattern(pattern).ToArray();
                Context.Cache.Remove(keys);
                return true;
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis RemoveByPattern异常，[pattern={pattern}]", ex);
            }

            return false;
        }

        /// <summary>
        /// 从set集合中移除指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="value">指定值</param>
        public static void RemoveFromSet<T>(string key, T value)
        {
            if (!CanConnected()) return;
            if (null == key) return;

            try
            {
                Context.Cache.RemoveFromSet<T>(key, value);
            }
            catch (Exception ex)
            {
                // 异常日志
                //LogHelper.Error($"Redis Remove异常，[key={key},value={value}]", ex);
            }
        }

        #endregion

    }
}
