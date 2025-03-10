using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.App.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }
    public class ElevatorNotFoundException : Exception
    {
        public ElevatorNotFoundException(string message) : base(message) { }
    }
    public class ElevatorProcessRequestException : Exception
    {
        public ElevatorProcessRequestException(string message) : base(message) { }
    }
}
