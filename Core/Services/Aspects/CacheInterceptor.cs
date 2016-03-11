using System;
using System.Reflection;
using System.Runtime.Caching;
using Castle.DynamicProxy;

namespace Server.Services.Aspects
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ObjectCache _objectCache;

        public CacheInterceptor(ObjectCache objectCache)
        {
            _objectCache = objectCache;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInvocationTarget = invocation.MethodInvocationTarget;

            string cacheKey = CacheKeyHelper.GetCacheKey(methodInvocationTarget, invocation.Arguments);
            object cacheValue = _objectCache.Get(cacheKey);

            if (cacheValue != null)
            {
                invocation.ReturnValue = cacheValue;
                return;
            }

            invocation.Proceed();

            _objectCache.Add(cacheKey, invocation.ReturnValue, new DateTimeOffset(DateTime.Now.AddMinutes(5)));
        }
    }
}
