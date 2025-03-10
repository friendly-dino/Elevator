using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Elevator.App.Controller;
using Elevator.App;
using Elevator.App.Interface;
using Elevator.App.Utility;
using Elevator.App.Exceptions;

namespace Elevator.Tests
{
    public class ElevatorControllerTests
    {
        private ElevatorController _controller;
        private Mock<IElevatorManager> _mockElevatorManager;

        [SetUp]
        public void Setup()
        {
            int maxFloors = 10;
            int numberOfElevators = 4;

            _mockElevatorManager = new Mock<IElevatorManager>();

            _controller = new ElevatorController(maxFloors, numberOfElevators);
        }

        [Test]
        public void AddRequest_ValidRequest_DoesNotThrow()
        {
            var request = new RequestDetail(3, 10);
            Assert.DoesNotThrow(() => _controller.AddRequest(request));
        }

        [Test]
        public void AddRequest_InvalidInput_ThrowsException()
        {
            var request = new RequestDetail(15, 6); // Exceeds max floors and elevators
            Assert.Throws<InvalidRequestException>(() => _controller.AddRequest(request));
        }

        [Test]
        public void GetBestElevator_ValidRequest_ReturnsMockedElevator()
        {
            var mockElevator = new Mock<IElevator>();
            mockElevator.Setup(e => e.ElevatorID).Returns(1);
            mockElevator.Setup(e => e.CurrentFloor).Returns(5);

            _mockElevatorManager
                .Setup(m => m.FindBestElevator(It.IsAny<int>()))
                .Returns(mockElevator.Object);

            var bestElevator = _mockElevatorManager.Object.FindBestElevator(3);

            Assert.IsNotNull(bestElevator);
            Assert.That(bestElevator.ElevatorID, Is.EqualTo(1));
        }
    }
}
