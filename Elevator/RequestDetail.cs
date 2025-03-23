using Elevator.App.Interface;
using Elevator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App
{
    
    public class RequestDetail : IRequestDetail
    {
        public int OriginFloor { get; set; }
        public int GotoFloor { get; }
        public int? ElevatorID { get; set; }
        public Direction DirectionRequest { get; }

        /// <summary>
        /// Represents a request for an elevator to move from one floor to another.
        /// </summary>
        /// <param name="originFloor">The source floor where the elevator came from.</param>
        /// <param name="gotoFloor">The destination floor which the elevator would go to.</param>
        /// <param name="elevID">Specify which elevator to use.</param>
        public RequestDetail(int originFloor, int gotoFloor, int? elevID = null)
        {
            OriginFloor = originFloor;
            GotoFloor = gotoFloor;
            ElevatorID = elevID;
        }
        /// <summary>
        /// Represents a request for an elevator to move from one floor to another.
        /// </summary>
        /// <param name="requestedFloor">This is both the origin and destination floor</param>
        /// <param name="direction">The requested direction upon user request.</param>
        /// <param name="elevID">Specify which elevator to use.</param>
        public RequestDetail(int requestedFloor, Direction direction, int? elevID = null)
        {
            GotoFloor = requestedFloor;
            DirectionRequest = direction;
            ElevatorID = elevID;
        }
    }
    
}
