using System.Collections.Concurrent;
using Elevator.App.Interface;
using Elevator.App.Utility;
using Elevator.App.Constants;
using Elevator.App.Exceptions;
using System.ComponentModel;

namespace Elevator.App.Controller
{
    public class ElevatorController : IElevatorController
    {
        #region Constructor
        public readonly List<IElevator> _elevators;
        private readonly IElevatorManager _elevatorManager;
        private readonly BlockingCollection<RequestDetail> _requests = [];
        private readonly int _maxFloors;
        private readonly object lockObj = new();
        public ElevatorController(int maxFloors, int numberOfElevators)
        {
            _maxFloors = maxFloors;
            _elevators = [];

            for (int i = 0; i < numberOfElevators; i++)
                _elevators.Add(new Elevator(i + 1));

            _elevatorManager = new ElevatorManager(_elevators); ;
            Thread controllerThread = new(ProcessRequests);
            controllerThread.Start();
        }
        #endregion
        [LogException]
        public void AddRequest(RequestDetail request)
        {
            if ( request.GotoFloor > _maxFloors || request.GotoFloor < 1)
                throw new InvalidRequestException(ElevatorConstants.InvalidReqMsg);

            lock (lockObj)
                _requests.Add(request);
        }

        [LogException]
        private void ProcessRequests()
        {
            foreach (var request in _requests.GetConsumingEnumerable())
            {
                    IElevator? bestElevator = GetBestElevatorForRequest(request);
                    if (bestElevator != null)
                        bestElevator.AddRequest(request);
                    else
                        throw new ElevatorNotFoundException(ElevatorConstants.NoElevatorMsg);  
            }
        }
        private IElevator? GetBestElevatorForRequest(RequestDetail request)
        {
            return request.ElevatorID.HasValue
                ? _elevators.FirstOrDefault(e => e.ElevatorID == request.ElevatorID.Value)
                : _elevatorManager.FindBestElevator(request.OriginFloor,request.DirectionRequest);
        }
    }

    }
