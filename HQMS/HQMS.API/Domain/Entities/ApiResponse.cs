namespace HQMS.API.Domain.Entities
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data)
        {
            IsSuccess = true;
            Data = data;
        }

        public ApiResponse(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
    }

}
