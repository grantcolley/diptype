using System;
using DevelopmentInProgress.DipMapper.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.DipType.Test
{
    [TestClass]
    public class PropertyHelperTest
    {
        [TestMethod]
        public void SupportedProperties()
        {
            // Arrange
            var propertyInfos = PropertyHelper.GetPropertyInfos<Activity>();
            var supportedPropertyNames = String.Empty;
            foreach (var propertyInfo in propertyInfos)
            {
                supportedPropertyNames += propertyInfo.Name + ",";
            }

            supportedPropertyNames = supportedPropertyNames.Remove(supportedPropertyNames.Length - 1, 1);

            var activityHelper = TypeHelper.Get<Activity>();

            // Act
            var supportedProperties = activityHelper.SupportedProperties;

            // Assert
            Assert.AreEqual(supportedProperties, supportedPropertyNames);
        }
    }
}
