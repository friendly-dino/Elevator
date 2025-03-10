

namespace Elevator.App.Interface
{
    public interface IElevatorController
    {
        /// <summary>
        /// Validates the request and adds it to a queue for processing.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="InvalidRequestException"></exception>
        void AddRequest(RequestDetail request);
    }
}
