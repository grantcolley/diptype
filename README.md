# diptype

*Faster than reflection!* dynamically created type helpers that using intermediate language to create new instances of a type and get and set properties faster than reflection.

####Table of Contents
* [TypeHelper<T>](#typehelper)
* [DynamicTypeHelper<T>](#dynamictypehelper)

## TypeHelper
Dynamically creates and caches a type helper class with methods for creating new instances of a type and gets and sets its properties faster than reflection.
```C#
            // Create the type helper
            var activityHelper = TypeHelper.Get<Activity>();
            
            // The type helper can create instances of the type...
            var activity = activityHelper.CreateInstance();           
            
            // The type helper can set property values on the object.
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);
            activityHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Public);
            
            // The type helper can get property values from the object.
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var activityType = activityHelper.GetValue(activity, "ActivityType");

            Assert.IsInstanceOfType(activity, typeof(Activity));
            Assert.IsInstanceOfType(activityHelper, typeof(TypeHelper<Activity>));
            
            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
```

## DynamicTypeHelper
A generic type helper class that creates and caches dynamic methods at runtime for creating new instances of a type and get and set its properties.
```C#
            // Create the dynamic type helper
            var activityHelper = DynamicTypeHelper.Get<Activity>();
            
            // The type helper can create instances of the type...
            var activity = activityHelper.CreateInstance();           
            
            // The type helper can set property values on the object.
            activityHelper.SetValue(activity, "Id", 100);
            activityHelper.SetValue(activity, "Name", "Read");
            activityHelper.SetValue(activity, "Level", 7.7);
            activityHelper.SetValue(activity, "IsActive", true);
            activityHelper.SetValue(activity, "Created", created);
            activityHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Public);
            
            // The type helper can get property values from the object.
            var id = activityHelper.GetValue(activity, "Id");
            var name = activityHelper.GetValue(activity, "Name");
            var level = activityHelper.GetValue(activity, "Level");
            var isActive = activityHelper.GetValue(activity, "IsActive");
            var created = activityHelper.GetValue(activity, "Created");
            var updated = activityHelper.GetValue(activity, "Updated");
            var activityType = activityHelper.GetValue(activity, "ActivityType");

            Assert.IsInstanceOfType(activity, typeof(Activity));
            Assert.IsInstanceOfType(activityHelper, typeof(DynamicTypeHelper<Activity>));
            
            Assert.AreEqual(activity.Id, id);
            Assert.AreEqual(activity.Name, name);
            Assert.AreEqual(activity.Level, level);
            Assert.AreEqual(activity.IsActive, isActive);
            Assert.AreEqual(activity.Created, created);
            Assert.AreEqual(activity.Updated, updated);
            Assert.AreEqual(activity.ActivityType, activityType);
```
