using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Domain.Entities.Constants
{
    public static class SystemMessageSettings
    {
        public static Dictionary<int, string> Settings = new Dictionary<int, string>()
        {
            { SystemMessageSettingNumbers.ExceptionMessageWithIdentifier, "SOMETHING WENT WRONG. Please contact customer support and share the identifer: {0} to lookup"},
            { SystemMessageSettingNumbers.ExceptionMessageWithTrace,"SOMETHING WENT WRONG. Not able to log error into the system: Exception: {0}, InnerException: {1}, StackTrace: {2} " },
            { SystemMessageSettingNumbers.ValidationErrors, "Validation Errors Occurred."},
            { SystemMessageSettingNumbers.Success, "Success!"},
            { SystemMessageSettingNumbers.Error, "An Error Occured."},
        };
    }
    public static class SystemMessageSettingNumbers
    {
        public const int ExceptionMessageWithIdentifier = 5000;
        public const int ExceptionMessageWithTrace = 5001;
        public const int ValidationErrors = 5002;
        public const int Success = 5003;
        public const int Error = 5004;

    }
}
