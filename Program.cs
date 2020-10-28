using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace boggleSolver
{
   class Program
    {
        static void Main(string[] args)
        {    
            TestBoggle();
        }
        
        private static void TestBoggle()
        {
            bool noError = true;
            noError = RunBoggleTest(@"unitTestBoggle\test1.txt", "4x4 Test");
            if (noError)
                Console.WriteLine("All tests completed successfully.");
            else
                Console.WriteLine("Tests completed with errors.");
        }
    
        private static bool RunBoggleTest(string testFile, string testName)
        {
            Boggle b = new Boggle();

            Console.WriteLine("------------------------------ Running " + testName);
            Console.WriteLine("");
            var input = Regex.Split(System.IO.File.ReadAllText(testFile), @"\r\n\r\n\r\n").Where( s => s != String.Empty).ToArray();
            string board = input[0];
            List<string> answers = Regex.Split(input[1], @"\r\n\r\n").Where( s => s != String.Empty).ToList();
            string dictionary = input[2];

            var words = Regex.Split(dictionary, @"\r\n").Where( s => s != String.Empty);
            b.SetLegalWords(words);

            var ans = b.SolveBoard(4,4,board).ToList();
            ans.Sort();
            answers.Sort();
            for (int i = 0; i < ans.Count; i++) 
            {
                Console.WriteLine("{0} - {1}", answers[i], ans[i]);
                if (answers[i] != ans[i]) return false;
            }
            return true;
        }
    }
}
