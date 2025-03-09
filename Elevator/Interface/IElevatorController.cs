using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.Interface
{
    public interface IElevatorController
    {
        void AddRequest(RequestDetail request);
    }
}
