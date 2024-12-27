using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogue
{
    /* Before, there were no subclasses of JapaneseManga, Manhwa, & Manhua. Instead, they were stored in a 
     * 'type' property in the Manga class.
     * After incorporating the Inheritance & Polymorphism topic, the subclasses were created, inheriting from the
     * Manga class. */
    internal class JapaneseManga : Manga
    {
        public JapaneseManga(string title, int chapters, bool isCompleted)
            : base(title, chapters, isCompleted) { }

        // Override keyword means that this method is ​overriding the GetTypeName() method in the ​base class Manga.​
        public override string GetTypeName()
        {
            return "Japanese Manga";
        }
    }
}
