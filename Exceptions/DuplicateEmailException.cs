namespace RAT_AUTH_API.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message)
        {
            
        }
    }
}