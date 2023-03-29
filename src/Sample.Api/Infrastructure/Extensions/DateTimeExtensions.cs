using System;

namespace Sample.Api.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    public static string GetDurationMilliseconds(this DateTime beginTime)
        => (DateTime.Now - beginTime).TotalMilliseconds.Round() + " ms.";
}