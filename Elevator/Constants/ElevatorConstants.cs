using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Constants
{
    public static class ElevatorConstants
    {
        public const int MoveDuration = 1000;
        public const int PassengerDuration = 1000;
        public const byte MaxFloors = 10;
        public const byte NumberOfElevators = 4;

        public const string NoElevatorMsg = "No available elevator found.";
        public const string InvalidReqMsg = "Request is invalid.";
        public const string DirectionUp = "is going UP";
        public const string DirectionDown = "is going DOWN";

        public const string RequestComplete = "Elevator {0} completed request: From {1}F -> To {2}F";
        public const string ProcessRequestError = "Elevator {0} encountered an error.";

    }
}
