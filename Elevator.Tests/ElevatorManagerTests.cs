using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Elevator.App.Interface;
using Elevator.App.Utility;
using Elevator.Enum;

namespace Elevator.Tests
{
    [TestFixture]
    public class ElevatorManagerTests
    {
        private List<IElevator> _elevators;
        private ElevatorManager _elevatorManager;

        [SetUp]
        public void Setup()
        {
            var elevatorMock1 = new Mock<IElevator>();
            elevatorMock1.Setup(e => e.CurrentFloor).Returns(1);
            elevatorMock1.Setup(e => e.CurrentDirection).Returns(Direction.Idle);
            elevatorMock1.Setup(e => e.NumberOfRequests).Returns(0);
            elevatorMock1.Setup(e => e.ElevatorID).Returns(1);

            var elevatorMock2 = new Mock<IElevator>();
            elevatorMock2.Setup(e => e.CurrentFloor).Returns(3);
            elevatorMock2.Setup(e => e.CurrentDirection).Returns(Direction.GoUp);
            elevatorMock2.Setup(e => e.NumberOfRequests).Returns(1);
            elevatorMock2.Setup(e => e.ElevatorID).Returns(2);

            var elevatorMock3 = new Mock<IElevator>();
            elevatorMock2.Setup(e => e.CurrentFloor).Returns(1);
            elevatorMock2.Setup(e => e.CurrentDirection).Returns(Direction.GoDown);
            elevatorMock2.Setup(e => e.NumberOfRequests).Returns(3);
            elevatorMock2.Setup(e => e.ElevatorID).Returns(3);

            _elevators = new List<IElevator> { elevatorMock1.Object, elevatorMock2.Object };
            _elevatorManager = new ElevatorManager(_elevators);
        }
        [Test]
        public void FindBestElevator_ShouldPickIdleElevatorOnRequestedFloor()
        {
            var bestElevator = _elevatorManager.FindBestElevator(1);
            Assert.That(bestElevator.ElevatorID, Is.EqualTo(1));
        }
        [Test]
        public void FindBestElevator_ShouldPickFewestRequest()
        {
            var bestElevator = _elevatorManager.FindBestElevator(2);
            Assert.That(bestElevator.ElevatorID, Is.EqualTo(1));
        }
        [Test]
        public void FindBestElevator_ClosestElevator()
        {
            var bestElevator = _elevatorManager.FindBestElevator(4);
            Assert.That(bestElevator.ElevatorID, Is.EqualTo(2));
        }
    }
}