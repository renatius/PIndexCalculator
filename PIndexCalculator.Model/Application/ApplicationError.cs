namespace PICalculator.Model.Application
{
    public class ApplicationError
    {
        public string Message { get; private set; }

        public ApplicationError(string msg) {
            Message = msg;
        }
    }
}
