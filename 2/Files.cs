using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace files
{
    class Program
    {
        
        static int Main(string[] args)
        {
            bool memory=false;
            List<UInt64> Numbers = new List<ulong>(); 
            StreamReader temp;
            foreach (string i in args)   
            {
                try
                {
                    temp=new StreamReader(i);
                }
                catch
                {
                    throw new Exception("Can't open file "+i);
                }
                string line;
                while ((line = temp.ReadLine()) != null && !memory) 
                {
                    string pattern = @"(\d{1,10})";
                    Regex rgx = new Regex(pattern);
                    MatchCollection matches = rgx.Matches(line);
                    if (matches.Count > 0)
                       foreach (Match match in matches)
                       {
                           if(!memory)
                           {
                               ulong e;
                               try
                               {
                                   e = Convert.ToUInt64(match.Value);
                               }                          
                               catch 
                               {
                                   throw new Exception("Number is too BIG");
                               }
                               String es = e.ToString();
                               if (es.Equals(match.ToString()))
                               {
                                   try
                                   {
                                       Numbers.Add(e);
                                   }
                                   catch
                                   {
                                       memory = true;
                                       throw new Exception("Not enough memory!");
                                   }
                               }
                           }
                       }
                }
                if(!memory)
                    break;
                temp.Close();
            }
            try
            {
                Numbers.Sort();
            }
            catch 
            {
                throw new Exception("Can't sort numbers. Probably not enough memory");
            }
            StreamWriter sw;
            try
            {
                 sw = new StreamWriter("out.txt");
            }
            catch
            {
                throw new Exception("Can't open output file for writing!");
            }
            foreach(int i in Numbers)
            {
                try
                {
                    sw.WriteLine(i);
                }
                catch 
                {
                    throw new Exception("Cant't writen to output file!");
                }
            }
            sw.Close();
            return 0;
        }
    }
}

