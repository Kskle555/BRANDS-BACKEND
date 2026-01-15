using System;

namespace ProductManagement.Domain.Common
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = null;
        public string? ErrorDetails { get; set; } // Hata ayıklama için

        // Başarılı cevaplar için yardımcı metot
        public static ServiceResponse<T> SuccessResponse(T data, string message = "İşlem başarılı")
        {
            return new ServiceResponse<T> { Data = data, Success = true, Message = message };
        }

        // Hatalı cevaplar için yardımcı metot
        public static ServiceResponse<T> ErrorResponse(string message, string errorDetails = null)
        {
            return new ServiceResponse<T> { Success = false, Message = message, ErrorDetails = errorDetails };
        }
    }
}