using Elevator.App.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App
{
    /// <summary>
    /// Represents a request for an elevator to move from one floor to another.
    /// </summary>
    /// <param name="originFloor">The source floor where the elevator came from.</param>
    /// <param name="gotoFloor">The destination floor which the elevator would go to.</param>
    /// /// <param name="elevID">Specify which elevator to use.</param>
    public class RequestDetail(int originFloor, int gotoFloor, int? elevID=null) : IRequestDetail
    {
        public int OriginFloor { get; } = originFloor;
        public int GotoFloor { get; } = gotoFloor;
        public int? ElevatorID { get; } = elevID;
    }
}
