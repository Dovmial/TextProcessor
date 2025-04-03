

using System.Text;

namespace TextProcessor.Extensions
{
    public static class ExceptionExtensions
    {
        public static string MessageWithInnerExceptions(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            Exception currentEx = ex;
            sb.AppendLine(currentEx.Message);
            for (int i = 1; ex.InnerException is not null; i++)
            {
                currentEx = ex.InnerException;
                sb.AppendLine($"Inner{i}: {currentEx.Message}");
            }
            return sb.ToString();
        }
    }
}
