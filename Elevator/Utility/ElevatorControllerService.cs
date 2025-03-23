using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevator.App.Controller;
using Elevator.App.Interface;

namespace Elevator.App.Utility
{
    public static class ElevatorControllerService
    {
        private static IElevatorController? _controller;

        // Initialize the ElevatorController with parameters
        public static void Initialize(int maxFloors, int numberOfElevators)
        {
            if (_controller != null)
            {
                throw new InvalidOperationException("ElevatorController has already been initialized.");
            }

            _controller = new ElevatorController(maxFloors, numberOfElevators);
        }

        // Retrieve the shared instance of ElevatorController
        public static IElevatorController GetController()
        {
            if (_controller == null)
            {
                throw new InvalidOperationException("ElevatorController has not been initialized.");
            }
            return _controller;
        }
    }
}
