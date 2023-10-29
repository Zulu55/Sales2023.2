namespace Orders.Shared.Responses
{
    public class Response<T>
    {
        public bool WasSuccess { get; set; }

        public string? Message { get; set; }

        public T? Result { get; set; }
    }
}