using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public List<string> getHeaderLines()
        {
            return headerLines;
        }
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            DateTime localDate = DateTime.Now;
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type: " + contentType );
            headerLines.Add("Content-Length: " + content.Length );
            headerLines.Add("Date: " + localDate.ToString() );

            if (redirectoinPath != "")
                headerLines.Add("Location: " + redirectoinPath );

            

            // TODO: Create the request string
            this.responseString = GetStatusLine(code)+ "\r\n" + String.Join("\r\n", headerLines)+"\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion  + " " ;
            if (code == StatusCode.BadRequest)
                statusLine += (int)StatusCode.BadRequest + " " + "Bad Request";
            else if (code == StatusCode.InternalServerError)
                statusLine += (int)StatusCode.InternalServerError + " " + "Internal Server Error";
            else if (code == StatusCode.NotFound)
                statusLine += (int)StatusCode.NotFound + " " + "Not Found";
            else if (code == StatusCode.OK)
                statusLine += (int)StatusCode.OK+ " " + "Ok";
            else if (code == StatusCode.Redirect)
                statusLine += (int)StatusCode.Redirect + " " + "Redirect";
            return statusLine;
        }
    }
}
