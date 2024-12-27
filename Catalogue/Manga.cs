using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogue
{
    internal class Manga
    {
        /* Before, the manga properties were public.
         * After incorporating the Encapsulation & Constructors topic, they were made private to improve 
         * data security and control access. */
        private string Title { get; set; }
        private int Chapters { get; set; }
        private bool IsCompleted { get; set; }

        public Manga(string title, int chapters, bool isCompleted)
        {
            Title = title;
            Chapters = chapters;
            IsCompleted = isCompleted;
        }

        // Property to display status as completed or incompleted
        public string Status
        {
            get
            {
                return IsCompleted ? "Status: Completed" : "Status: Incompleted";
            }
        }

        // Virtual keyword means that this method is meant to be overriden by subclasses like Manhwa
        public virtual string GetTypeName()
        {
            return "General Manga";
        }

        // Method to display mangas
        public void Print(int id)
        {
            Console.WriteLine(@$"ID: {id}
Title: {Title}
Type: {GetTypeName()}
Chapters: {Chapters}
{Status}");
        }

        public bool GetIsCompleted()
        {
            return IsCompleted;
        }

        public int GetChapters()
        {
            return Chapters;
        }

        public void SetIsCompleted(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }

        public bool HasLessChaptersThan(int maxChapters)
        {
            return Chapters < maxChapters;
        }

        public string GetTitle()
        {
            return Title;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void SetChapters(int chapters)
        {
            Chapters = chapters;
        }
    }
}
