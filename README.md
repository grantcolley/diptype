# diptype

[![Build status](https://ci.appveyor.com/api/projects/status/ivlk4a45y2kutj67?svg=true)](https://ci.appveyor.com/project/grantcolley/diptype)

Dynamically created type helpers that using intermediate language to create new instances of a type and get and set properties.

####Table of Contents
* [Example Usage](#example-usage)
* [TypeHelper\<T>](#typehelper)
* [DynamicTypeHelper\<T>](#dynamictypehelper)
* [Performance Tests](#performance-tests)

## Example Usage
The following example shows a method reading data and mapping it to fields of a type unknown at design time. The full code for this example can be seen at [dipmapper](https://github.com/grantcolley/dipmapper).
```C#
            public virtual T ReadData<T>(IDataReader reader) where T : class, new()
            {
                var typeHelper = DynamicTypeHelper.Get<T>();
                var t = typeHelper.CreateInstance();

                foreach (var propertyInfo in typeHelper.SupportedProperties)
                {
                    var value = reader[propertyInfo.Name];
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }

                    typeHelper.SetValue(t, propertyInfo.Name, value);                    
                }

                return t;
            }
```

## TypeHelper\<T>
Dynamically creates and caches a type helper class with methods for creating new instances of a type and gets and sets its properties.
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

## DynamicTypeHelper\<T>
A generic type helper class that creates and caches dynamic methods at runtime for creating new instances of a type and gets and sets its properties.
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

## Performance Tests
The following shows the results of a simple test where an instance of the Activity class is created and it's properties set and read *n* number of times. The test can be found in PerformanceTest.cs.

```C#
// The time it takes to create an instance of the
// TypeHelper, DynamicTypeHelper or, in the case of 
// reflection, a list of PropertyInfo's for the same object. 
// NOTE: caching is used when creating a TypeHelper or DynamicTypeHelper
// so this is a one off performance hit.
TypeHelper         - 00:00:00.0050471
DynamicTypeHelper  - 00:00:00.0038052
Reflection         - 00:00:00.0000226


1 x Activity
TypeHelper         - 00:00:00.0027211
DynamicTypeHelper  - 00:00:00.0019163
Reflection         - 00:00:00.0012029


1000 x Activity
TypeHelper         - 00:00:00.0017709
DynamicTypeHelper  - 00:00:00.0029563
Reflection         - 00:00:00.0053687


10000 x Activity
TypeHelper         - 00:00:00.0172054
DynamicTypeHelper  - 00:00:00.0367415
Reflection         - 00:00:00.0601286


100000 x Activity
TypeHelper         - 00:00:00.1596597
DynamicTypeHelper  - 00:00:00.3159577
Reflection         - 00:00:00.5803338
```
