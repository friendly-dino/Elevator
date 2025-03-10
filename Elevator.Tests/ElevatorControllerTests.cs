using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Elevator.App.Controller;
using Elevator.App;

namespace Elevator.Tests
{
    public class ElevatorControllerTests
    {
        private ElevatorController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new ElevatorController(10, 4);
        }

        [Test]
        public void AddRequest_ValidRequest_Test()
        {
            var request = new RequestDetail(1, 5);

            _controller.AddRequest(request);

            // Assuming there's a way to verify the request was added, e.g., a public method or property
            // Assert.IsTrue(_controller.Requests.Contains(request));
        }

        [Test]
        public void AddRequest_InvalidRequest_Test()
        {
            var request = new RequestDetail(11, 5);

            _controller.AddRequest(request);

            // Assuming there's a way to verify the request was not added
            // Assert.IsFalse(_controller.Requests.Contains(request));
        }
    }
}
