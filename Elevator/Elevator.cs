using Elevator.Enum;
using Elevator.App.Interface;
using System.Collections.Concurrent;
using Elevator.App.Constants;
using Elevator.App.Utility;
using Elevator.App.Exceptions;
using System.Threading.Channels;

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
        private SortedSet<RequestDetail> _elevatorRequests = new(new RequestComparer(Direction.Idle));
        public Elevator(int id, bool startTask = true)
        {
            ElevatorID = id;
            if (startTask)
                _elevatorTask = ProcessRequestsAsync(_cancellationTokenSource.Token);
            _elevatorManager = new ElevatorManager(ElevatorID);
            //_elevatorRequests = new SortedSet<RequestDetail>(new RequestComparer(CurrentDirection));
        }
        #endregion
        public void AddRequest(RequestDetail request)
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
        private void UpdateComparer()
        {
            lock (lockObj)
            {
                var updatedRequests = new SortedSet<RequestDetail>(_elevatorRequests, new RequestComparer(CurrentDirection));
                _elevatorRequests = updatedRequests;
            }
        }
        [LogException]
        private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    RequestDetail? nextRequest = null;
                    lock (lockObj)
                    {
                        if (_elevatorRequests.Count > 0)
                        {
                            nextRequest = _elevatorRequests.Min; // Retrieve the smallest request
                            _elevatorRequests.Remove(nextRequest); // Remove it after retrieving
                        }
                    }
                    if (nextRequest != null)
                    {
                        try
                        {
                            // Process the request
                            nextRequest.OriginFloor = CurrentDestination;
                            _elevatorManager.MoveToFloor(nextRequest.GotoFloor);

                            CurrentDestination = nextRequest.GotoFloor;
                            CurrentDirection = nextRequest.DirectionRequest;

                            //Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                            //string? inputFloors = Console.ReadLine();

                            string? inputFloors;
                            lock (InputLock) // Ensure only one thread prompts at a time
                            {
                                Console.WriteLine($"Elevator {ElevatorID} has reached {CurrentDestination}F [{CurrentDirection}]. Please enter your destination floor:");
                                inputFloors = Console.ReadLine();
                            }

                            if (!string.IsNullOrEmpty(inputFloors))
                                {
                                    try
                                    {
                                        if (inputFloors.Equals("0")) continue; // Simulate no new requests with "0"

                                        int[] floors = inputFloors.Split(',').Select(floor => int.Parse(floor.Trim())).ToArray();

                                        foreach (int floor in floors)
                                        {
                                            CurrentDirection = (floor > CurrentDestination) ? Direction.GoUp : Direction.GoDown;
                                            UpdateComparer();
                                            //AddRequest(new RequestDetail(nextRequest.OriginFloor, floor, ElevatorID));
                                            AddRequest(new RequestDetail(floor, CurrentDirection, ElevatorID));
                                        }    
                                    }
                                    catch (FormatException)
                                    {
                                        throw new FormatException("Invalid input format. Only accepts a number or comma-separated numbers (e.g., 1,2,4).");
                                    }
                                }
                            }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing request in Elevator {ElevatorID}: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Idle delay when no requests are pending
                        await Task.Delay(500, cancellationToken);
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
