using System.Reflection;

namespace Elevator.App.Utility
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class LogExceptionAttribute : Attribute
    {
        public static void OnException(MethodBase method, Exception exception)
        {
            //for error logging
            Console.WriteLine($"Error in method {method.Name}: {exception.Message}");
        }
    }
}
