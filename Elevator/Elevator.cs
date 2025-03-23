using Elevator.Enum;
using Elevator.App.Interface;
using System.Collections.Concurrent;
using Elevator.App.Constants;
using Elevator.App.Utility;
using Elevator.App.Exceptions;
using System.Threading.Channels;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Elevator.App
{
    public class Elevator : IElevator
    {
        #region Constructor
        public int CurrentDestination { get; private set; } = 1;
        public int ElevatorID { get; }
        public int CurrentFloor { get; private set; } = 1;
        public int NumberOfRequests => _elevatorRequests.Count;
        public Direction CurrentDirection { get;  private set; } = Direction.Idle;
        public IEnumerable<RequestDetail> Requests => _elevatorRequests; //for tests
        private readonly IElevatorManager _elevatorManager;
        private readonly object lockObj = new();
        private static readonly object InputLock = new();
        private readonly Task _elevatorTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private List<RequestDetail> _elevatorRequests = [];
        public Elevator(int id, bool startTask = true)
        {
            ElevatorID = id;
            if (startTask)
                _elevatorTask = ProcessRequestsAsync(_cancellationTokenSource.Token);
            _elevatorManager = new ElevatorManager(ElevatorID);
        }
        #endregion
        public void AddRequest(RequestDetail request)//note: check if the sorting logic is better to place here
        {
            lock (lockObj)
                _elevatorRequests.Add(request);
            Console.SetWindowSize(80, 60);
            ElevatorLog.Info($"Request assigned to Elevator {ElevatorID}: From {CurrentDestination}F -> Going to {request.GotoFloor}F");
        }
        public async Task WaitForCompletion()
        {
            try
            {
                if (_elevatorTask != null)
                    await _elevatorTask; // Await the task to complete
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Elevator {ElevatorID} task was canceled.");
            }
        }
        private void InsertRequestInOrder(List<RequestDetail> requests, string inputFloors, int currentFloor, Direction currentDirection)
        {
            int[] newRequests = inputFloors.Split(',').Select(floor => int.Parse(floor.Trim())).ToArray();
            foreach (int floor in newRequests)
            {
                var inputFloorDirection = (floor > currentFloor) ? Direction.GoUp : Direction.GoDown;
                var newRequest = new RequestDetail(floor, inputFloorDirection, ElevatorID);
                if (requests.Contains(newRequest)) continue;
                var index = _elevatorManager.GetInsertPosition(newRequest, currentDirection, currentFloor, requests);
                requests.Insert(index, newRequest);
            }
            //remove the currentFloor from the list
            requests.RemoveAt(requests.FindIndex(r => r.GotoFloor == currentFloor && r.ElevatorID == ElevatorID));
        }
        [LogException]
        private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_elevatorRequests.Count > 0)
                {
                    lock (lockObj)
                    {
                        try
                        {
                            while (_elevatorRequests.Count > 0)
                            {
                                
                                bool shouldRemove = true;
                                var request = _elevatorRequests[0]; // Get the next request from the queue

                                _elevatorManager.MoveToFloor(request.GotoFloor);

                                CurrentDestination = request.GotoFloor;
                                CurrentDirection = request.DirectionRequest;

                                //use this for continuos logging even for multiple input prompt----------------------------------------------------------------------------
                                //Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                                //string? inputFloors = Console.ReadLine();
                                //end--------------------------------------------------------------------------------------------------------------------------------------

                                string? inputFloors;
                                lock (InputLock) // Ensure only one prompt at a time
                                {
                                    Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                                    inputFloors = Console.ReadLine();
                                }

                                if (!string.IsNullOrEmpty(inputFloors))
                                {
                                    try
                                    {
                                        if (inputFloors.Equals("x")) //simulates no new request with "x" input
                                        {
                                            _elevatorRequests.RemoveAt(0); 
                                            continue;
                                        }
                                        InsertRequestInOrder(_elevatorRequests, inputFloors, CurrentDestination, CurrentDirection);
                                        shouldRemove = false;
                                     }
                                    catch (FormatException)
                                    {
                                        throw new FormatException(ElevatorConstants.InvalidFormat);
                                    }
                                }
                                if (shouldRemove)
                                    _elevatorRequests.RemoveAt(0); // Remove the processed request
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ElevatorProcessRequestException(string.Format(ElevatorConstants.ProcessRequestError, ex.Message));
                        }
                    }
                }
                else
                {
                    // No requests available, so idle for a moment before checking again
                    await Task.Delay(500, cancellationToken); // Add a small delay to avoid excessive CPU usage
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
