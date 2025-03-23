using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevator.Enum;

namespace Elevator.App.Interface
{
    public interface IElevatorManager
    {
        /// <summary>
        /// Gets the best possible elevator to be used based on certain parameters.
        /// </summary>
        /// <param name="requestedFloor">Number of the floor requested.</param>
        /// <returns>Elevator object containing the elevator ID.</returns>
        IElevator FindBestElevator(int requestedFloor,Direction requestedDirection);
        /// <summary>
        /// Efficiently sorts the floors array depending on the current floor to avoid redundant trips.
        /// </summary>
        /// <param name="inputFloors">The requested floor/s.</param>
        /// <param name="currentFloor">The floor which elevator is currently on.</param>
        /// <param name="direction">Current elevator direction.</param>
        /// <returns></returns>
        int[] SortFloors(string inputFloors, int currentFloor, Direction direction);
        /// <summary>
        /// Indicates whether the elevator would go up or down.
        /// </summary>
        /// <param name="targetFloor">Floor which the elevator would go.</param>
        void MoveToFloor(int targetFloor);
        /// <summary>
        /// Inserts a new elevator request into the existing list of requests in the correct order.
        /// </summary>
        /// <param name="newRequest">The new request to be inserted into the list.</param>
        /// <param name="currentDirection">The current direction of the elevator (Up or Down).</param>
        /// <param name="currentFloor">The current floor of the elevator.</param>
        /// <param name="requests">The list of existing requests to be updated.</param>
        /// <returns>The index number</returns>
        int GetInsertPosition(RequestDetail newRequest, Direction currentDirection, int currentFloor, List<RequestDetail> requests);
        int CurrentFloor { get; }
        Direction CurrentDirection { get; }
    }
}
