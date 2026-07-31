namespace Application.Exceptions
{
    public sealed class NotFoundException : Exception//Todo : make an basic custom exception for whole layer and inherit from it not from default Exception
    {
        public NotFoundException() { }

        public NotFoundException(string message) 
            : base(message) { }

        public NotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
    
}
