using Elevator.Enum;

namespace Elevator.App.Interface
{
    public interface IRequestDetail
    {
        /// <summary>
        /// The floor from which the elevator came from.
        /// </summary>
        int OriginFloor { get; set; }
        /// <summary>
        /// The floor which the elevator would go to. ie destination.
        /// </summary>
        int GotoFloor { get; }
        /// <summary>
        /// Unique ID for each elevator car.
        /// </summary>
        public int? ElevatorID { get; }
        /// <summary>
        /// The elevator direction to be requested.
        /// </summary>
        Direction DirectionRequest { get; }

    }
}
