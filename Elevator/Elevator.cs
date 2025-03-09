using Elevator.Enum;
using Elevator.App.Interface;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Elevator.App
{
    public class Elevator : IElevator
    {
        public int ElevatorID { get; }
        public int CurrentFloor { get; private set; } = 1;
        public int NumberOfRequests => requests.Count;
        public Direction CurrentDirection { get; private set; } = Direction.Idle;
        private readonly BlockingCollection<RequestDetail> requests = [];
        private readonly object lockObj = new();
        public Elevator(int id)
        {
            ElevatorID = id;
            Thread elevatorThread = new(ProcessRequests);
            elevatorThread.Start();
        }
        public void AddRequest(RequestDetail request)
        {
            lock (lockObj)
                requests.Add(request);

            Console.WriteLine($"Request assigned to Elevator {ElevatorID}: From {request.OriginFloor}F -> Going to {request.GotoFloor}F");
        }
        private void ProcessRequests()
        {
            RequestDetail lastRequest = null;
            foreach (var request in requests.GetConsumingEnumerable())
            {
                try
                {
                    bool sameElevID = false;
                    if (request.ElevatorID.HasValue && lastRequest != null) //check value to avoid error
                        sameElevID = lastRequest.ElevatorID == request.ElevatorID;

                    if (lastRequest != null && sameElevID)
                    {
                        MoveToFloor(request.GotoFloor);
                    }
                    else//this the default behavior
                    {
                        MoveToFloor(request.OriginFloor);
                        MoveToFloor(request.GotoFloor);
                    }
                    lock (lockObj)
                    {
                        CurrentDirection = Direction.Idle;
                    }
                    lastRequest = request;
                    Console.WriteLine($"Elevator {ElevatorID} completed request: Source {request.OriginFloor} -> Destination {request.GotoFloor}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Elevator {ElevatorID} encountered an error: {ex.Message}");
                }
            }
        }
        private void MoveToFloor(int targetFloor, int originFloor =0)
        {
            while (CurrentFloor != targetFloor)
            {
                lock (lockObj)
                {
                    if (CurrentFloor < targetFloor)
                    {
                        CurrentFloor++;
                        CurrentDirection = Direction.GoUp;
                    }
                    else
                    {
                        CurrentFloor--;
                        CurrentDirection = Direction.GoDown;
                    }
                }
                string direction = CurrentDirection.Equals(Direction.GoUp) ? "is going UP" : "is going DOWN";
                Console.WriteLine($"Elevator {ElevatorID} {direction}: {CurrentFloor}F/{targetFloor}F.");
                Thread.Sleep(500); // Simulating elevator movement between floors
            }
            Console.WriteLine($"Elevator {ElevatorID} has reached and stopped at floor {CurrentFloor}.");
            //Thread.Sleep(10000); // Simulating passengers entering/leaving
        }

    }
}


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
