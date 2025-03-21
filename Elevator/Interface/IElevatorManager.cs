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
        /// Indicates whether the elevator would go up or down.
        /// </summary>
        /// <param name="targetFloor">Floor which the elevator would go.</param>
        void MoveToFloor(int targetFloor);
        int CurrentFloor { get; }
        Direction CurrentDirection { get; }
    }
}
