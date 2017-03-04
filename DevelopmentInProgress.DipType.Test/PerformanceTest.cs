using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DevelopmentInProgress.DipMapper.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.DipType.Test
{
    [TestClass]
    public class PerformanceTest
    {
        [TestMethod]
        public void Run()
        {
            // Arrange
            var typeHelper = TypeHelper.Get<Activity>();
            var dynamicTypeHelper = DynamicTypeHelper.Get<Activity>();
            var properties = PropertyHelper.GetPropertyInfos<Activity>();

            var swTypeHelper = new Stopwatch();
            var swDynamicTypeHelper = new Stopwatch();
            var swReflection = new Stopwatch();

            // Act
            swTypeHelper.Start();
            RunTypeHelper(typeHelper, 1);
            var typeHelperTime = swTypeHelper.Elapsed;


            swDynamicTypeHelper.Start();
            RunDynamicTypeHelper(dynamicTypeHelper, 1);
            var dynamicTypeHelperTime = swDynamicTypeHelper.Elapsed;

            swReflection.Start();
            RunReflection(properties, 1);
            var reflectionTime = swReflection.Elapsed;

            Debug.Print("1 x Activity");
            Debug.Print("TypeHelper         - {0}", typeHelperTime);
            Debug.Print("DynamicTypeHelper  - {0}", dynamicTypeHelperTime);
            Debug.Print("Reflection         - {0}", reflectionTime);

            Debug.Print("");
            Debug.Print("");

            swTypeHelper.Reset();
            swTypeHelper.Start();
            RunTypeHelper(typeHelper, 1000);
            typeHelperTime = swTypeHelper.Elapsed;

            swDynamicTypeHelper.Reset();
            swDynamicTypeHelper.Start();
            RunDynamicTypeHelper(dynamicTypeHelper, 1000);
            dynamicTypeHelperTime = swDynamicTypeHelper.Elapsed;

            swReflection.Reset();
            swReflection.Start();
            RunReflection(properties, 1000);
            reflectionTime = swReflection.Elapsed;

            Debug.Print("1000 x Activity");
            Debug.Print("TypeHelper         - {0}", typeHelperTime);
            Debug.Print("DynamicTypeHelper  - {0}", dynamicTypeHelperTime);
            Debug.Print("Reflection         - {0}", reflectionTime);

            Debug.Print("");
            Debug.Print("");

            swTypeHelper.Reset();
            swTypeHelper.Start();
            RunTypeHelper(typeHelper, 10000);
            typeHelperTime = swTypeHelper.Elapsed;

            swDynamicTypeHelper.Reset();
            swDynamicTypeHelper.Start();
            RunDynamicTypeHelper(dynamicTypeHelper, 10000);
            dynamicTypeHelperTime = swDynamicTypeHelper.Elapsed;

            swReflection.Reset();
            swReflection.Start();
            RunReflection(properties, 10000);
            reflectionTime = swReflection.Elapsed;

            Debug.Print("10000 x Activity");
            Debug.Print("TypeHelper         - {0}", typeHelperTime);
            Debug.Print("DynamicTypeHelper  - {0}", dynamicTypeHelperTime);
            Debug.Print("Reflection         - {0}", reflectionTime);

            Debug.Print("");
            Debug.Print("");

            swTypeHelper.Reset();
            swTypeHelper.Start();
            RunTypeHelper(typeHelper, 100000);
            typeHelperTime = swTypeHelper.Elapsed;

            swDynamicTypeHelper.Reset();
            swDynamicTypeHelper.Start();
            RunDynamicTypeHelper(dynamicTypeHelper, 100000);
            dynamicTypeHelperTime = swDynamicTypeHelper.Elapsed;

            swReflection.Reset();
            swReflection.Start();
            RunReflection(properties, 100000);
            reflectionTime = swReflection.Elapsed;

            Debug.Print("100000 x Activity");
            Debug.Print("TypeHelper         - {0}", typeHelperTime);
            Debug.Print("DynamicTypeHelper  - {0}", dynamicTypeHelperTime);
            Debug.Print("Reflection         - {0}", reflectionTime);

            // Assert
            Assert.IsTrue(reflectionTime > typeHelperTime);
            Assert.IsTrue(reflectionTime > dynamicTypeHelperTime);
        }

        private void RunTypeHelper(TypeHelper<Activity> typeHelper, int times)
        {
            for (int i = 0; i < times; i++)
            {
                var activity = typeHelper.CreateInstance();
                typeHelper.SetValue(activity, "Id", 100);
                typeHelper.SetValue(activity, "Name", "Read");
                typeHelper.SetValue(activity, "Level", 7.7);
                typeHelper.SetValue(activity, "IsActive", true);
                typeHelper.SetValue(activity, "Created", DateTime.Now);
                typeHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Shared);

                var id = typeHelper.GetValue(activity, "Id");
                var name = typeHelper.GetValue(activity, "Name");
                var level = typeHelper.GetValue(activity, "Level");
                var isActive = typeHelper.GetValue(activity, "IsActive");
                var created = typeHelper.GetValue(activity, "Created");
                var updated = typeHelper.GetValue(activity, "Updated");
                var activityType = typeHelper.GetValue(activity, "ActivityType");
            }
        }

        private void RunDynamicTypeHelper(DynamicTypeHelper<Activity> dynamicTypeHelper, int times)
        {
            for (int i = 0; i < times; i++)
            {
                var activity = dynamicTypeHelper.CreateInstance();
                dynamicTypeHelper.SetValue(activity, "Id", 100);
                dynamicTypeHelper.SetValue(activity, "Name", "Read");
                dynamicTypeHelper.SetValue(activity, "Level", 7.7);
                dynamicTypeHelper.SetValue(activity, "IsActive", true);
                dynamicTypeHelper.SetValue(activity, "Created", DateTime.Now);
                dynamicTypeHelper.SetValue(activity, "ActivityType", ActivityTypeEnum.Shared);

                var id = dynamicTypeHelper.GetValue(activity, "Id");
                var name = dynamicTypeHelper.GetValue(activity, "Name");
                var level = dynamicTypeHelper.GetValue(activity, "Level");
                var isActive = dynamicTypeHelper.GetValue(activity, "IsActive");
                var created = dynamicTypeHelper.GetValue(activity, "Created");
                var updated = dynamicTypeHelper.GetValue(activity, "Updated");
                var activityType = dynamicTypeHelper.GetValue(activity, "ActivityType");
            }
        }

        private void RunReflection(IEnumerable<PropertyInfo> properties, int times)
        {
            var idProperty = properties.First(p => p.Name.Equals("Id"));
            var nameProperty = properties.First(p => p.Name.Equals("Name"));
            var levelProperty = properties.First(p => p.Name.Equals("Level"));
            var isActiveProperty = properties.First(p => p.Name.Equals("IsActive"));
            var createdProperty = properties.First(p => p.Name.Equals("Created"));
            var updatedProperty = properties.First(p => p.Name.Equals("Updated"));
            var activityTypeProperty = properties.First(p => p.Name.Equals("ActivityType"));

            for (int i = 0; i < times; i++)
            {
                var activity = Activator.CreateInstance(typeof(Activity));

                idProperty.SetValue(activity, 100);
                nameProperty.SetValue(activity, "Read");
                levelProperty.SetValue(activity, 7.7);
                isActiveProperty.SetValue(activity, true);
                createdProperty.SetValue(activity, DateTime.Now);
                activityTypeProperty.SetValue(activity, ActivityTypeEnum.Shared);

                var id = idProperty.GetValue(activity);
                var name = nameProperty.GetValue(activity);
                var level = levelProperty.GetValue(activity);
                var isActive = isActiveProperty.GetValue(activity);
                var created = createdProperty.GetValue(activity);
                var updated = updatedProperty.GetValue(activity);
                var activityType = activityTypeProperty.GetValue(activity);
            }
        }
    }
}