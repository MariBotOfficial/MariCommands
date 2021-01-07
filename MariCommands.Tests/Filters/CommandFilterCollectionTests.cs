using System;
using MariCommands.Filters;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class CommandFilterCollectionTests
    {
        [InlineData(typeof(DisposeFilter))]
        [Theory]
        public void Add_Should_Add_TypeCommandFilterFactory_Of_SpecifiedType(Type filterType)
        {
            // Act
            var collection = new CommandFilterCollection();
            var expectedFilter = new TypeCommandFilterFactory(filterType);

            // Arrange
            var filter = collection.Add(filterType, 0);

            // Assert
            Assert.NotNull(filter);
            Assert.NotEmpty(collection);
            Assert.Contains(filter, collection);
            Assert.IsType(expectedFilter.GetType(), filter);
        }
    }
}