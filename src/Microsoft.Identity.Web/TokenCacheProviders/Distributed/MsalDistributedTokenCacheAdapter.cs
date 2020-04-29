// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Microsoft.Identity.Web.TokenCacheProviders.Distributed
{
    /// <summary>
    /// An implementation of the token cache for both Confidential and Public clients backed by MemoryCache.
    /// </summary>
    /// <seealso cref="https://aka.ms/msal-net-token-cache-serialization"/>
    public class MsalDistributedTokenCacheAdapter : MsalAbstractTokenCacheProvider
    {
        /// <summary>
        /// .NET Core Memory cache.
        /// </summary>
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Msal memory token cache options.
        /// </summary>
        private readonly DistributedCacheEntryOptions cacheOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsalDistributedTokenCacheAdapter"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="microsoftIdentityOptions"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="memoryCache"></param>
        /// <param name="cacheOptions"></param>
        public MsalDistributedTokenCacheAdapter(
                                            IOptions<MicrosoftIdentityOptions> microsoftIdentityOptions,
                                            IHttpContextAccessor httpContextAccessor,
                                            IDistributedCache memoryCache,
                                            IOptions<DistributedCacheEntryOptions> cacheOptions) 
            : base(microsoftIdentityOptions, httpContextAccessor)
        {
            distributedCache = memoryCache;
            this.cacheOptions = cacheOptions.Value;
        }

        protected override async Task RemoveKeyAsync(string cacheKey)
        {
            await distributedCache.RemoveAsync(cacheKey).ConfigureAwait(false);
        }

        protected override async Task<byte[]> ReadCacheBytesAsync(string cacheKey)
        {
            return await distributedCache.GetAsync(cacheKey).ConfigureAwait(false);
        }

        protected override async Task WriteCacheBytesAsync(string cacheKey, byte[] bytes)
        {
            await distributedCache.SetAsync(cacheKey, bytes, cacheOptions).ConfigureAwait(false);
        }
    }
}
