using ElevatorApp = Elevator.App;
using Elevator.Enum;
using Elevator.App;
using System.Reflection;
using System.Collections.Concurrent;
using Elevator.App.Interface;

namespace Elevator.Tests
{
    [TestFixture]
    public class ElevatorTests
    {
        private ElevatorApp.Elevator _elevator;

        [SetUp]
        public void SetUp()
        {
            _elevator = new ElevatorApp.Elevator(1); 
        }

        [Test]
        public void AddRequest_ShouldAddRequest()
        {
            var request = new RequestDetail(1, 5);
            _elevator.AddRequest(request);

            // temporary turn off threading to pass
            Assert.That(_elevator.Requests.ToList(), Does.Contain(request));
        }
    }
}
