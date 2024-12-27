using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogue
{
    internal class Manhwa : Manga
    {
        public Manhwa(string title, int chapters, bool isCompleted)
            : base(title, chapters, isCompleted) { }

        // Override keyword means that this method is ​overriding the GetTypeName() method in the ​base class Manga.​
        public override string GetTypeName()
        {
            return "Korean Manhwa";
        }
    }
}
