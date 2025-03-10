using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Interface
{
    public interface IElevatorManager
    {
        IElevator FindBestElevator(int requestedFloor);
    }
}
