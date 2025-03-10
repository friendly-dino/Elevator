using Elevator.App;
using Elevator.App.Controller;
using Elevator.App.Interface;
using Elevator.App.Constants;

try
{

    IElevatorController elevatorController = new ElevatorController(ElevatorConstants.MaxFloors, ElevatorConstants.NumberOfElevators);

    //SIMULATE USER REQUESTS

    //why i addded elevator id in parameter:
    //it is to try simulating where the users picked the same elevator on the same floor, but they requested different floors

    //same elevator and origin --would continue to floor 4
    //elevatorController.AddRequest(new RequestDetail(1, 3,1));
    //elevatorController.AddRequest(new RequestDetail(1, 4,1));

    //same origin, elevator not specified 
    //would go back to floor 1 on second request as it is treated as a new request while the elevator is moving
    //elevatorController.AddRequest(new RequestDetail(1, 7));
    //elevatorController.AddRequest(new RequestDetail(1, 4));

    //same origin origin, same destination, elevator not specified
    //treatd as separated requests so elevator would go back to 1st floor after completing the initial request
    //if elevator id is specified, would make one trip only
    //elevatorController.AddRequest(new RequestDetail(1, 4));
    //elevatorController.AddRequest(new RequestDetail(1, 4));

    //down,no elevID specified, complete normal trip
    //elevatorController.AddRequest(new RequestDetail(8, 7));
    //elevatorController.AddRequest(new RequestDetail(10, 1));

    //down, same destination, different origin - completes the first request then goes back to 6F
    //doesnt yo-yo but efficiency can be improved by making the elevator trip pass only once while going down 
    elevatorController.AddRequest(new RequestDetail(7, 5));
    elevatorController.AddRequest(new RequestDetail(6, 5));

    //random calls
    //elevatorController.AddRequest(new RequestDetail(10, 2));
    //elevatorController.AddRequest(new RequestDetail(9, 3));
    //elevatorController.AddRequest(new RequestDetail(4, 2));
    //elevatorController.AddRequest(new RequestDetail(9, 10));
    //elevatorController.AddRequest(new RequestDetail(9, 8));

    //request different origin, specified same elevator --this scenario should not be possible in real life
    //elevatorController.AddRequest(new RequestDetail(3, 9,1));
    //elevatorController.AddRequest(new RequestDetail(4, 10,1));
    Console.ReadKey();

}
catch (Exception ex)
{
	throw ex;
}
