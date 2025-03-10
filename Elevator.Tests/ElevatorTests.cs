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
            // Set initial floor
            //_elevator.CurrentFloor = 5;
            var currentFloorField = typeof(ElevatorApp.Elevator).GetField("CurrentFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            currentFloorField.SetValue(_elevator, 1);
            var targetFloor = 5;

            // Directly invoke the MoveToFloor method
            var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            moveToFloorMethod.Invoke(_elevator, [targetFloor]);

            // Verify the elevator moved down
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(targetFloor));
            Assert.That(_elevator.CurrentDirection, Is.EqualTo(Direction.Idle));
        }

        //[Test]
        //public void MoveToFloor_ShouldNotMoveIfAlreadyAtTargetFloor()
        //{
        //    // Set initial floor
        //    _elevator.CurrentFloor = 3;
        //    var targetFloor = 3;

        //    // Directly invoke the MoveToFloor method
        //    var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
        //    moveToFloorMethod.Invoke(_elevator, new object[] { targetFloor });

        //    // Verify the elevator did not move
        //    Assert.AreEqual(targetFloor, _elevator.CurrentFloor);
        //    Assert.AreEqual(ElevatorApp.Direction.Idle, _elevator.CurrentDirection);
        //}

        //[Test]
        //public void MoveToFloor_ShouldHandleMultipleMoves()
        //{
        //    // Set initial floor
        //    _elevator.CurrentFloor = 1;
        //    var targetFloor1 = 5;
        //    var targetFloor2 = 2;

        //    // Directly invoke the MoveToFloor method to move up
        //    var moveToFloorMethod = typeof(ElevatorApp.Elevator).GetMethod("MoveToFloor", BindingFlags.NonPublic | BindingFlags.Instance);
        //    moveToFloorMethod.Invoke(_elevator, new object[] { targetFloor1 });

        //    // Verify the elevator moved up
        //    Assert.AreEqual(targetFloor1, _elevator.CurrentFloor);

        //    // Directly invoke the MoveToFloor method to move down
        //    moveToFloorMethod.Invoke(_elevator, new object[] { targetFloor2 });

        //    // Verify the elevator moved down
        //    Assert.AreEqual(targetFloor2, _elevator.CurrentFloor);
        //}
    }
}
