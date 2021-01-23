using System.Collections.Generic;
using System.Linq;

namespace Conmo.Utils {
    public class StringManipulations {
        
         public static string AddSpacesBeetween(string text) {
            string rez = "";

            for (int i = 0; i < text.Length; i++) {
                if (text[i] >= 'A' && text[i] <= 'Z' && rez.Length > 0)
                    rez += " ";
                rez += text[i];
            }

            return rez;
         }
                        
        public static List<string> AddSpacesBeetween(List<string> text) {
            return text.Select(item => item = AddSpacesBeetween(item)).ToList();
        }
                
        public static string RemoveSpaces(string text) {
                    string rez = "";
                    foreach (var chr in text) {
                        if (!char.IsWhiteSpace(chr))
                            rez += chr;
                    }
                    return rez;
                }
                        
    }
}