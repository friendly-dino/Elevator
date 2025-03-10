using Elevator.App;

namespace Elevator.Tests
{
    [TestFixture]
    public class RequestDetailTests
    {
        [Test]
        public void RequestDetail_Initialize()
        {
            var request = new RequestDetail(1, 5, 2);

            Assert.Multiple(() =>
            {
                Assert.That(request.OriginFloor, Is.EqualTo(1));
                Assert.That(request.GotoFloor, Is.EqualTo(5));
                Assert.That(request.ElevatorID, Is.EqualTo(2));
            });
        }
        [Test]
        public void RequestDetail_Initialize_WithNullElevatorID()
        {
            var request = new RequestDetail(4, 2);

            Assert.Multiple(() =>
            {
                Assert.That(request.OriginFloor, Is.EqualTo(4));
                Assert.That(request.GotoFloor, Is.EqualTo(2));
            });
            Assert.That(request.ElevatorID, Is.Null);
        }
    }

}

