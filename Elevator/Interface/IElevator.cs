using Elevator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.Interface
{
    public interface IElevator
    {
        int ElevatorID { get; }
        int CurrentFloor { get; }
        int NumberOfRequests {  get; }
        Direction CurrentDirection { get; }
        void AddRequest(RequestDetail request);
    }
}
