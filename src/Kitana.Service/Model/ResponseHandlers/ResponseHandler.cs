using Azure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Kitana.Service.Model.ResponseHandlers.ResponseHandler;
using static MongoDB.Libmongocrypt.CryptContext;

namespace Kitana.Service.Model.ResponseHandlers
{
    public class Response<T>
    {

        public bool Success { get; set; }

        public CustomStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public T Content { get; set; }
    }

    public class ResponseHandler
    {
        public enum CustomStatusCode
        {
            Success = 200,
            NotFound = 404,
            InternalServerError = 500,
            InvalidOperationException = 703,
            LogLimit = 705,
            AlreadyExist = 704,
            UnableToLog = 708,
            Exception = 700,
            AppAlreadyExist = 900,
            OrgAlreadyExist = 901,
            InvalidRetentionPeriod = 902,
            PlanAlreadyExist = 903,
            EventAlreadyExist = 904,
            EventCodeAlreadyExist = 905
        }

        public static Response<T> HandleSuccess<T>()
        {
            Response<T> response = new Response<T>()
            {
                Success = true,
                StatusCode = CustomStatusCode.Success,
            };
            return response;
        }

        public static Response<T> HandleSuccess<T>(string message)
        {
            Response<T> response = new Response<T>()
            {
                Success = true,
                StatusCode = CustomStatusCode.Success,
                Message = message
            };
            return response;
        }

        public static Response<T> HandleSuccess<T>(string message, T content)
        {
            Response<T> response = new Response<T>()
            {
                Success = true,
                StatusCode = CustomStatusCode.Success,
                Message = message,
                Content = content
            };
            return response;
        }

        public static Response<T> HandleError<T>(string statusCode, string message)
        {
            Response<T> response;

            switch (statusCode)
            {
                case "NotFound":
                    response = new()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.NotFound,
                        Message = message
                    };
                    break;
                case "InvalidOperationException":
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.InvalidOperationException,
                        Message = message
                    };
                    break;
                case "LogLimit":
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.LogLimit,
                        Message = message
                    };
                    break;
                case "InternalServerError":
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.InternalServerError,
                        Message = message
                    };
                    break;
                case "AlreadyExist":
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.AlreadyExist,
                        Message = message
                    };
                    break;
                case "UnableToLog":
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.UnableToLog,
                        Message = message
                    };
                    break;
                case "AppCode already exists":
                 response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.AppAlreadyExist,
                        Message = message
                    };
                    break;
                case "OrgCode already exists!":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.OrgAlreadyExist,
                        Message = message
                    };
                    break;
                case "OrgCode Not Found":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.NotFound,
                        Message = message
                    };
                    break;
                case "AppCode Not Found":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.NotFound,
                        Message = message
                    };
                    break;
                case "Invalid value for retention period":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.InvalidRetentionPeriod,
                        Message = message
                    };
                    break;
                case "Event Not Found":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.NotFound,
                        Message = message
                    };
                    break;
                case "Plan Already Exists":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.PlanAlreadyExist,
                        Message = message
                    };
                    break;
                case "Plan Not Found":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.NotFound,
                        Message = message
                    };
                    break;
                case "EventCode already exists":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.EventCodeAlreadyExist,
                        Message = message
                    };
                    break;
                case "Event Already Exists":
                response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.EventAlreadyExist,
                        Message = message
                    };
                    break;
                default:
                    response = new Response<T>()
                    {
                        Success = false,
                        StatusCode = CustomStatusCode.Exception,
                        Message = message
                    };
                    break;
            }
            return response;
        }
    }
}
