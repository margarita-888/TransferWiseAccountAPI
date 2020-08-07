using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest_TransferWiseWebhook
{
    public class ControllerContextHelper
    {
        // Replace this with your own signature
        private const string signature = "UEraUmtRUr+bPCzOOKyQaf0rvGXYiLjZiKmvRzhx4kJz0IQDXxdIqRmOxbB5ejwi+M/qKEJVO+UoeePCmn4cffDP9YT8u3nZiErOaKNRTn7gzSGPEswck1rQ6ga/gV154yqRjYlVjKJUBCASWY5FelMAu77s1uI+4ELeyTioHCLwCzLfuZKaBUUJQ2/637IimWl7bHaVOIgM2TbNNwCPt4VvWaZ5nwmOkiOD1l9TXOxYMx75ah7sfJn16TjBgx3pcktKc7nVzxozy8lpMoLUPLE1F51oKV86wweww8S2jEpr8xm+0KJkoowezhko4Bh3zOaxSa77P7fUfG7Tmvy3xg==";

        private const string invalidSignature = "aBcD7ELv/kIQKUXjVkEHlhKo6fVQSRU1TdIpu0VvO0RvWLEGy3nasurjsWvnbr6089x0uS1aZwaWCt+nThGKUEoIBTehTMkdZKvqZcfHI3RAdKwo9aPPpDZ+DZBM1lKZ1t+vqmqc/XHs/Z5Yv77+0z7H4FK19WOTuskPXe+WgFoSTTbWe8/QbR5DRFdeBX3VU1r4MznWTNzNl6uVgn61b/sq3cR7eqxVNimoXPtEaGpVumAnrDhRybTp12OI7eZZ4A78qPPU7kJCgcAXvqMtfDsQjjSMjUcpeMnWu4gVLrabgfSPK104J8z2CJ55v+3Yi5j1B4NWsilliKI4eU2Uqg==";

        // Create "X-Signature-SHA256" request header
        public static ControllerContext Create(bool withValidSignature)
        {
            var httpContext = new DefaultHttpContext();

            if (withValidSignature)
                httpContext.Request.Headers.Add("X-Signature-SHA256", signature);
            else
                httpContext.Request.Headers.Add("X-Signature-SHA256", invalidSignature);

            var actx = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            return new ControllerContext(actx);
        }
    }
}