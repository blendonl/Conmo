using System;

namespace Conmo.Utils {

    public static class CommonViews {

       public static int Menu( string[] choices) {

            for (int i = 0; i < choices.Length; i++)
                Console.WriteLine($"Press {i} to {choices[i]}");
            Console.WriteLine();

            Console.Write("Choose: ");
            int choice;

            if (!int.TryParse(Console.ReadLine(), out choice))
                choice = -1;

            return choice;
        }

       public static void Title(string title) {

            Console.Clear();
            Console.WriteLine(title);
            Console.WriteLine();

        }

       public static T LoopInput<T>(string title, int length) {
           
           T value; 
           try { 
               value = (T) Convert.ChangeType(Input(title, length), typeof(T));
               
               if (value.ToString().Length < length) {
                 return LoopInput<T>(title, length); 
               } 
           }
           catch (Exception) {
             Console.WriteLine($"Please enter a valid value");
             return LoopInput<T>(title, length); 
           }
           return value;
       }
         
         
        public static string Input(string inputType, int length) {
     
             Console.Write($"{inputType}: ");
             string s = Console.ReadLine();
             if (String.IsNullOrEmpty(s) || s.Length < length) {
                 Console.WriteLine($"Please enter a valid {length} chars string");
                 return Input(inputType, length);
             }
             return s; 
        }
    }
}

