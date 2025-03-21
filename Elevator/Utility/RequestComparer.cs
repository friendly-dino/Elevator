
namespace Elevator.App.Utility
{
    public class RequestComparer : IComparer<RequestDetail>
    {
        public int Compare(RequestDetail? x, RequestDetail? y)
        {
            if (x == null || y == null) return 0;

            // Primary sort: GotoFloor
            int floorComparison = x.GotoFloor.CompareTo(y.GotoFloor);
            if (floorComparison != 0) return floorComparison;
            // Secondary sort: Direction
            return x.DirectionRequest.CompareTo(y.DirectionRequest);
        }
    }

}
