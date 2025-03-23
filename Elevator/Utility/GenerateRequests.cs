using Elevator.App.Constants;
using Elevator.App.Interface;
using Elevator.Enum;

namespace Elevator.App.Utility
{
    public static class GenerateRequests
    {
        /// <summary>
        /// Starts generating random elevator requests at regular intervals.
        /// </summary>
        /// <param name="elevatorController">The instance of the elevator controller responsible for processing requests.</param>
        public static void Start(IElevatorController elevatorController)
        {
            Random random = new Random();
            int minFloor = 1;
            int maxFloor = ElevatorConstants.MaxFloors;

            Task.Run(async () =>
            {
                while (true)
                {
                    int floor = random.Next(minFloor, maxFloor + 1);
                    Direction direction;

                    if (floor == minFloor)
                        direction = Direction.GoUp; // 1F can only go up
                    else if (floor == maxFloor)
                        direction = Direction.GoDown; // 10F can only go down
                    else
                        direction = (Direction)random.Next(0, 2); // 0 = GoUp, 1 = GoDown

                    elevatorController.AddRequest(new RequestDetail(floor, direction));

                    int delay = random.Next(5000, 10000);
                    await Task.Delay(delay);
                }
            });
        }
    }
}
