using DevelopmentInProgress.DipMapper.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.DipType.Test
{
    [TestClass]
    public class TypeHelperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var activity = new Activity();
            var typeHelper = TypeHelper.Create<Activity>();

            // Act
            var property1 = typeHelper.GetValue(activity, "Id");
            var property2 = typeHelper.GetValue(activity, "Name");
            var property3 = typeHelper.GetValue(activity, "Level");
            var property4 = typeHelper.GetValue(activity, "IsActive");
            var property5 = typeHelper.GetValue(activity, "Created");
            var property6 = typeHelper.GetValue(activity, "Updated");
            var property7 = typeHelper.GetValue(activity, "Test");

            // Assert
        }
    }
}
