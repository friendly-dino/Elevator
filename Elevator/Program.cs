using Elevator.App.Utility;
using Elevator.App.Controller;
using Elevator.App.Interface;
using Elevator.App.Constants;
using Elevator.Enum;

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
                GenerateRequests.Start(elevatorController);
                
                //wait for tasks to finish
                var elevatorTasks = (elevatorController as ElevatorController)._elevators
                                    .Select(elevator => (elevator as Elevator)?.WaitForCompletion())
                                    .Where(task => task != null).ToList();
                await Task.WhenAll(elevatorTasks!);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
       
    }
    
}


