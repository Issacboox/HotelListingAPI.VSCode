namespace HotelListingAPI.VSCode.Models.ApiResponse
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int? Count { get; set; } // Optional field to indicate the number of items if applicable

        public ApiResponse(int statusCode, bool success, string message, T data, int? count = null)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Data = data;
            Count = count;
        }
    }
}