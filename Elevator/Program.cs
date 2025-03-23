using Elevator.Enum;
using Elevator.App.Controller;
using Elevator.App.Interface;
using Elevator.App.Constants;

namespace Elevator.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IElevatorController elevatorController = new ElevatorController(ElevatorConstants.MaxFloors, ElevatorConstants.NumberOfElevators);

                //SIMULATE USER REQUESTS

                //why i addded elevator id in parameter:
                //it is to try simulating where the users picked the same elevator on the same floor, but they requested different floors

                elevatorController.AddRequest(new RequestDetail(3, Direction.GoDown));
                //Thread.Sleep(5000);
                //elevatorController.AddRequest(new RequestDetail(5, Direction.GoUp));
                //elevatorController.AddRequest(new RequestDetail(9, Direction.GoDown));
                //elevatorController.AddRequest(new RequestDetail(7, Direction.GoDown));

                var elevatorTasks = (elevatorController as ElevatorController)._elevators
                                    .Select(elevator => (elevator as Elevator)?.WaitForCompletion())
                                    .Where(task => task != null).ToList();

                await Task.WhenAll(elevatorTasks!);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
    }
    
}


