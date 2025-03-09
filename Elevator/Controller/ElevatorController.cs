using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevator.Interface;
using Elevator.Utility;

namespace Elevator.Controller
{
    public class ElevatorController : IElevatorController
    {
        private readonly List<IElevator> _elevators;
        private readonly ElevatorManager _elevatorManager;
        private readonly BlockingCollection<RequestDetail> _requests = [];
        private readonly int _maxFloors;
        private readonly object lockObj = new();
        public ElevatorController(int maxFloors, int numberOfElevators)
        {
            _maxFloors = maxFloors;
            _elevators = new List<IElevator>();

            for (int i = 0; i < numberOfElevators; i++)
                _elevators.Add(new Elevator(i + 1));

            _elevatorManager = new ElevatorManager(_elevators);
            Thread controllerThread = new(ProcessRequests);
            controllerThread.Start();
        }

        public void AddRequest(RequestDetail request)
        {
            if (request.OriginFloor > _maxFloors || request.GotoFloor > _maxFloors || request.OriginFloor < 1 || request.GotoFloor < 1)
            {
                Console.WriteLine("Request exceeds the maximum number of floors or is invalid.");
                return;
            }
            lock (lockObj)
                _requests.Add(request);

           // Console.WriteLine($"Request added: From floor: {request.OriginFloor} -> Going to floor: {request.GotoFloor}");
        }

        private void ProcessRequests()
        {
            foreach (var request in _requests.GetConsumingEnumerable())
            {
                IElevator? bestElevator = GetBestElevatorForRequest(request);
                if (bestElevator != null)
                    bestElevator.AddRequest(request);
                else
                    Console.WriteLine("No suitable elevator found for the request.");
            }
        }
        private IElevator? GetBestElevatorForRequest(RequestDetail request)
        {
            return request.ElevatorID.HasValue
                ? _elevators.FirstOrDefault(e => e.ElevatorID == request.ElevatorID.Value)
                : _elevatorManager.FindBestElevator(request.OriginFloor);
        }
    }

    }
