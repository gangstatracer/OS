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
       static void ExcDisplay (Exception exc, string msg)
        {
            Console.WriteLine(msg + exc.Message);
         }

        static int Main(string[] args)
        {
            List<UInt64> Numbers = new List<ulong>();
            StreamReader temp;
            foreach (string i in args)
            {
                try
                {
                    temp = new StreamReader(i);
                }
                catch(Exception exc)
                {
                    ExcDisplay(exc, "Can't open file: "+ i);
                    continue;
                }
                string line;
                try
                {
                    while ((line = temp.ReadLine()) != null)
                    {
                        string pattern = @"(\d{1,10})";
                        Regex rgx = new Regex(pattern);
                        MatchCollection matches = rgx.Matches(line);
                        if (matches.Count > 0)
                            foreach (Match match in matches)
                            {
                                ulong e = Convert.ToUInt64(match.Value);
                                String es = e.ToString();
                                if (es.Equals(match.ToString()))
                                    Numbers.Add(e);
                            }

                    }
                }
                catch (IOException exc)
                {
                    ExcDisplay(exc, "Unable to read data");
                    temp.Close();
                    continue;
                }
                catch (OutOfMemoryException exc)
                {
                    ExcDisplay(exc, "Not enough memory");
                    temp.Close();
                    return -1;
                }
                catch (ArgumentNullException exc)
                {
                    ExcDisplay(exc, "Argument NULL");
                    return -1;
                }
                catch (OverflowException exc)
                {
                    ExcDisplay(exc, "Overflow: ");
                    return -1;
                }
                catch(Exception exc) 
                {
                 ExcDisplay(exc, "Error: ");
                 return -1;
                }

              
                temp.Close();
            }
            try
            {
                Numbers.Sort();
                StreamWriter sw;
                sw = new StreamWriter("out.txt");
                foreach (int i in Numbers)
                {
                    sw.WriteLine(i);
                }
                sw.Close();
                return 0;
            }
            catch (InvalidOperationException exc)
            {
                ExcDisplay(exc, "The exception that is thrown when a method call is invalid for the object's current state.");
                return -1;
            }
            catch (UnauthorizedAccessException exc)
            {
                ExcDisplay(exc, "The exception that is thrown when the operating system denies access because of an I/O error or a specific type of security error.");
                return -1;
            }
            catch (IOException exc)
            {
                ExcDisplay(exc, "The exception that is thrown when an error occurs, the input-output.");
                return -1;
            }
            catch (Exception exc)
            {
                ExcDisplay(exc, "The exception: ");
                return -1;
            }
        }
    }
}


