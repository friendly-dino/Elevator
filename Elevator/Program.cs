using Elevator;
using Elevator.App.Controller;
using Elevator.App.Interface;

try
{
    int maxFloors = 10;
    int numberOfElevators = 4;

    IElevatorController elevatorController = new ElevatorController(maxFloors, numberOfElevators);

    // simulate user request

    elevatorController.AddRequest(new RequestDetail(1, 2));
    elevatorController.AddRequest(new RequestDetail(1, 4));
    elevatorController.AddRequest(new RequestDetail(3, 9));
    elevatorController.AddRequest(new RequestDetail(1, 10,1));
    //elevatorController.AddRequest(new RequestDetail(9, 4));
    //elevatorController.AddRequest(new RequestDetail(7, 2));
    //elevatorController.AddRequest(new RequestDetail(8, 9));
    //elevatorController.AddRequest(new RequestDetail(10, 1));
    //elevatorController.AddRequest(new RequestDetail(7, 5));
    //elevatorController.AddRequest(new RequestDetail(1, 5));
    //elevatorController.AddRequest(new RequestDetail(10, 2));
    //elevatorController.AddRequest(new RequestDetail(9, 3));
    //elevatorController.AddRequest(new RequestDetail(4, 2));
    //elevatorController.AddRequest(new RequestDetail(9, 10));
    //elevatorController.AddRequest(new RequestDetail(9, 8));

    Console.ReadKey();

}
catch (Exception)
{
	throw;
}
