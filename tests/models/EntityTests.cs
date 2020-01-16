using System;
using Xunit;
using LobbyEngine;
using LobbyEngine.Models;

namespace LobbyEngine.Tests
{
    public class EntityTests
    {
        class TestableEntity: Entity { }
        class DifferentEntity: Entity { }

        [Fact]
        public void NewEntityHasId()
        {
            var entity = new TestableEntity();
            Assert.False(string.IsNullOrEmpty(entity.Id));
        }

        [Fact]
        public void EntitiesWithSameTypeAndSameIdAreTheSame()
        {
            var firstEntity = new TestableEntity();
            var secondEntity = new TestableEntity();

            secondEntity.Id = firstEntity.Id;

            Assert.True(firstEntity.Equals(secondEntity));
        }

        [Fact]
        public void EntitiesWithSameTypeAndDifferentIdAreDifferent()
        {
            var firstEntity = new TestableEntity();
            var secondEntity = new TestableEntity();

            Assert.False(firstEntity.Equals(secondEntity));
        }

        [Fact]
        public void EntitiesWithDifferentTypeAndSameIdAreDifferent()
        {
            var firstEntity = new TestableEntity();
            var secondEntity = new DifferentEntity();

            secondEntity.Id = firstEntity.Id;

            Assert.False(firstEntity.Equals(secondEntity));
        }

        [Fact]
        public void EntitiesWithDifferentTypeAndDifferentIdAreDifferent()
        {
            var firstEntity = new TestableEntity();
            var secondEntity = new DifferentEntity();

            Assert.False(firstEntity.Equals(secondEntity));
        }
    }
}
