using System.Collections.Concurrent;
using Elevator.App.Interface;
using Elevator.App.Utility;
using Elevator.App.Constants;
using Elevator.App.Exceptions;
using System.ComponentModel;
using System.Threading.Channels;
using System.Threading;

namespace Elevator.App.Controller
{
    public class ElevatorController : IElevatorController
    {
        #region Constructor
        public readonly List<IElevator> _elevators;
        private readonly IElevatorManager _elevatorManager;
        private readonly int _maxFloors;
        private readonly object lockObj = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SortedSet<RequestDetail> _sortedRequests = new(new RequestComparer());

        public ElevatorController(int maxFloors, int numberOfElevators)
        {
            _maxFloors = maxFloors;
            _elevators = [];

            for (int i = 0; i < numberOfElevators; i++)
                _elevators.Add(new Elevator(i + 1));

            _elevatorManager = new ElevatorManager(_elevators);
            Task.Run(() => ProcessRequestsAsync(_cancellationTokenSource.Token));
        }
        #endregion
        [LogException]
        public void AddRequest(RequestDetail request)
        {
            if (request.GotoFloor > _maxFloors || request.GotoFloor < 1)
                throw new InvalidRequestException(ElevatorConstants.InvalidReqMsg);
            lock (lockObj)
                _sortedRequests.Add(request);
        }

        //public void AddRequest(RequestDetail request)
        //{
        //    if ( request.GotoFloor > _maxFloors || request.GotoFloor < 1)
        //        throw new InvalidRequestException(ElevatorConstants.InvalidReqMsg);

        //    lock (lockObj)
        //        _requests.Add(request);
        //}
        [LogException]
        private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                RequestDetail? nextRequest = null;
                lock (lockObj)
                {
                    if (_sortedRequests.Count > 0)
                    {
                        nextRequest = _sortedRequests.Min; // Get the lowest-priority item
                        _sortedRequests.Remove(nextRequest); // Remove after processing
                    }
                }

                if (nextRequest != null)
                {
                    try
                    {
                        IElevator? bestElevator = GetBestElevatorForRequest(nextRequest);
                        if (bestElevator != null)
                            bestElevator.AddRequest(nextRequest);
                        else
                            Console.WriteLine("No available elevator for this request.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing request: {ex.Message}");
                    }
                }
                else
                    await Task.Delay(500); // Idle wait if no requests
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
