using Humanizer;
using System.Collections.Concurrent;
using System.Net;
using VoiceMatters.Shared.Exceptions;

namespace Shared.Exceptions
{
    public sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        private static readonly ConcurrentDictionary<Type, string> Codes = new();

        public ExceptionResponse Map(Exception exception)
        {
            if (exception is VoiceMattersException ex)
            {
                if (exception is BadRequestException)
                {
                    return new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(ex), ex.Message)), HttpStatusCode.BadRequest);
                }
                else if (exception is AuthorizationException)
                {
                    return new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(ex), ex.Message)), HttpStatusCode.Forbidden);
                }
                else
                {
                    return new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(ex), ex.Message)), HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw exception;
            }
        }




        private record Error(string Code, string Message);

        private record ErrorsResponse(params Error[] Errors);

        private static string GetErrorCode(object exception)
        {
            var type = exception.GetType();
            return Codes.GetOrAdd(type, type.Name.Underscore().Replace("_exception", string.Empty));
        }
    }
}
