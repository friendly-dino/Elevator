using ElevatorApp = Elevator.App;
using Elevator.Enum;
using Elevator.App;
using System.Reflection;
using System.Collections.Concurrent;

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

        [Test]
        public void MoveToFloor_ShouldUpdateCurrentFloor()
        {
            var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            moveToFloorMethod.Invoke(_elevator, [5]);

            Assert.That(_elevator.CurrentFloor, Is.EqualTo(5));
        }

        [Test]
        public void MoveToFloor_ShouldMoveUp()
        {
            var currentFloor = typeof(ElevatorApp.Elevator).GetProperty("CurrentFloor", BindingFlags.Public | BindingFlags.Instance);
            currentFloor.SetValue(_elevator, 1);
            var targetFloor = 5;

            // Directly invoke the MoveToFloor method
            var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            moveToFloorMethod.Invoke(_elevator, [targetFloor]);

            Assert.Multiple(() =>
            {
                Assert.That(_elevator.CurrentFloor, Is.EqualTo(targetFloor));
                Assert.That(_elevator.CurrentDirection, Is.EqualTo(Direction.GoUp));
            });
        }

        [Test]
        public void MoveToFloor_ShouldMoveDown()
        {
            var currentFloor = typeof(ElevatorApp.Elevator).GetProperty("CurrentFloor", BindingFlags.Public | BindingFlags.Instance);
            currentFloor.SetValue(_elevator, 10);
            var targetFloor = 7;

            // Directly invoke the MoveToFloor method
            var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            moveToFloorMethod.Invoke(_elevator, [targetFloor]);

            Assert.Multiple(() =>
            {
                Assert.That(_elevator.CurrentFloor, Is.EqualTo(targetFloor));
                Assert.That(_elevator.CurrentDirection, Is.EqualTo(Direction.GoDown));
            });
        }

        [Test]
        public void MoveToFloor_ShouldNotMoveIfAlreadyAtTargetFloor()
        {
            // Set initial floor
            var currentFloor = typeof(ElevatorApp.Elevator).GetProperty("CurrentFloor", BindingFlags.Public | BindingFlags.Instance);
            currentFloor.SetValue(_elevator, 3);
            var targetFloor = 3;

            // Directly invoke the MoveToFloor method
            var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            moveToFloorMethod.Invoke(_elevator, new object[] { targetFloor });

            // Verify the elevator did not move
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(targetFloor));
            Assert.That(_elevator.CurrentDirection, Is.EqualTo(Direction.Idle));
            //Assert.AreEqual(ElevatorApp.Direction.Idle, _elevator.CurrentDirection);
        }
    }
}
