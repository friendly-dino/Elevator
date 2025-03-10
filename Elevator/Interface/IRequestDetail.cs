using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Interface
{
    public interface IRequestDetail
    {
        /// <summary>
        /// The floor from which the elevator came from.
        /// </summary>
        int OriginFloor { get; }
        /// <summary>
        /// The floor which the elevator would go to. ie destination.
        /// </summary>
        int GotoFloor { get; }
        /// <summary>
        /// Unique ID for each elevator car.
        /// </summary>
        public int? ElevatorID { get; }
    }
}
