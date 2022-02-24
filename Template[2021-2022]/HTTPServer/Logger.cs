using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            StreamWriter sr = new StreamWriter("log.txt", true);
            DateTime localDate = DateTime.Now;
            sr.WriteLine("Datetime: " + localDate.ToString());
            sr.WriteLine("messege: "+ex.Message);
            sr.Close();
            
            
        }
    }
}
