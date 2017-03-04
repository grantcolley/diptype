using System;
using System.Linq;
using DevelopmentInProgress.DipMapper.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.DipType.Test
{
    [TestClass]
    public class DynamicTypeHelperTest
    {
        [TestMethod]
        public void CreateInstance()
        {
            // Arrange

            // Act
            var activityHelper = DynamicTypeHelper.Get<Activity>();
            var activity = activityHelper.CreateInstance();

            // Assert
            Assert.IsInstanceOfType(activity, typeof(Activity));
            Assert.IsInstanceOfType(activityHelper, typeof(DynamicTypeHelper<Activity>));
        }

        [TestMethod]
        public void CreateChachedInstance()
        {
            // Arrange
            var activityHelper1 = DynamicTypeHelper.Get<Activity>();
            var activityHelper2 = DynamicTypeHelper.Get<Activity>();
            var genericActivity = DynamicTypeHelper.Get<GenericActivity<string>>();

            // Act
            var activity = activityHelper2.CreateInstance();

            activityHelper2.SetValue(activity, "Id", 100);
            activityHelper2.SetValue(activity, "Name", "Read");
            activityHelper2.SetValue(activity, "Level", 7.7);
            activityHelper2.SetValue(activity, "IsActive", true);
            activityHelper2.SetValue(activity, "Created", DateTime.Now);
            activityHelper2.SetValue(activity, "ActivityType", ActivityTypeEnum.Shared);

            var id = activityHelper2.GetValue(activity, "Id");
            var name = activityHelper2.GetValue(activity, "Name");
            var level = activityHelper2.GetValue(activity, "Level");
            var isActive = activityHelper2.GetValue(activity, "IsActive");
            var created = activityHelper2.GetValue(activity, "Created");
            var updated = activityHelper2.GetValue(activity, "Updated");
            var activityType = activityHelper2.GetValue(activity, "ActivityType");

            // Assert
            Assert.AreEqual(DynamicTypeHelper.cache.Count, 2);
            Assert.IsTrue(DynamicTypeHelper.cache.ContainsKey(typeof(Activity)));
            Assert.IsTrue(DynamicTypeHelper.cache.ContainsKey(typeof(GenericActivity<string>)));

            Assert.AreEqual(activityHelper1, activityHelper2);

            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
        }

        [TestMethod]
        public void DynamicTypeHelper_Get_OverloadedConstructor()
        {
            // Arrange
            var properties = PropertyHelper.GetPropertyInfos<Activity>();
            var activityHelper1 = DynamicTypeHelper.Get<Activity>(properties);
            var activityHelper2 = DynamicTypeHelper.Get<Activity>();

            // Act
            var activity = activityHelper2.CreateInstance();

            activityHelper2.SetValue(activity, "Id", 100);
            activityHelper2.SetValue(activity, "Name", "Read");
            activityHelper2.SetValue(activity, "Level", 7.7);
            activityHelper2.SetValue(activity, "IsActive", true);
            activityHelper2.SetValue(activity, "Created", DateTime.Now);
            activityHelper2.SetValue(activity, "ActivityType", ActivityTypeEnum.Shared);

            var id = activityHelper2.GetValue(activity, "Id");
            var name = activityHelper2.GetValue(activity, "Name");
            var level = activityHelper2.GetValue(activity, "Level");
            var isActive = activityHelper2.GetValue(activity, "IsActive");
            var created = activityHelper2.GetValue(activity, "Created");
            var updated = activityHelper2.GetValue(activity, "Updated");
            var activityType = activityHelper2.GetValue(activity, "ActivityType");

            // Assert
            Assert.IsTrue(DynamicTypeHelper.cache.ContainsKey(typeof(Activity)));

            Assert.AreEqual(activityHelper1, activityHelper2);

            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
        }

        [TestMethod]
        public void GetValue()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();

            var activity = activityHelper.CreateInstance();
            activity.Id = 100;
            activity.Name = "Read";
            activity.Level = 7.7;
            activity.IsActive = true;
            activity.Created = DateTime.Now;
            activity.ActivityType = ActivityTypeEnum.Public;

            // Act
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var activityType = activityHelper.GetValue(activity, "ActivityType");

            // Assert
            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
        }

        [TestMethod]
        public void GetValue_GenericType()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<GenericActivity<string>>();
            var activity = new GenericActivity<string>()
            {
                Id = 100,
                Name = "Read",
                Level = 7.7,
                IsActive = true,
                Created = DateTime.Now,
                ActivityType = ActivityTypeEnum.Public,
                GenericProperty = "Hello World"
            };

            // Act
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var activityType = activityHelper.GetValue(activity, "ActivityType");
            var genericProperty = activityHelper.GetValue(activity, "GenericProperty");

            // Assert
            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
            Assert.AreEqual(activity.GenericProperty, genericProperty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetValue_ExpectedException()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();
            var activity = new Activity()
            {
                Id = 100,
                Name = "Read",
                Level = 7.7,
                IsActive = true,
                Created = DateTime.Now,
                ActivityType = ActivityTypeEnum.Public,
            };

            // Act
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var test = activityHelper.GetValue(activity, "Test");
            var activityType = activityHelper.GetValue(activity, "ActivityType");

            // Assert
        }

        [TestMethod]
        public void SetValue()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();
            var activity = activityHelper.CreateInstance();

            var created = DateTime.Now;

            // Act
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);
            activityHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Public);

            // Assert
            Assert.AreEqual(activity.Id, 100);
            Assert.AreEqual(activity.Name, "Read");
            Assert.AreEqual(activity.Level, 7.7);
            Assert.AreEqual(activity.IsActive, true);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, null);
            Assert.AreEqual(activity.ActivityType, ActivityTypeEnum.Public);
        }

        [TestMethod]
        public void SetValue_GenericType()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<GenericActivity<string>>();
            var activity = new GenericActivity<string>();
            var created = DateTime.Now;

            // Act
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);
            activityHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Public);
            activityHelper.SetValue(activity, "GenericProperty", "Hello World");

            // Assert
            Assert.AreEqual(activity.Id, 100);
            Assert.AreEqual(activity.Name, "Read");
            Assert.AreEqual(activity.Level, 7.7);
            Assert.AreEqual(activity.IsActive, true);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, null);
            Assert.AreEqual(activity.ActivityType, ActivityTypeEnum.Public);
            Assert.AreEqual(activity.GenericProperty, "Hello World");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetValue_ExpectedException()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();
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

        [TestMethod]
        public void PropertyNames()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();

            // Act
            
            // Assert
            Assert.AreEqual(activityHelper.SupportedProperties.Count(), 7);
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "Id"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "Name"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "Level"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "IsActive"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "Created"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "Updated"));
            Assert.IsTrue(activityHelper.SupportedProperties.Any(p => p.Name == "ActivityType"));
        }

        [TestMethod]
        public void New()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();

            // Act
            var activity = activityHelper.CreateInstance();

            // Assert
            Assert.IsInstanceOfType(activity, typeof(Activity));
        }

        [TestMethod]
        public void New_GenericType()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<GenericActivity<string>>();

            // Act
            var activity = activityHelper.CreateInstance();

            // Assert
            Assert.IsInstanceOfType(activity, typeof(GenericActivity<string>));
        }

        [TestMethod]
        public void New_SetValues()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<Activity>();

            // Act
            var activity = activityHelper.CreateInstance();
            activityHelper.SetValue(activity, "Name", "Test");

            // Assert
            Assert.IsInstanceOfType(activity, typeof(Activity));
            Assert.AreEqual(activity.Name, "Test");
        }

        [TestMethod]
        public void New_GenericType_SetValues()
        {
            // Arrange
            var activityHelper = DynamicTypeHelper.Get<GenericActivity<string>>();

            // Act
            var activity = activityHelper.CreateInstance();
            activityHelper.SetValue(activity, "Name", "Test");
            activityHelper.SetValue(activity, "GenericProperty", "Hello World");

            // Assert
            Assert.IsInstanceOfType(activity, typeof(GenericActivity<string>));
            Assert.AreEqual(activity.Name, "Test");
            Assert.AreEqual(activity.GenericProperty, "Hello World");
        }
    }
}