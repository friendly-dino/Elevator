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
        //private SortedSet<RequestDetail> _elevatorRequests = new(new RequestComparer());
        private List<RequestDetail> _elevatorRequests = [];
        //private Queue<RequestDetail> _elevatorRequests = [];
        public Elevator(int id, bool startTask = true)
        {
            ElevatorID = id;
            if (startTask)
                _elevatorTask = ProcessRequestsAsync(_cancellationTokenSource.Token);
            _elevatorManager = new ElevatorManager(ElevatorID);
            //_elevatorRequests = new SortedSet<RequestDetail>(new RequestComparer(CurrentDirection));
        }
        #endregion
        public void AddRequest(RequestDetail request)//note: check if the sorting logic is better to place here
        {
            lock (lockObj)
                _elevatorRequests.Add(request);
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
        //private void UpdateComparer()
        //{
        //    lock (lockObj)
        //    {
        //        var updatedRequests = new SortedSet<RequestDetail>(_elevatorRequests, new RequestComparer(CurrentDirection));
        //        _elevatorRequests = updatedRequests;
        //    }
        //}
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
            //int currentFloorIndex = ;
            requests.RemoveAt(requests.FindIndex(r => r.GotoFloor == currentFloor));
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
                            // Loop over the current requests in _elevatorRequests, checking if any new requests come in
                            while (_elevatorRequests.Count > 0)
                            {
                                // Get the next request from the queue
                                bool shouldRemove = true;
                                var request = _elevatorRequests[0];

                                // Move elevator to the requested floor
                                _elevatorManager.MoveToFloor(request.GotoFloor);

                                CurrentDestination = request.GotoFloor;
                                CurrentDirection = request.DirectionRequest;//_elevatorManager.CurrentDirection;

                                string ? inputFloors;
                                lock (InputLock) // Ensure only one prompt at a time
                                {
                                    Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                                    inputFloors = Console.ReadLine();
                                }

                                if (!string.IsNullOrEmpty(inputFloors))
                                {
                                    try
                                    {
                                        // If input is "x", it simulates no new request (could be modified for other behavior)
                                        if (inputFloors.Equals("x"))
                                        {
                                            _elevatorRequests.RemoveAt(0); 
                                            continue;
                                        }
                                        InsertRequestInOrder(_elevatorRequests, inputFloors, CurrentDestination, CurrentDirection);
                                        shouldRemove = false;
                                     }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Invalid floor format.");
                                    }
                                }

                                // Remove the processed request
                                if (shouldRemove)
                                {
                                    _elevatorRequests.RemoveAt(0);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing requests: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // No requests available, so idle for a moment before checking again
                    await Task.Delay(500, cancellationToken); // Add a small delay to avoid excessive CPU usage
                }

                //RequestDetail? nextRequest = null;
                //if (_elevatorRequests.Count > 0)
                //{
                //    lock (lockObj)
                //    {
                //        try
                //        {
                //            // Process the request
                //            //nextRequest.OriginFloor = CurrentDestination;
                //            //_elevatorManager.MoveToFloor(nextRequest.GotoFloor);
                //            var listCopy = _elevatorRequests;
                //            foreach (var request in listCopy.ToList())
                //            {
                //                Debug.WriteLine($"{request.GotoFloor}--{request.DirectionRequest}");
                //                Debug.WriteLine($"XXXXXXX");
                //                _elevatorManager.MoveToFloor(request.GotoFloor, out Direction currentDirection);

                //                CurrentDestination = request.GotoFloor;
                //                CurrentDirection = _elevatorManager.CurrentDirection;//currentDirection;//request.DirectionRequest;

                //                //use this for continuos logging even for multiple input prompt----------------------------------------------------------------------------
                //                //Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                //                //string? inputFloors = Console.ReadLine();
                //                //end--------------------------------------------------------------------------------------------------------------------------------------

                //                string? inputFloors;
                //                lock (InputLock) // Ensure only one prompt at a time
                //                {
                //                    Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                //                    inputFloors = Console.ReadLine();
                //                }
                //                if (!string.IsNullOrEmpty(inputFloors))
                //                {
                //                    try
                //                    {
                //                        if (inputFloors.Equals("x"))
                //                        {
                //                            //CurrentDirection = (CurrentDestination > request.GotoFloor ) ? Direction.GoUp : Direction.GoDown;
                //                            _elevatorRequests.Remove(request);
                //                            foreach (var item in _elevatorRequests)
                //                            {
                //                                Debug.WriteLine($"{item.GotoFloor}--{item.DirectionRequest}");
                //                            }
                //                            Debug.WriteLine($"CCCCC");
                //                            continue; // Simulate no new requests with "0"
                //                        }

                //                        int[] floors = _elevatorManager.SortFloors(inputFloors, CurrentDestination, CurrentDirection);
                //                        foreach (int floor in floors)
                //                        {
                //                            Console.WriteLine($"CurrentDestination >>>> {_elevatorManager.CurrentFloor} ");

                //                            var newRequest = new RequestDetail(floor, CurrentDirection, ElevatorID);
                //                            CurrentDirection = (floor > _elevatorManager.CurrentFloor) ? Direction.GoUp : Direction.GoDown;
                //                            InsertInOrder(_elevatorRequests, newRequest, CurrentDestination, CurrentDirection);
                //                            foreach (var item in _elevatorRequests)
                //                            {
                //                                Debug.WriteLine($"{item.GotoFloor}--{item.DirectionRequest}");
                //                            }
                //                            Debug.WriteLine($"====");
                //                            //AddRequest(new RequestDetail(floor, CurrentDirection, ElevatorID));
                //                        }
                //                        //var inputFloorsList =  new List<RequestDetail>();
                //                        //int[] floors = inputFloors.Split(',').Select(floor => int.Parse(floor.Trim())).ToArray();
                //                        //foreach (var floor in floors)
                //                        //{
                //                        //    inputFloorsList.Add(new RequestDetail(floor, CurrentDirection, ElevatorID));
                //                        //}
                //                        //var sortedRequests = _elevatorManager.SortFloors(_elevatorRequests.ToList(), inputFloorsList, CurrentDestination, CurrentDirection);
                //                        //foreach (var item in floors)
                //                        //{

                //                        //}
                //                    }
                //                    catch (FormatException)
                //                    {
                //                        throw new FormatException(ElevatorConstants.InvalidFormat);
                //                    }
                //                }
                //                _elevatorRequests.Remove(request);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            throw ex;
                //        }
                //    }
                //}
                //else
                //{
                //    // Idle delay when no requests are pending
                //    await Task.Delay(500, cancellationToken);
                //}
                if (false)
                    {
                        try
                        {
                            // Process the request
                            //nextRequest.OriginFloor = CurrentDestination;
                            //_elevatorManager.MoveToFloor(nextRequest.GotoFloor);

                            //CurrentDestination = nextRequest.GotoFloor;
                            //CurrentDirection = nextRequest.DirectionRequest;
                            
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
                                        if (inputFloors.Equals("0")) continue; // Simulate no new requests with "0"

                                        int[] floors = _elevatorManager.SortFloors(inputFloors, CurrentDestination, CurrentDirection);
                                        //int[] floors = inputFloors.Split(',').Select(floor => int.Parse(floor.Trim())).ToArray();
                                        foreach (int floor in floors)
                                        {
                                            AddRequest(new RequestDetail(floor, CurrentDirection, ElevatorID));
                                            //CurrentDirection = (floor > CurrentDestination) ? Direction.GoUp : Direction.GoDown;
                                            //CurrentDestination = floor;
                                            //UpdateComparer();
                                        }    
                                    }
                                    catch (FormatException)
                                    {
                                        throw new FormatException(ElevatorConstants.InvalidFormat);
                                    }
                                }
                            }
                        catch (Exception ex)
                        {
                            throw new ElevatorProcessRequestException(String.Format(ElevatorConstants.ProcessRequestError,ElevatorID,ex.Message));
                            //Console.WriteLine($"Error processing request in Elevator {ElevatorID}: {ex.Message}");
                        }
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
