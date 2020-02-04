using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.HTTP
{
    public enum HttpResponseCode
    {
        Ok = 200,
        MovedPermanently = 301,
        TemporaryRedirect = 307,
        NotFound = 404,
        Forbidden = 403,
        InternalServerError = 500,
        NotImplemented = 501,
    }
}
