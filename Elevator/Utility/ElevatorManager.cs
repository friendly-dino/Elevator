using Elevator.Enum;
using Elevator.App.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public IElevator FindBestElevator(int requestedFloor)
        {
            IElevator bestElevator = null;
            int closestDistance = int.MaxValue;
            int fewestReqCount = int.MaxValue;
            try
            {
                foreach (var elevator in _elevators)
                {
                    int distance = Math.Abs(elevator.CurrentFloor - requestedFloor);
                    bool isGoingToReqFlr = IsGoingTo(elevator, requestedFloor);

                    // if perfect match is found,return immediately, no need for further checks
                    if (elevator.CurrentFloor == requestedFloor && elevator.CurrentDirection == Direction.Idle)
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
            catch (Exception)
            {
                throw;
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
            ElevatorLog.Info($"Elevator {ElevatorID} has reached and stopped at floor {CurrentFloor}.");
            Thread.Sleep(ElevatorConstants.PassengerDuration); // Simulating passengers entering/leaving 
        }
        /// <summary>
        /// Checks for the current direction of the elevator if it is going to the requested floor.
        /// </summary>
        private static bool IsGoingTo(IElevator elevator, int requestedFloor)
        {
            return elevator.CurrentDirection == Direction.GoUp && elevator.CurrentFloor < requestedFloor ||
                   elevator.CurrentDirection == Direction.GoDown && elevator.CurrentFloor > requestedFloor ||
                   elevator.CurrentDirection == Direction.Idle;
        }
    }
}
