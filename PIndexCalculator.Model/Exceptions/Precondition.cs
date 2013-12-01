namespace PIndexCalculator.Model.Exceptions
{
    public static class Precondition
    {
        public static void Require(bool condition, string errorMessage) {
            if (!condition) 
                throw new BusinessException(errorMessage);
        }
    }
}
