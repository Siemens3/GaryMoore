﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class Logger
    {
        //public static string LogDirectoryPath = Environment.CurrentDirectory;

        public static void Log(String lines)
        {
            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            try
            {
                //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Error.log", true);
                //file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + lines);
                //file.Close();
                using (var db = new DataContext())
                {
                    Error errors = new Error();
                    errors.error = lines;
                    var err = db.Errors.Add(errors);
                    db.SaveChanges();
                }
            }
            catch
            {

            }
        }

       
    }
}