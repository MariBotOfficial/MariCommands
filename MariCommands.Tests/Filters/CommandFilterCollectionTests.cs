using System;
using MariCommands.Filters;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class CommandFilterCollectionTests
    {
        [InlineData(typeof(DisposeFilter))]
        [Theory]
        public void Add_Should_Add_TypeCommandFilterFactory_Of_Specified_Type(Type filterType)
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

        [Fact]
        public void Add_Should_Throw_When_FilterType_Is_Not_A_Filter()
        {
            // Act
            var collection = new CommandFilterCollection();

            // Arrange + Assert
            Assert.Throws<ArgumentException>(() =>
            {
                collection.Add(typeof(string));
            });
        }

        [Fact]
        public void AddService_Should_Throw_When_FilterType_Is_Not_A_Filter()
        {
            // Act
            var collection = new CommandFilterCollection();

            // Arrange + Assert
            Assert.Throws<ArgumentException>(() =>
            {
                collection.AddService(typeof(string));
            });
        }

        [InlineData(typeof(DisposeFilter))]
        [Theory]
        public void AddService_Should_Add_ServiceCommandFilterFactory_Of_Specified_Type(Type filterType)
        {
            // Act
            var collection = new CommandFilterCollection();
            var expectedFilter = new ServiceCommandFilterFactory(filterType);

            // Arrange
            var filter = collection.AddService(filterType, 0);

            // Assert
            Assert.NotNull(filter);
            Assert.NotEmpty(collection);
            Assert.Contains(filter, collection);
            Assert.IsType(expectedFilter.GetType(), filter);
        }
    }
}