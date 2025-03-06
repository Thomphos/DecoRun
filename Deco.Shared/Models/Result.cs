using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deco.Shared.Models
{
    public abstract class Result
    {
        [JsonPropertyName("Success")]
        [JsonPropertyOrder(0)]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("ErrorMessage")]
        [JsonPropertyOrder(1)]
        public string ErrorMessage { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
    }

    public class Result<T> : Result
    {
        protected Result(T? value = default)
        {
            Data = value;
            IsSuccess = value is not null;
            ErrorMessage = IsSuccess ? string.Empty : "Value can not be null";
        }

        public Result()
        {

        }

        [JsonPropertyName("Data")]
        [JsonPropertyOrder(2)]
        public T? Data { get; set; }


    }

    public class Success : Result
    {
        public Success()
        {
            IsSuccess = true;
            StatusCode = HttpStatusCode.OK;
        }
    }

    public class Success<T> : Result<T> where T : notnull
    {
        public Success() { IsSuccess = true; }
        public Success(T data) : base(data)
        {
            IsSuccess = true;
            Data = IsSuccess
                ? data ?? throw new Exception("A success result can not have a body of null")
                : throw new Exception($"{typeof(Success)} can not be created if {nameof(IsSuccess)} is false");
            StatusCode = HttpStatusCode.OK;
        }
    }

    public class Failure : Result
    {
        public Failure() { }
        public Failure(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
            StatusCode = HttpStatusCode.BadRequest;
        }

        public Failure(Exception e) : this(e.Message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }

    public class Failure<T> : Result<T>
    {
        public Failure() { }
        public Failure(string errorMessage, T? error = default) : base(error)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
            StatusCode = HttpStatusCode.BadRequest;
        }

        public Failure(Exception e) : this(e.Message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
