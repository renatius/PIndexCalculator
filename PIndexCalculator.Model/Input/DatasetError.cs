namespace PICalculator.Model.Input 
{
    
    public class DatasetError
    {
        public string Message { get; private set; }

        public DatasetError(string message)
        {
            Message = message;
        }
    }
}
