
using Elevator.Enum;

namespace Elevator.App.Utility
{
    public class RequestComparer : IComparer<RequestDetail>
    {
        private readonly Direction _currentDirection;

        public RequestComparer(Direction currentDirection = Direction.Idle)
        {
            _currentDirection = currentDirection;
        }
        public int Compare(RequestDetail? x, RequestDetail? y)
        {
            if (x == null || y == null) return 0;

            if (_currentDirection == Direction.GoDown)
                return y.GotoFloor.CompareTo(x.GotoFloor); // Desc
            else if (_currentDirection == Direction.GoUp || _currentDirection == Direction.Idle)//up and default sort
                return x.GotoFloor.CompareTo(y.GotoFloor); // Asc

            //Secondary sort by DirectionRequest
            return x.DirectionRequest.CompareTo(y.DirectionRequest);
        }
    }
}


//with elevator ID
//public int Compare(RequestDetail? x, RequestDetail? y)
//{
//    if (x == null || y == null) return 0;

//    // Primary sort: GotoFloor, based on current direction
//    int floorComparison;
//    if (_currentDirection == Direction.GoUp)
//        floorComparison = x.GotoFloor.CompareTo(y.GotoFloor); // Ascending
//    else if (_currentDirection == Direction.GoDown)
//        floorComparison = y.GotoFloor.CompareTo(x.GotoFloor); // Descending
//    else
//        floorComparison = x.GotoFloor.CompareTo(y.GotoFloor); // Default

//    if (floorComparison != 0) return floorComparison;

//    // Secondary sort: DirectionRequest
//    int directionComparison = x.DirectionRequest.CompareTo(y.DirectionRequest);
//    if (directionComparison != 0) return directionComparison;

//    // Tertiary sort: ElevatorID
//    return (x.ElevatorID ?? 0).CompareTo(y.ElevatorID ?? 0); // Use nullable ElevatorID safely
//} 
