using Elevator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Interface
{
    public interface IElevator
    {
        int ElevatorID { get; }
        int CurrentFloor { get; }
        int NumberOfRequests {  get; }
        Direction CurrentDirection { get; }
        /// <summary>
        /// Adds the request to the elevator's internal blocking collection (requests) and logs the elevator assignment.
        /// </summary>
        /// <param name="request"></param>
        void AddRequest(RequestDetail request);
    }
}
