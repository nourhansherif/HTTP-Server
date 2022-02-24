using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public RequestMethod getMethod() { return method; }
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        public string[] getContent() { return contentLines; }
        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();
            
            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(new String[] { "\r\n" }, StringSplitOptions.None);
            
            //To Test Bad Request
            //requestLines[0]=requestLines[0].Replace(" ", "");
            
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;

            // Parse Request line

            if (!ParseRequestLine())
                return false;

            
            if (relativeURI.Equals("HeadTest.html"))
                method = RequestMethod.HEAD;
            
            if (!ValidateIsURI(relativeURI))
                return false;

            // Validate blank line exists

            if (!ValidateBlankLine())
                return false;

            // Load header lines into HeaderLines dictionary
            headerLines = new Dictionary<string, string>();

            if (!LoadHeaderLines())
                return false;

            contentLines = new string [requestLines.Length-(2 + headerLines.Count)];
            for (int i = 2 + headerLines.Count, j = 0; i < requestLines.Length; i++, j++)
                contentLines[j] = requestLines[i];

            
            return true;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();

            // request line = method URI httpVersion

            String[] requestLineParts = requestLines[0].Split(' ');
            if (requestLineParts.Length < 3)
                return false;

            String requestMethod = requestLineParts[0];

            if (requestMethod.Equals("GET"))
                method = RequestMethod.GET;
            else if (requestMethod.Equals("HEAD"))
                method = RequestMethod.HEAD;
            else if (requestMethod.Equals("POST"))
                method = RequestMethod.POST;
            else
                return false;
           
            relativeURI = requestLineParts[1].Split('/')[1];

            String version = requestLineParts[2];
            if (version.Equals("HTTP/0.9"))
                httpVersion = HTTPVersion.HTTP09;
            else if (version.Equals("HTTP/1.0"))
                httpVersion = HTTPVersion.HTTP10;
            else if (version.Equals("HTTP/1.1"))
                httpVersion = HTTPVersion.HTTP11;
            else
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            int lineNumber = 1;
            String line = requestLines[lineNumber];
            if (line.Length == 0)
                return false;

            while (line != "")
            {
                String[]splittedLine = line.Split(new String[] { ": " }, StringSplitOptions.None);
                headerLines[splittedLine[0]] = splittedLine[1];
                lineNumber++;
                line = requestLines[lineNumber];
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
         
            if (!requestLines.Contains(""))
                return false;
            
            return true;
        }

    }
}
