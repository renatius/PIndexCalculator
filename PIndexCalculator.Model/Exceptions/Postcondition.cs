namespace PIndexCalculator.Model.Exceptions
{

    public static class Postcondition
    {
        public static void Ensure(bool condition, string errorMessage) {
            if (!condition) 
                throw new BusinessException(errorMessage);
        }

    }
}
