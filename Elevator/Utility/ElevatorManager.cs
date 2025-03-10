using Elevator.Enum;
using Elevator.App.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Utility
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly List<IElevator> _elevators;
        public ElevatorManager(List<IElevator> elevators) => _elevators = elevators;
        /// <summary>
        /// Gets the best possible elevator to be used based on certain parameters.
        /// </summary>
        /// <param name="requestedFloor">Number of the floor requested.</param>
        /// <returns></returns>
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
