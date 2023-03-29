using System;
using Sample.Api.Infrastructure.Extensions;

namespace Sample.Api.Infrastructure.Http;

public class ErrorModel<T> : JsonModel where T : Enum
{
    public ErrorModel(T errorCode)
    {
        this.ErrorCode = errorCode;
        this.ErrorDescription = errorCode.GetDescription();
    }

    public T ErrorCode { get; set; }

    public string ErrorDescription { get; set; }
}