﻿using Elevator.Enum;
using Elevator.App.Interface;
using System.Collections.Concurrent;
using Elevator.App.Constants;
using Elevator.App.Utility;
using Elevator.App.Exceptions;

namespace Elevator.App
{
    public class Elevator : IElevator
    {
        #region Constructor
        public int ElevatorID { get; }
        public int CurrentFloor { get; private set; } = 1;
        public int NumberOfRequests => requests.Count;
        public Direction CurrentDirection { get; private set; } = Direction.Idle;
        public IEnumerable<RequestDetail> Requests => requests; //for tests
        private readonly IElevatorManager _elevatorManager;
        private readonly BlockingCollection<RequestDetail> requests = [];
        private readonly object lockObj = new();
        public Elevator(int id, bool startThread = true)
        {
            ElevatorID = id;
            if (startThread)
            {
                Thread elevatorThread = new(ProcessRequests);
                elevatorThread.Start();
            }
            _elevatorManager = new ElevatorManager(ElevatorID);
        }
        #endregion
        public void AddRequest(RequestDetail request)
        {
            lock (lockObj)
                requests.Add(request);
            ElevatorLog.Info($"Request assigned to Elevator {ElevatorID}: From {request.OriginFloor}F -> Going to {request.GotoFloor}F");
        }
        [LogException]
        private void ProcessRequests()
        {
            //notes: can add batch processing for advanced handling of requests ie same directions etc
            RequestDetail lastRequest = null;
            foreach (var request in requests.GetConsumingEnumerable())
            {
                try
                {
                    bool sameElevID = false;
                    if (request.ElevatorID.HasValue && lastRequest != null) //check value to avoid error
                        sameElevID = lastRequest.ElevatorID == request.ElevatorID;

                    if (lastRequest != null && sameElevID)//if request came from the same elevator, continue going to destination floor
                        _elevatorManager.MoveToFloor(request.GotoFloor);
                    else//this the default behavior
                    {
                        _elevatorManager.MoveToFloor(request.OriginFloor);
                        _elevatorManager.MoveToFloor(request.GotoFloor);
                    }
                    lock (lockObj)
                        CurrentDirection = Direction.Idle;

                    lastRequest = request;
                    ElevatorLog.Info(String.Format(ElevatorConstants.RequestComplete,ElevatorID, request.OriginFloor, request.GotoFloor));
                }
                catch (Exception)
                {
                    throw new ElevatorProcessRequestException(string.Format(ElevatorConstants.ProcessRequestError, ElevatorID));
                }
            }
        }
    }
}

#region for reference
// Move to the ground floor if idle and no pending requests
//while (CurrentDirection == Direction.Idle && CurrentFloor != 0 && requests.Count == 0)
//{
//    Console.WriteLine($"Elevator {ElevatorID} is idle, moving to the ground floor.");
//    if (CurrentFloor > 0)
//    {
//        CurrentFloor--;
//        CurrentDirection = Direction.GoDown;
//    }
//    Console.WriteLine($"Elevator {ElevatorID} reached floor {CurrentFloor}");
//    Thread.Sleep(1000); // Simulating elevator movement

//    // Check for new requests during the movement to the ground floor
//    if (requests.Count > 0)
//    {
//        break; // Exit the loop to process the new request
//    }
//}

//if (CurrentFloor == 0)
//{
//    CurrentDirection = Direction.Idle;
//    Console.WriteLine($"Elevator {ElevatorID} is now at the ground floor.");
//}
#endregion
