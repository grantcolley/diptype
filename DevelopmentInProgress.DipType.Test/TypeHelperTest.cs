using System;
using DevelopmentInProgress.DipMapper.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.DipType.Test
{
    [TestClass]
    public class TypeHelperTest
    {
        [TestMethod]
        public void GetValue_Passes()
        {
            // Arrange
            var activityHelper = TypeHelper.Create<Activity>();
            var activity = new Activity()
            {
                Id = 100,
                Name = "Read",
                Level = 7.7,
                IsActive = true,
                Created = DateTime.Now
            };

            // Act
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");

            // Assert
            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level,level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetValue_ExpectedException()
        {
            // Arrange
            var activityHelper = TypeHelper.Create<Activity>();
            var activity = new Activity()
            {
                Id = 100,
                Name = "Read",
                Level = 7.7,
                IsActive = true,
                Created = DateTime.Now
            };

            // Act
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var test = activityHelper.GetValue(activity, "Test");

            // Assert
        }

        [TestMethod]
        public void SetValue_Passes()
        {
            // Arrange
            var activityHelper = TypeHelper.Create<Activity>();
            var activity = new Activity();
            var created = DateTime.Now;
            
            // Act
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);

            // Assert
            Assert.AreEqual(activity.Id, 100);
            Assert.AreEqual(activity.Name, "Read");
            Assert.AreEqual(activity.Level, 7.7);
            Assert.AreEqual(activity.IsActive, true);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetValue_ExpectedException()
        {
            // Arrange
            var activityHelper = TypeHelper.Create<Activity>();
            var activity = new Activity();
            var created = DateTime.Now;

            // Act
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);
            activityHelper.SetValue(activity, "Test", true);

            // Assert
        }
    }
}
