namespace PICalculator.Model.Output
{
    public class PanelError
    {
        public string Message { get; private set; }

        public PanelError(string message) {
            Message = message;
        }
    }
}
