using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint hostEndPoint;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.hostEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Bind(this.hostEndPoint);
            this.serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);

                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            Socket clientSocket = (Socket)obj;
            
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] dataReceived = new byte[1024 * 1024];
                    int receivedLen = clientSocket.Receive(dataReceived);
                    //Console.WriteLine(Encoding.ASCII.GetString(dataReceived, 0, receivedLen));
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(dataReceived, 0, receivedLen));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                    
                    Console.WriteLine(response.ResponseString);
                    
                    
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string webPage;
            try
            {
                //TODO: check for bad request 
                
                if (request.ParseRequest() == false)
                {
                    webPage = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", webPage, "");
                }
                
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Configuration.RootPath + "\\" + request.relativeURI;
                Console.WriteLine("Trying to open " + physicalPath);
                
                //TODO: check for redirect
                string path = GetRedirectionPagePathIFExist(request.relativeURI);

               
                if (path.Length != 0)
                {
                    webPage = LoadDefaultPage(path);
                    return new Response(StatusCode.Redirect, "text/html", webPage, path);
                }
                
                //TODO: check file exists
                
                //TODO: read the physical file

                    
                // Create OK response
                webPage = LoadDefaultPage(request.relativeURI);
                
                if(webPage.Length == 0)
                {
                    webPage = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", webPage, "");
                }

                if (request.getMethod().Equals(RequestMethod.POST))
                    PostMethod(request);
                else if (request.getMethod().Equals(RequestMethod.HEAD))
                    HeadMethod(request, webPage);

                return new Response(StatusCode.OK, "text/html", webPage, "");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
                Logger.LogException(new Exception("Filepath not exist"));

            // else read file and return its content
            else
                return File.ReadAllText(filePath);

            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                StreamReader sr = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();

                while (sr.Peek()!=-1)
                {
                    String[] line = sr.ReadLine().Split(',');
                    Configuration.RedirectionRules[line[0]] = line[1];
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                //Environment.Exit(1);
                Logger.LogException(ex);

            }
        }
        private void PostMethod(Request req)
        {

            StreamWriter sr = new StreamWriter("formResults.txt", true);
            string[] content = req.getContent();
            foreach (string line in content)
            {
                //fname=runtime&lname=terror&mainMember=jojooo
                string[] formParts = line.Split('&');
                foreach (string part in formParts)
                {
                    string[] labelAndValue = part.Split('=');
                    sr.WriteLine(labelAndValue[0] + " : " + labelAndValue[1]);
                }
                sr.WriteLine("----------------------------------");
            }
            sr.Close();
        }
        private void HeadMethod(Request request, string webPage)
        {
            int indexOfMeta = webPage.IndexOf("<meta");
            if (indexOfMeta == -1)
            {
                Console.WriteLine("The "+ request.relativeURI+ " file has no meta data \n---------------------------------");
                return;
            }
            int indexOfEndMeta = webPage.IndexOf(">",indexOfMeta) ;
            string metaInformation = webPage.Substring(indexOfMeta + 5, indexOfEndMeta - (indexOfMeta+5)).Trim();
           
            string[] m = metaInformation.Split(' ');
            Console.WriteLine("The "+request.relativeURI+" Meta Data is: "+"\n------------------------------");
            foreach (string data in m)
            {
                string[] parts = data.Split('=');
                Console.WriteLine(parts[0] + " : " + parts[1]);
            }
            Console.WriteLine("-------------------------------------------");
        }
            
    }
}
