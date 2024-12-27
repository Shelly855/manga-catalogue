using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogue
{
    internal class MangaStorage
    {
        // Dictionary to store mangas
        /* UPDATE HISTORY: Before, the dictionary was in Program.cs file.
         * After integrating the Encapsulation & Constructors topic, it was moved into its own class to 
         * improve data security & organisation. */
        private Dictionary<int, Manga> mangas;

        public MangaStorage()
        {

            /* UPDATE HISTORY: Before, the dictionary did not have a set initial capacity.
             * After incorporating the Profiling & Performance topic, an initial capacity of 30 was set
             * to reduce the need for frequent resizing, improving performance. */
            int estimatedCapacity = 30; // Estimate for amount of mangas
            mangas = new Dictionary<int, Manga>(estimatedCapacity)
            {
                { 1, new Manhua("Prime Minister in Disguise", 167, true) },
                { 2, new JapaneseManga("ReLIFE", 222, true) },
                { 3, new Manhwa("Omniscient Reader's Viewpoint", 230, false) },
                { 4, new Manhwa("Return of the Blossoming Blade", 143, false) },
                { 5, new Manhwa("The Perks of Being a Villainess", 63, false) },
                { 6, new Manhua("Mr.Y & Mr.J", 79, true) },
                { 7, new Manhwa("See You in My 19th Life", 114, true) },
                { 8, new Manhwa("This Witch of Mine", 62, true) },
                { 9, new Manhwa("Debut or Die!", 65, false) },
                { 10, new Manhwa("I am the Real One", 137, false) },
                { 11, new JapaneseManga("With One Day Left I'll Break All the Destruction Flags", 16, true) },
                { 12, new JapaneseManga("One-Week Family", 6, true) },
                { 13, new JapaneseManga("Otoge ni Trip Shita Ore", 15, true) },
                { 14, new Manhua("Heaven Official's Blessing", 101, false) },
                { 15, new Manhua("The Master of Diabolism", 259, true) }
            };
        }

        // Retrieve manga by ID
        public Manga GetManga(int id)
        {
            if (mangas.TryGetValue(id, out Manga manga))
            {
                return manga;
            }

            // If manga ID isn't found
            throw new KeyNotFoundException($"Manga with ID {id} does not exist.");
        }

        // Check if manga with given ID exists
        public bool ContainsManga(int id)
        {
            // Returns true if ID exists
            return mangas.ContainsKey(id);
        }

        public void AddNewManga(Manga manga, string filePath)
        {
            // Find the next available key
            int counter = 1;
            while (mangas.ContainsKey(counter))
            {
                counter++;
            }

            // Add the manga to the collection
            mangas.Add(counter, manga);

            SaveToBinaryFile(filePath);
        }

        public bool RemoveManga(int id)
        {
            return mangas.Remove(id);
        }

        public void UpdateManga(int id, Manga updatedManga)
        {
            // Check if manga with given ID exists
            if (mangas.ContainsKey(id))
            {
                mangas[id] = updatedManga;
            }
            else
            {
                throw new KeyNotFoundException("Manga ID not found.");
            }
        }

        public IEnumerable<Manga> SearchMangaByTitle(string title)
        {
            // Filter manga to where the title contains keyword
            return mangas.Values.Where(manga =>
                manga.GetTitle().Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        // Retrieve all mangas
        public IEnumerable<KeyValuePair<int, Manga>> GetAllMangas()
        {
            return mangas;
        }

        // Save mangas to binary file
        /* Before, mangas would not be saved.
         * After incorporating the Serialisation & Binary Files topic, mangas are now saved to a binary file
         * when the user exits the application, making sure their changes are retained. */
        public void SaveToBinaryFile(string filePath)
        {
            try
            {
                // To create or overwrite file
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var writer = new BinaryWriter(stream))
                {
                    // Write total number of mangas to file
                    writer.Write(mangas.Count);
                    foreach (var kvp in mangas)
                    {
                        // Write manga ID to file
                        writer.Write(kvp.Key);
                        Manga manga = kvp.Value;
                        writer.Write(manga.GetTitle());
                        writer.Write(manga.GetTypeName()); // type of manga (manga, manhwa, manhua)
                        writer.Write(manga.GetChapters());
                        writer.Write(manga.GetIsCompleted());
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        // Load mangas from binary file
        /* Before, mangas would not be loaded from a binary file.
         * After incorporating the Serialisation & Binary Files topic, mangas are now loaded from a binary file
         * when the user runs the application, restoring their previous changes. */
        public void LoadFromBinaryFile(string filePath)
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Using default mangas.");
                SaveToBinaryFile(filePath);
                return;
            }

            // Open binary file
            using (FileStream file = File.Open(filePath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(file))
            {
                // Clear manga dictionary
                mangas.Clear();

                // Read number of mangas in file
                int mangaCount = br.ReadInt32();

                // Read manga details
                for (int i = 0; i < mangaCount; i++)
                {
                    int id = br.ReadInt32(); // read ID
                    string title = br.ReadString();
                    string typeName = br.ReadString();
                    int chapters = br.ReadInt32();
                    bool isCompleted = br.ReadBoolean();

                    // Create manga object
                    Manga manga = typeName switch
                    {
                        "Chinese Manhua" => new Manhua(title, chapters, isCompleted),
                        "Korean Manhwa" => new Manhwa(title, chapters, isCompleted),
                        "Japanese Manga" => new JapaneseManga(title, chapters, isCompleted),
                        _ => throw new InvalidDataException($"Unknown manga type: {typeName}")
                    };

                    // Add manga to dictionary
                    mangas.Add(id, manga);
                }
            }
        }
    }
}
