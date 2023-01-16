using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Finbourne.Api.Domain;
using Finbourne.Cache.Component;
using Finbourne.Cache.Component.Configuration;
using Xunit;

namespace Finbourne.UnitTests
{
    public class FinbourneCacheServiceTests
    {
        public FinbourneCacheServiceTests()
        {

        }

        [Fact]
        public async Task Should_Add_New_Cache_Item_When_There_is_No_Cache_Items()
        {
            // Arrange
            var memoryCache = new FinbourneMemoryCache();
            var sut = new FinbourneCacheService(memoryCache);
            var customObject = new CustomObject()
            {
                Key = "1",
                Value = 1
            };
            // Act
            await sut.Set(customObject.Key, customObject);

            // Assert
            var response = sut.Get<CustomObject>(customObject.Key);

            Assert.Matches(customObject.Key, response.Key);
            Assert.Equal(customObject.Value, response.Value);
        }

        [Fact]
        public async Task Should_Force_Evict_LRU_From_Cache_When_Cache_Size_Limit_Has_been_Met_and_Exceeded()
        {
            // Arrange
            var memoryCache = new FinbourneMemoryCache();
            var sut = new FinbourneCacheService(memoryCache);

            var sizeLimit = long.Parse(ConfigurationManager.AppSettings["Finbourne.MemoryCache.SizeLimit"]);

            var myCollectiion = new List<CustomObject>();

            for (var i = 1L; i <= (sizeLimit + 1L); i++)
            {
                myCollectiion.Add(new CustomObject()
                {
                    Key = i.ToString(),
                    Value = Convert.ToInt32(i)
                });
            }

            // Act
            foreach (var item in myCollectiion)
                await sut.Set(item.Key, item);

            // Act and Assert
            var slimit = Convert.ToInt32(sizeLimit);
            var mostRecentCacheItem = sut.Get<CustomObject>(myCollectiion[slimit].Key);
            
            Assert.Throws<Exception>(() => sut.Get<CustomObject>(myCollectiion[0].Key));

            Assert.NotNull(mostRecentCacheItem);
            Assert.Matches(myCollectiion[slimit].Key, mostRecentCacheItem.Key);
            Assert.Equal(myCollectiion[slimit].Value, mostRecentCacheItem.Value);
        }
    }
}