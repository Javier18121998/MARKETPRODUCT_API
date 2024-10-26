using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Market.Exceptions
{
    /// <summary>
    ///Represent handled / custom exception raised while changing PIN 
    /// </summary>
    public class CustomException : Exception
    {
        /// <summary>
        /// used to hold HTTP status code that is to be return.
        /// </summary>
        public HttpStatusCode StatusCode;

        /// <summary>The error code</summary>
        public string ErrorCode;

        /// <summary>
        /// Custom Exception
        /// </summary>
        public CustomException() : base()
        { }

        /// <summary>
        ///  Custom Exception
        /// </summary>
        /// <param name="message">Message to be returned</param>
        public CustomException(string message) : base(message)
        { }

        /// <summary>
        ///  Custom Exception
        /// </summary>
        /// <param name="message">Message to be returned</param>
        /// <param name="innerException">Inner Exception Details</param>
        public CustomException(string message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        ///  Custom Exception
        /// </summary>
        /// <param name="statusCode">Http Status Code that should be returned as API Response</param>
        /// <param name="message">Message to be returned</param>
        /// <param name="errorCode">Error Code that describe the error in detail</param>
        public CustomException(HttpStatusCode statusCode, string message, string errorCode = "") : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///  Custom Exception
        /// </summary>
        /// <param name="message">Message to be returned</param>
        /// <param name="statusCode">Http Status Code that should be returned as API Response</param>
        /// <param name="innerException">Inner Exception Details</param>
        /// <param name="errorCode">Error Code that describe the error in detail</param>
        public CustomException(HttpStatusCode statusCode, string message, Exception innerException, string errorCode = "") : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
