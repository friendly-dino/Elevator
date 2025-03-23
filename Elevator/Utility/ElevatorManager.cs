using Elevator.Enum;
using Elevator.App.Interface;
using Elevator.App.Exceptions;
using Elevator.App.Constants;

namespace Elevator.App.Utility
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly List<IElevator> _elevators;
        private readonly int ElevatorID;
        private readonly object lockObj = new();
        public int CurrentFloor { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public ElevatorManager(List<IElevator> elevators) => _elevators = elevators;
        public ElevatorManager(int elevatorId)
        {
            ElevatorID = elevatorId;
            CurrentFloor = 1; // Assuming the elevator starts on the 1st floor
            CurrentDirection = Direction.Idle;
        }
        public IElevator FindBestElevator(int requestedFloor, Direction requestedDirection)
        {
            IElevator bestElevator = null;
            int closestDistance = int.MaxValue;
            int fewestReqCount = int.MaxValue;
            try
            {
                foreach (var elevator in _elevators)
                {
                    int distance = Math.Abs(elevator.CurrentFloor - requestedFloor);
                    bool isGoingToReqFlr = IsGoingTo(elevator, requestedFloor, requestedDirection);

                    // if perfect match is found,return immediately, no need for further checks
                    if (elevator.CurrentFloor == requestedFloor && elevator.CurrentDirection == requestedDirection)
                        return elevator;

                    if (isGoingToReqFlr && (distance <= closestDistance && elevator.NumberOfRequests <= fewestReqCount))
                    {
                        closestDistance = distance;
                        fewestReqCount = elevator.NumberOfRequests;
                        bestElevator = elevator;
                    }
                    else if (bestElevator == null && distance <= closestDistance)//Use the closest elevator if above conditions arent met
                    {
                        closestDistance = distance;
                        bestElevator = elevator;
                    }
                }
                return bestElevator;
            }
            catch (Exception ex)
            {
                throw new ElevatorNotAvailableException(String.Format(ElevatorConstants.ElevetorUnavailable,ex.Message));
            }
            
        }
        public void MoveToFloor(int targetFloor)
        {
            string sDirection = string.Empty;
            while (CurrentFloor != targetFloor)
            {
                lock (lockObj)
                {
                    if (CurrentFloor < targetFloor)
                    {
                        CurrentFloor++;
                        CurrentDirection = Direction.GoUp;
                        sDirection = ElevatorConstants.DirectionUp;
                    }
                    else
                    {
                        CurrentFloor--;
                        CurrentDirection = Direction.GoDown;
                        sDirection = ElevatorConstants.DirectionDown;
                    }
                }

                ElevatorLog.Info($"Elevator {ElevatorID} {sDirection}: {CurrentFloor}F/{targetFloor}F.");
                Thread.Sleep(ElevatorConstants.MoveDuration); // Simulating elevator movement between floors 
            }
            Thread.Sleep(ElevatorConstants.PassengerDuration); // Simulating passengers entering/leaving 
        }
        public int GetInsertPosition(RequestDetail newRequest, Direction currentDirection, int currentFloor, List<RequestDetail> requests)
        {
            var insertPos = 0;
            bool isNewFloorInCurrentDirection = IsFloorInCurrentDirection(newRequest.GotoFloor, currentDirection, currentFloor);
            bool passedCurrentGroup = false;

            while (insertPos < requests.Count)
            {
                int currentFloorInList = requests[insertPos].GotoFloor;
                bool isExistingFloorInCurrentDirection = IsFloorInCurrentDirection(currentFloorInList, currentDirection, currentFloor);

                if (isNewFloorInCurrentDirection &&
                    HandleCurrentGroup(newRequest.GotoFloor, currentFloorInList, currentDirection, !isExistingFloorInCurrentDirection))
                    break;

                if (!isNewFloorInCurrentDirection &&
                    HandleOppositeGroup(currentFloorInList, isExistingFloorInCurrentDirection, ref passedCurrentGroup, currentDirection, currentFloor, newRequest.GotoFloor))
                    break;
                insertPos++;
            }
            return insertPos;
        }
        /// <summary>
        /// Checks for the current direction of the elevator if it is going to the requested floor.
        /// </summary>
        private static bool IsGoingTo(IElevator elevator, int requestedFloor,Direction requestedDirection)
        {
            return elevator.CurrentDirection == requestedDirection && elevator.CurrentFloor < requestedFloor ||
                   elevator.CurrentDirection == requestedDirection && elevator.CurrentFloor > requestedFloor ||
                   elevator.CurrentDirection == Direction.Idle;
        }
        private static bool IsFloorInCurrentDirection(int floor, Direction currentDirection, int currentFloor) =>
            currentDirection == Direction.GoUp ? floor >= currentFloor : floor <= currentFloor;
        private static bool HandleCurrentGroup(int newGotoFloor, int currentFloorInList, Direction currentDirection, bool isOppositeGroupFound)
        {
            if (isOppositeGroupFound) return true;

            if (currentDirection == Direction.GoUp && currentFloorInList > newGotoFloor) return true;
            if (currentDirection == Direction.GoDown && currentFloorInList < newGotoFloor) return true;

            return false;
        }
        private static bool HandleOppositeGroup(
            int currentFloorInList,
            bool isExistingFloorInCurrentDirection,
            ref bool passedCurrentGroup,
            Direction currentDirection,
            int currentFloor,
            int newGotoFloor)
        {
            if (!passedCurrentGroup && isExistingFloorInCurrentDirection)
                return false;

            passedCurrentGroup = true;

            if (currentDirection == Direction.GoUp && currentFloorInList < newGotoFloor) return true;
            if (currentDirection == Direction.GoDown && currentFloorInList > newGotoFloor) return true;

            return false;
        }
    }
}
