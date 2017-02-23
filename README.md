# diptype
Type helper for dynamically creating new instances of classes, getting and setting properties.

### CreateInstance()
```C#
            var activityHelper = TypeHelper.CreateInstance<Activity>();
            var activity = activityHelper.CreateInstance();

            Assert.IsInstanceOfType(activity, typeof(Activity));
            Assert.IsInstanceOfType(activityHelper, typeof(TypeHelper<Activity>));
```

### GetValue(object, string) 
```C#
            // Arrange
            var activityHelper = TypeHelper.CreateInstance<Activity>();

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
```

### SetValue(object, string, object) 
```C#
            // Arrange
            var activityHelper = TypeHelper.CreateInstance<Activity>();
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
```
