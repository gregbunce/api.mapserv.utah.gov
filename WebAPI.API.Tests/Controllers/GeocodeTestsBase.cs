﻿using NUnit.Framework;

namespace WebAPI.API.Tests.Controllers
{
    public class GeocodeTestsBase
    {
        [OneTimeSetUp]
        public void SetupFixture()
        {
            CacheConfig.BuildCache();
        }
    }
}