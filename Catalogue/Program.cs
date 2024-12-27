using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Diagnostics;
using System.Text;

namespace Catalogue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "mangaData.dat";

            // Create instance of MangaStorage class
            MangaStorage mangaStorage = new MangaStorage();

            // Check if binary file exists
            // POSSIBLE IMPROVEMENT: Move checking the file existence to its own method.
            if (File.Exists(filePath))
            {
                // Load data from binary file
                /* POSSIBLE IMPROVEMENT: Provide more user feedback when loading/saving data, like success messages. */
                mangaStorage.LoadFromBinaryFile(filePath);
            }
            else
            {
                // Use default mangas
                mangaStorage.SaveToBinaryFile(filePath);
            }

            // Check for arguments in command line
            if (args.Length > 0)
            {
                CommandLine(args, mangaStorage);
            }
            else
            {
                Console.WriteLine("Welcome to the Manga Catalogue!");
                Console.WriteLine(); // blank line for spacing

                // Display main menu
                ShowMenu(mangaStorage);
            }
        }

        // Show number of processors on computer
        /* After incorporating the Using Multiple Processors topic, users can now select a choice from
         * the main menu to view the number of processors on their computer. */
        static void ShowProcessorCount(MangaStorage mangaStorage)
        {
            int processors = Environment.ProcessorCount;
            Console.WriteLine($"This computer has {processors} processors available.");
            Console.WriteLine();
            ReturnToMainMenu(mangaStorage);
        }

        /* POSSIBLE IMPROVEMENT: Cache the console colour settings to improve performance, instead of
         * setting & resetting the colour each time. */
        // Function for changing colours for section titles (cyan)
        static void PrintSectionTitle(string title, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.WriteLine(new string('-', 50)); // line of dashes
            Console.ForegroundColor = color;
            Console.WriteLine("-- " + title + " --");
            Console.ResetColor();
        }

        // Colour for error messages (red)
        static void PrintErrorMessage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
            Console.WriteLine();
        }

        // Colour for system messages (dark grey)
        static void PrintSystemMessage(string systemMessage)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(systemMessage);
            Console.ResetColor();
        }

        static void ReturnToDisplayMangaMenu(MangaStorage mangaStorage, string message)
        {
            PrintSystemMessage(message);
            PrintSystemMessage("Directing you back to the display manga menu.");
            Console.WriteLine();
            DisplayManga(mangaStorage);
        }

        static void ReturnToMainMenu(MangaStorage mangaStorage)
        {
            PrintSystemMessage("Directing you back to the main menu.");
            Console.WriteLine();
            ShowMenu(mangaStorage);
        }

        static int GetValidUserChoice(int firstChoice, int lastChoice, string prompt, Action Error)
        {
            /* POSSIBLE IMPROVEMENT: Add retry limit for invalid inputs to prevent infinite loops. */
            while (true) // Loop until valid input is provided
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine();
                
                // Check if input can be parsed to integer and is within the needed range
                if (int.TryParse(userInput, out int choice) && choice >= firstChoice && choice <= lastChoice)
                {
                    return choice;
                }

                // If invalid input -> error message
                Error?.Invoke();
            }
        }

        // If no mangas in dictionary
        static void NoMangasExist(MangaStorage mangaStorage, string action)
        {
            Console.WriteLine($"There are no mangas available to {action}.");
            ReturnToMainMenu(mangaStorage);
        }

        // The first menu the user sees
        static void ShowMenu(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Main Menu");

            // Display available choices user can select
            DisplayMenuOptions();

            int menuChoice = GetMenuChoice();
            MenuChoiceNavigation(menuChoice, mangaStorage);
        }

        static void DisplayMenuOptions()
        {
            Console.WriteLine(@"
Please pick an option using its number:
1: Display mangas
2: Add a new manga
3: Remove a manga
4: Update a manga
5: Search for a manga
6: Show number of processors
7: Exit");
            Console.WriteLine();
        }

        // Getting user input and parsing it as integer
        static int GetMenuChoice()
        {
            return GetValidUserChoice(1, 7, "Your choice (enter a number from 1 to 7): ", () =>
            {
                PrintErrorMessage("Invalid option. Please enter a number from 1-7.");
                Console.WriteLine();
            });
        }

        // Navigate to other methods based on menuChoice(1-7) provided by user
        /* POSSIBLE IMPROVEMENT: Using a dictionary instead of switch statements to make adding menu choices more efficient. */
        static void MenuChoiceNavigation(int menuChoice, MangaStorage mangaStorage)
        {
            switch (menuChoice)
            {
                case 1:
                    DisplayManga(mangaStorage);
                    break;
                case 2:
                    AddNewManga(mangaStorage);
                    break;
                case 3:
                    RemoveManga(mangaStorage);
                    break;
                case 4:
                    UpdateManga(mangaStorage);
                    break;
                case 5:
                    SearchForManga(mangaStorage);
                    break;
                case 6:
                    ShowProcessorCount(mangaStorage);
                    break;
                case 7:

                    /* POSSIBLE IMPROVEMENT: Save manga to binary file whenever the user makes a change, 
                     * to make sure their changes are saved even if they exit by closing the window 
                     * instead of using the main menu. */
                    mangaStorage.SaveToBinaryFile("mangaData.dat");
                    Console.WriteLine("Exiting the Manga Catalogue.");

                    /* POSSIBLE IMPROVEMENT: Asking the user if they are sure they want to exit before exiting. */
                    Environment.Exit(0); // exit the program
                    return;
                default:
                    PrintErrorMessage("Invalid option. Please enter a number from 1-6.");
                    ShowMenu(mangaStorage); // go back to main menu
                    break;
            }
        }

        static void DisplayManga(MangaStorage mangaStorage)
        {
            var mangas = mangaStorage.GetAllMangas();

            // Check if there are any mangas
            if (!mangas.Any())
            {
                /* POSSIBLE IMPROVEMENT: If no mangas are available, ask the user if they would like to add a new manga. */
                NoMangasExist(mangaStorage, "display");
            }
            else
            {
                ShowFilterMenu();
                HandleFilterChoice(mangaStorage);
            }

            ShowMenu(mangaStorage);
        }

        // Menu for filtering the manga display
        static void ShowFilterMenu()
        {
            PrintSectionTitle("Display Manga");
            Console.WriteLine(@"
Please pick a filter using its number:
1: Display all mangas
2: Filter by status
3: Filter by type
4: Filter by number of chapters
5: Filter by alphabetical order
6: Go back to main menu");

            Console.WriteLine();
        }

        // Getting user input and parsing it as integer
        static void HandleFilterChoice(MangaStorage mangaStorage)
        {
            int userChoice = GetValidUserChoice(1, 6, "Your choice: ", () =>
            {
                PrintErrorMessage("Invalid option. Please enter a number from 1-6.");
                Console.WriteLine();
            });

            FilterChoiceNavigation(userChoice, mangaStorage);
        }

        // Navigate to other methods based on filterChoice(1-6) provided by user
        static void FilterChoiceNavigation(int filterChoice, MangaStorage mangaStorage)
        {
            switch (filterChoice)
            {
                case 1:
                    DisplayAllManga(mangaStorage);
                    break;
                case 2:
                    FilterByStatus(mangaStorage);
                    break;
                case 3:
                    FilterByType(mangaStorage);
                    break;
                case 4:
                    FilterByChapters(mangaStorage);
                    break;
                case 5:
                    AlphabeticalOrder(mangaStorage);
                    break;
                case 6:
                    ShowMenu(mangaStorage);
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please enter a number from 1-6.");
                    DisplayManga(mangaStorage);
                    break;
            }
        }

        // Display every single manga
        static void DisplayAllManga(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Display All Manga");
            Console.WriteLine();

            var allMangas = mangaStorage.GetAllMangas();
            DisplayFilteredManga(allMangas, mangaStorage, "Finished displaying all mangas.");
        }


        // Filter by completed or incomplete
        static void FilterByStatus(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Filter by Status");
            ShowStatusFilterMenu();
            HandleStatusChoice(mangaStorage);
        }

        static void ShowStatusFilterMenu()
        {
            Console.WriteLine(@"
Please pick a status using its number:
1: Incomplete
2: Completed
3: Go back to Display Manga menu");

            Console.WriteLine();
        }

        static void HandleStatusChoice(MangaStorage mangaStorage)
        {
            int statusChoice = GetValidUserChoice(1, 3, "Your choice: ", () =>
            {
                PrintErrorMessage("Invalid option. Please enter a number from 1-3.");
                Console.WriteLine();
            });

            StatusChoice(statusChoice, mangaStorage);
        }

        // Navigate to other methods based on statusChoice(1-3) provided by user
        static void StatusChoice(int statusChoice, MangaStorage mangaStorage)
        {
            switch (statusChoice)
            {
                case 1:
                    ShowIncomplete(mangaStorage);
                    break;
                case 2:
                    ShowCompleted(mangaStorage);
                    break;
                case 3:
                    DisplayManga(mangaStorage);
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please enter a number from 1-3.");
                    FilterByStatus(mangaStorage);
                    break;
            }
        }

        // Display all incomplete manga
        static void ShowIncomplete(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Incomplete Manga", ConsoleColor.DarkCyan);
            Console.WriteLine();

            // Filter manga collection to include only incomplete ones
            var filterMangaIncomplete = mangaStorage.GetAllMangas()
                                                     .Where(kvp => !kvp.Value.GetIsCompleted());

            DisplayFilteredManga(filterMangaIncomplete, mangaStorage, "Finished displaying incomplete manga.");
        }

        // Display all completed manga
        static void ShowCompleted(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Completed Manga", ConsoleColor.DarkCyan);
            Console.WriteLine();

            // Filter manga collection to include only completed ones
            var filterMangaCompleted = mangaStorage.GetAllMangas()
                                                   .Where(kvp => kvp.Value.GetIsCompleted());

            DisplayFilteredManga(filterMangaCompleted, mangaStorage, "Finished displaying completed manga.");
        }

        // Filter by manga type (manhwa, manhua, manga)
        static void FilterByType(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Filter by Type");
            ShowTypeMenu();
            HandleTypeChoice(mangaStorage);
        }

        static void ShowTypeMenu()
        {
            Console.WriteLine(@"
Please pick a type using its number:
1: Manhua
2: Manhwa
3: Manga
4: Go back to Display Manga menu");

            Console.WriteLine();
        }

        static void HandleTypeChoice(MangaStorage mangaStorage)
        {
            int typeChoice = GetValidUserChoice(1, 4, "Your choice: ", () =>
            {
                PrintErrorMessage("Invalid option. Please enter a number from 1-4.");
                Console.WriteLine();
            });

            TypeChoice(typeChoice, mangaStorage);
        }

        // Navigate to other methods based on typeChoice(1-4) provided by user
        static void TypeChoice(int typeChoice, MangaStorage mangaStorage)
        {
            switch (typeChoice)
            {
                case 1:
                    ShowManhua(mangaStorage);
                    break;
                case 2:
                    ShowManhwa(mangaStorage);
                    break;
                case 3:
                    ShowManga(mangaStorage);
                    break;
                case 4:
                    DisplayManga(mangaStorage);
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please enter a number from 1-4.");
                    FilterByType(mangaStorage);
                    break;
            }
        }

        /* POSSIBLE IMPROVEMENT: If there are no filtered mangas, inform the user. */
        /* POSSIBLE IMPROVEMENT: Refactor repeated logic into a single method to prevent code duplication. */
        static void ShowManhua(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Manhua", ConsoleColor.DarkCyan);
            Console.WriteLine();

            // Filter manga collection to include only manhua
            var filterManhua = mangaStorage.GetAllMangas()
                                           .Where(kvp => kvp.Value.GetTypeName() == "Chinese Manhua");

            DisplayFilteredManga(filterManhua, mangaStorage, "Finished displaying manhua.");
        }

        static void ShowManhwa(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Manhwa", ConsoleColor.DarkCyan);
            Console.WriteLine();

            var filterManhwa = mangaStorage.GetAllMangas()
                                           .Where(kvp => kvp.Value.GetTypeName() == "Korean Manhwa");

            DisplayFilteredManga(filterManhwa, mangaStorage, "Finished displaying manhwa.");
        }

        static void ShowManga(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Manga", ConsoleColor.DarkCyan);
            Console.WriteLine();

            var filterManga = mangaStorage.GetAllMangas()
                                          .Where(kvp => kvp.Value.GetTypeName() == "Japanese Manga");

            DisplayFilteredManga(filterManga, mangaStorage, "Finished displaying manga.");
        }

        static void DisplayFilteredManga(IEnumerable<KeyValuePair<int, Manga>> filteredManga, MangaStorage mangaStorage, string completionMessage)
        {
            foreach (var kvp in filteredManga)
            {
                int mangaId = kvp.Key;
                Manga manga = kvp.Value;
                manga.Print(mangaId); // call the print method of manga class
                Console.WriteLine();
            }

            ReturnToDisplayMangaMenu(mangaStorage, completionMessage);
        }

        // POSSIBLE IMPROVEMENT: Let user filter mangas that have more than or equal to chosen amount of chapters.
        static void FilterByChapters(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Filter by Chapters");
            Console.WriteLine();

            int chapterChoice = GetChapterChoice();
            ChapterChoice(chapterChoice, mangaStorage);
        }

        // Get valid chapter number from user
        static int GetChapterChoice()
        {
            return GetValidUserChoice(0, int.MaxValue, "Please enter a number of chapters: ", () =>
            {
                PrintErrorMessage("Invalid input. Please enter a valid number (0 or more).");
                Console.WriteLine();
            });
        }

        // Filter manga collection to include only manga with chapters less than chapterChoice
        /* UPDATE HISTORY: Before, there was only the standard foreach loop.
         * After incorporating the Using Multiple Processors topic, a stopwatch & parallel foreach loop was added to
         * show a comparison between the 2 loops. */
        static void ChapterChoice(int chapterChoice, MangaStorage mangaStorage)
        {
            PrintSectionTitle($"Manga With Less Than {chapterChoice} Chapters", ConsoleColor.DarkCyan);
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();

            // Standard foreach loop: faster since this application has a small dataset
            // POSSIBLE IMPROVEMENT: Move filtering logic into separate method to prevent code duplication.
            var standardFilteredManga = new List<KeyValuePair<int, Manga>>();
            foreach (var kvp in mangaStorage.GetAllMangas())
            {
                // Check if manga has less chapters than specified chapter number
                if (kvp.Value.HasLessChaptersThan(chapterChoice))
                {
                    // Add manga to filtered list
                    standardFilteredManga.Add(kvp);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Standard foreach filtering completed in {stopwatch.ElapsedMilliseconds} ms.");
            Console.WriteLine();

            stopwatch.Restart();

            // Parallel.ForEach loop: slower due to thread management overhead
            var parallelFilteredManga = new ConcurrentBag<KeyValuePair<int, Manga>>();
            Parallel.ForEach(mangaStorage.GetAllMangas(), kvp =>
            {
                if (kvp.Value.HasLessChaptersThan(chapterChoice))
                {
                    parallelFilteredManga.Add(kvp);
                }
            });

            stopwatch.Stop();
            Console.WriteLine($"Parallel.ForEach filtering completed in {stopwatch.ElapsedMilliseconds} ms.");
            Console.WriteLine();

            DisplayFilteredManga(parallelFilteredManga, mangaStorage, $"Finished displaying manga with less than {chapterChoice} chapters.");
        }


        // POSSIBLE IMPROVEMENT: Let user sort mangas by descending alphabetical order as well
        static void AlphabeticalOrder(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Filter By Alphabetical Order");
            Console.WriteLine();

            // Sort the manga collection in ascending order by title
            var sortedMangas = mangaStorage.GetAllMangas()
                                           .OrderBy(kvp => kvp.Value.GetTitle());

            DisplayFilteredManga(sortedMangas, mangaStorage, "Finished displaying mangas in alphabetical order.");
        }

        static void AddNewManga(MangaStorage mangaStorage)
        {
            string filePath = "mangaData.dat";

            /* UPDATE HISTORY: Before, try catch was not used.
             * After incorporating the Robustness topic, this method now handles potential errors & provide
             * provide error messages to improve robustness. */
            try
            {
                PrintSectionTitle("Add a New Manga");
                Console.WriteLine();

                string title = GetMangaTitle();
                string type = GetMangaType();
                int chapters = GetNumberOfChapters();
                bool isCompleted = GetCompletionStatus();

                // Create new manga object
                Manga newManga = CreateManga(type, title, chapters, isCompleted);

                // Add manga to manga storage and save it to binary file
                mangaStorage.AddNewManga(newManga, filePath);

                Console.WriteLine($"Manga '{title}' has been added.");
                Console.WriteLine();

                // Question if user wants to add another manga
                QuestionAnotherAdd(mangaStorage);
            }
            catch (Exception ex)
            {
                PrintErrorMessage($"Error: {ex.Message}");
            }
        }

        // POSSIBLE IMPROVEMENT: Add validation for user input (e.g. check if title is empty)
        static string GetMangaTitle()
        {
            Console.Write("Enter a title: ");
            return Console.ReadLine();
        }

        static string GetMangaType()
        {
            while (true)
            {
                Console.Write("Enter the type (Manga/Manhua/Manhwa): ");
                string type = Console.ReadLine().Trim().ToLower();

                if (type == "manga" || type == "manhua" || type == "manhwa")
                {
                    return type;
                }
                else
                {
                    PrintErrorMessage("Invalid type. Please enter 'Manga', 'Manhua', or 'Manhwa'.");
                }
            }
        }

        static int GetNumberOfChapters()
        {
            return GetValidUserChoice(0, int.MaxValue, "Enter the number of chapters: ", () =>
            {
                PrintErrorMessage("Invalid input. Please enter a valid number of chapters (0 or more).");
                Console.WriteLine();
            });
        }

        static bool GetCompletionStatus()
        {
            while (true)
            {
                Console.Write("Is the manga completed? Enter yes or no: ");
                string isCompletedInput = Console.ReadLine().ToLower().Trim();

                if (isCompletedInput == "yes")
                {
                    return true;
                }
                else if (isCompletedInput == "no")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine();
                    PrintErrorMessage("Invalid input. Please enter yes or no.");
                }
            }
        }

        // Create manga object
        static Manga CreateManga(string type, string title, int chapters, bool isCompleted)
        {
            switch (type.ToLower()) // convert manga type to lowercase
            {
                case "manga":
                    return new JapaneseManga(title, chapters, isCompleted);
                case "manhua":
                    return new Manhua(title, chapters, isCompleted);
                case "manhwa":
                    return new Manhwa(title, chapters, isCompleted);
                default:
                    throw new Exception("Invalid type.");
            }
        }

        static void QuestionAnotherAdd(MangaStorage mangaStorage)
        {
            Console.Write("Would you like to add another manga? Enter yes or no: ");
            string addMangaChoice = Console.ReadLine();

            if (addMangaChoice.ToLower().Trim() == "yes")
            {
                Console.WriteLine();
                AddNewManga(mangaStorage);
            }
            else
            {
                ReturnToMainMenu(mangaStorage);
            }
        }

        static void RemoveManga(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Remove a Manga");
            Console.WriteLine();

            var mangas = mangaStorage.GetAllMangas();

            // Check if there are any mangas
            bool hasMangas = false;
            foreach (var kvp in mangas)
            {
                hasMangas = true;
                break;
            }

            if (!hasMangas)
            {
                NoMangasExist(mangaStorage, "remove");
                return;
            }

            DisplayAvailableMangas(mangas, "Available mangas to remove:");
            HandleMangaRemoval(mangaStorage);
        }

        // Display available mangas to remove/update
        static void DisplayAvailableMangas(IEnumerable<KeyValuePair<int, Manga>> mangas, string title)
        {
            Console.WriteLine($"{title}");
            Console.WriteLine();

            // Table header
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine(" ID  | Title                                 ");
            Console.WriteLine("-------------------------------------------------");

            foreach (var kvp in mangas)
            {
                int id = kvp.Key;
                string titleFormatted = kvp.Value.GetTitle();

                // Cut off long titles
                // POSSIBLE IMPROVEMENT: Provide a way for user to view full titles
                if (titleFormatted.Length > 35)
                {
                    titleFormatted = titleFormatted.Substring(0, 32) + "...";
                }

                Console.WriteLine($"{id.ToString().PadLeft(3)}  | {titleFormatted.PadRight(35)}");
            }

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine();
        }

        // POSSIBLE IMPROVEMENT: Moving some parts of code to separate functions to improve readability
        static void HandleMangaRemoval(MangaStorage mangaStorage)
        {
            while (true)
            {
                string removeMangaIdString = GetMangaInputOrGoBack(
                    "Enter the ID of the manga to remove, or enter B to go back to the main menu: ",
                    mangaStorage
                );

                if (int.TryParse(removeMangaIdString, out int removeMangaId) && mangaStorage.ContainsManga(removeMangaId))
                {
                    Manga removedManga = mangaStorage.GetManga(removeMangaId);

                    if (mangaStorage.RemoveManga(removeMangaId))
                    {
                        Console.WriteLine($"Manga '{removedManga.GetTitle()}' with ID {removeMangaId} has been removed.");
                        Console.WriteLine();

                        if (!QuestionAnotherRemoval())
                        {
                            ReturnToMainMenu(mangaStorage);
                            return;
                        }
                    }
                }
                else
                {
                    PrintErrorMessage("Invalid ID. Please try again or enter B to go back to the main menu.");
                }
            }
        }

        static string GetMangaInputOrGoBack(string prompt, MangaStorage mangaStorage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.ToUpper().Trim();

                // See if user wants to return to main menu
                if (input == "B")
                {
                    Console.WriteLine();
                    ShowMenu(mangaStorage);
                    return null;
                }

                return input;
            }
        }

        static bool QuestionAnotherRemoval()
        {
            Console.Write("Would you like to remove another manga? Enter yes or no: ");
            string removeMangaChoice = Console.ReadLine()?.ToLower().Trim();

            // Return true if the user wants to remove another manga
            return removeMangaChoice == "yes";
        }

        /* Before, LINQ's Any() was used to check for manga existence.
         * After incorporating the Profiling & Performance topic, LINQ's Any() was replaced with a manual loop.
         * Performance improvements are more noticeable with larger datasets, as LINQ's overhead is avoided. */
        static void UpdateManga(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Update Manga");
            Console.WriteLine();

            var mangas = mangaStorage.GetAllMangas();

            bool hasMangas = false;
            foreach (var kvp in mangas)
            {
                hasMangas = true;
                break;
            }

            if (!hasMangas)
            {
                NoMangasExist(mangaStorage, "update");
                return;
            }

            DisplayAvailableMangas(mangas, "Available mangas to update:");
            HandleMangaUpdate(mangaStorage);
        }

        static void HandleMangaUpdate(MangaStorage mangaStorage)
        {
            while (true)
            {
                string mangaUpdateIDString = GetMangaInputOrGoBack(
                    "Enter the ID of the manga to update, or enter B to go back to the main menu: ",
                    mangaStorage
                );

                if (int.TryParse(mangaUpdateIDString, out int mangaID) && mangaStorage.ContainsManga(mangaID))
                {
                    UpdateChosenManga(mangaID, mangaStorage);

                    if (!QuestionAnotherUpdate())
                    {
                        ReturnToMainMenu(mangaStorage);
                        return;
                    }
                }
                else
                {
                    PrintErrorMessage("Invalid ID. Please try again or enter B to go back to the main menu.");
                }
            }
        }


        // POSSIBLE IMPROVEMENT: Validation
        static void UpdateChosenManga(int mangaID, MangaStorage mangaStorage)
        {
            Manga chosenManga = mangaStorage.GetManga(mangaID);
            Console.WriteLine($"Updating details for: {chosenManga.GetTitle()}");
            Console.WriteLine();

            UpdateTitle(chosenManga);
            UpdateType(chosenManga, mangaID, mangaStorage);
            UpdateChapters(chosenManga);
            UpdateCompletionStatus(chosenManga);

            Console.WriteLine($"Manga '{chosenManga.GetTitle()}' has been updated.");
        }


        static void UpdateTitle(Manga chosenManga)
        {
            Console.Write("Enter a new title or leave empty to keep unchanged: ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTitle))
            {
                chosenManga.SetTitle(newTitle);
            }
        }

        static void UpdateType(Manga chosenManga, int mangaID, MangaStorage mangaStorage)
        {
            Console.Write("Enter a new type (Manga/Manhua/Manhwa) or leave empty to keep unchanged: ");
            string newType = Console.ReadLine().Trim();

            if (!string.IsNullOrEmpty(newType))
            {
                Manga updatedManga = null;

                switch (newType.ToLower())
                {
                    /* POSSIBLE IMPROVEMENT: Move creation of new manga object into a separate function
                     * to avoid code repetition for each manga type. */
                    case "manga":
                        updatedManga = new JapaneseManga(
                            chosenManga.GetTitle(),
                            chosenManga.GetChapters(),
                            chosenManga.GetIsCompleted()
                        );
                        break;
                    case "manhua":
                        updatedManga = new Manhua(
                            chosenManga.GetTitle(),
                            chosenManga.GetChapters(),
                            chosenManga.GetIsCompleted()
                        );
                        break;
                    case "manhwa":
                        updatedManga = new Manhwa(
                            chosenManga.GetTitle(),
                            chosenManga.GetChapters(),
                            chosenManga.GetIsCompleted()
                        );
                        break;
                    default:
                        PrintErrorMessage("Invalid type entered. No changes made to the type.");
                        break;
                }

                if (updatedManga != null)
                {
                    mangaStorage.UpdateManga(mangaID, updatedManga);
                    chosenManga = updatedManga;
                }
            }
        }

        static void UpdateChapters(Manga chosenManga)
        {
            Console.Write("Enter the new number of chapters or leave empty to keep unchanged: ");
            string newChaptersInput = Console.ReadLine().Trim();

            if (int.TryParse(newChaptersInput, out int newChapters))
            {
                chosenManga.SetChapters(newChapters);
            }
        }

        static void UpdateCompletionStatus(Manga chosenManga)
        {
            Console.Write("Is the manga completed? (yes/no) or leave empty to keep unchanged: ");
            string isCompletedInput = Console.ReadLine().ToLower().Trim();

            if (isCompletedInput == "yes")
            {
                chosenManga.SetIsCompleted(true);
            }
            else if (isCompletedInput == "no")
            {
                chosenManga.SetIsCompleted(false);
            }
        }

        static bool QuestionAnotherUpdate()
        {
            Console.WriteLine();
            Console.Write("Would you like to update another manga? Enter yes or no: ");
            string updateMangaChoice = Console.ReadLine()?.ToLower().Trim();

            // Return true if the user wants to update another manga
            return updateMangaChoice == "yes";
        }

        static void SearchForManga(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Search For Manga");
            Console.WriteLine(@"
Please pick a choice using its number:
1: Search by ID
2: Search by Title
3: Go back to Main Menu");

            Console.WriteLine();

            int searchChoice = GetValidUserChoice(1, 3, "Your choice: ", () =>
            {
                PrintErrorMessage("Invalid input. Please enter a number from 1-3.");
                Console.WriteLine();
            });

            SearchChoice(searchChoice, mangaStorage);
        }

        // Navigate to other methods based on searchChoice(1-3) provided by user
        static void SearchChoice(int searchChoice, MangaStorage mangaStorage)
        {
            switch (searchChoice)
            {
                case 1:
                    SearchByID(mangaStorage);
                    break;
                case 2:
                    SearchByTitle(mangaStorage);
                    break;
                case 3:
                    ShowMenu(mangaStorage);
                    break;
                default:
                    PrintErrorMessage("Invalid option. Please enter a number from 1-3.");
                    SearchForManga(mangaStorage);
                    break;
            }
        }

        static void SearchByID(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Search by Manga ID");
            Console.WriteLine();

            while (true)
            {
                string mangaIDString = GetMangaInputOrGoBack(
                    "Enter the ID of the manga to search, or enter B to go back to the main menu: ",
                    mangaStorage
                );

                if (int.TryParse(mangaIDString, out int mangaID))
                {
                    Console.WriteLine();
                    DisplayMangaIDSearch(mangaID, mangaStorage);

                    if (!QuestionAnotherSearch())
                    {
                        ReturnToMainMenu(mangaStorage);
                        return;
                    }
                }
                else
                {
                    PrintErrorMessage("Invalid ID. Please try again or enter B to go back to the main menu.");
                }
            }
        }

        // Displaying the details of the manga that was searched for
        static void DisplayMangaIDSearch(int mangaID, MangaStorage mangaStorage)
        {
            try
            {
                Manga manga = mangaStorage.GetManga(mangaID);
                manga.Print(mangaID);
                Console.WriteLine();
            }
            catch (KeyNotFoundException)
            {
                PrintErrorMessage("Manga not found.");
            }
        }

        /* Before, caching was not used in the SearchByTitle method.
         * After applying the Profiling & Performance topic, a cache was incorporated to store previous
         * search results, potentially improving performance by avoiding redundant searches for the same title. 
         * Caching will be more effective as the dataset grows in size or when there are repetitive searches. */
        static Dictionary<string, List<Manga>> searchCache = new Dictionary<string, List<Manga>>(StringComparer.OrdinalIgnoreCase);

        static void SearchByTitle(MangaStorage mangaStorage)
        {
            PrintSectionTitle("Search by Manga Title");
            Console.WriteLine();

            while (true)
            {
                string mangaTitle = GetMangaInputOrGoBack(
                    "Enter the title of the manga to search, or enter B to go back to the main menu: ",
                    mangaStorage
                );

                // Check if manga title is in cache
                if (!searchCache.TryGetValue(mangaTitle, out var foundMangas))
                {
                    // If not in cache, then search for the manga
                    foundMangas = mangaStorage.SearchMangaByTitle(mangaTitle).ToList();
                    searchCache[mangaTitle] = foundMangas;
                }

                if (foundMangas.Any())
                {
                    Console.WriteLine();
                    DisplaySearchResults(foundMangas, mangaStorage);

                    if (!QuestionAnotherSearch())
                    {
                        ReturnToMainMenu(mangaStorage);
                        return;
                    }
                }
                else
                {
                    PrintErrorMessage("Manga not found. Please try again or enter B to go back to the main menu.");
                }
            }
        }

        static void DisplaySearchResults(IEnumerable<Manga> foundMangas, MangaStorage mangaStorage)
        {
            foreach (var manga in foundMangas)
            {
                int mangaId = mangaStorage.GetAllMangas()
                                          .First(kvp => kvp.Value == manga).Key;

                manga.Print(mangaId);
                Console.WriteLine();
            }
        }

        static bool QuestionAnotherSearch()
        {
            Console.Write("Would you like to search for another manga? Enter yes or no: ");
            string searchMangaChoice = Console.ReadLine()?.ToLower().Trim();
            Console.WriteLine();

            return searchMangaChoice == "yes";
        }


        /* UPDATE HISTORY: Before, there were no command line methods.
         * After incorporating the Command Line topic, the application now allows users to 
         * interact with the catalogue directly from the command line. */
        static void CommandLine(string[] args, MangaStorage mangaStorage)
        {
            try
            {
                if (!ValidateCommandArguments(args)) return;

                // Convert first argument to lowercase
                string command = args[0].ToLower();

                HandleCommand(command, args, mangaStorage);
            }
            catch (Exception ex)
            {
                PrintErrorMessage($"Error: {ex.Message}");
            }
        }

        static bool ValidateCommandArguments(string[] args)
        {
            // Check if there are any arguments
            if (args.Length == 0)
            {
                PrintErrorMessage("No command provided. Please use: --list, --add, --search");
                return false;
            }
            return true;
        }

        static void HandleCommand(string command, string[] args, MangaStorage mangaStorage)
        {
            switch (command)
            {
                case "--list":
                    DisplayMangaCommand(args, mangaStorage);
                    return;
                case "--add":
                    AddMangaCommand(args, mangaStorage);
                    return;
                case "--search":
                    SearchMangaCommand(args, mangaStorage);
                    return;
                case "--help":
                    HelpCommand(args);
                    return;
                default:
                    PrintErrorMessage("Unknown command. Please use: --list, --add, --search, --help");
                    return;
            }
        }

        /* UPDATE HISTORY: Before, the HelpCommand method used many Console.WriteLine() calls, which
         * increased memory usage and was inefficient.
         * After incorporating the Profiling & Performance topic, StringBuilder was used to reduce memory
         * allocation & improve performance, as StringBuilder appends each line without creating new strings. */
        static void HelpCommand(string[] args)
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine()
                       .AppendLine("Available commands:")
                       .AppendLine()
                       .AppendLine("--list")
                       .AppendLine("Use: List all mangas in the catalogue.")
                       .AppendLine()
                       .AppendLine("--add <title> <type> <chapters> <isCompleted>")
                       .AppendLine("Use: Add a new manga to the catalogue.")
                       .AppendLine("Example: --add \"Odd Girl Out\" manga 200 false")
                       .AppendLine()
                       .AppendLine("--search <title>")
                       .AppendLine("Use: Search for a manga by title.")
                       .AppendLine("Example: --search \"Omniscient Reader's Viewpoint\"")
                       .AppendLine()
                       .AppendLine("--help")
                       .AppendLine("Use: Display this help information.")
                       .AppendLine();

            Console.WriteLine(helpMessage.ToString());
        }

        static void DisplayMangaCommand(string[] args, MangaStorage mangaStorage)
        {
            Console.WriteLine();
            var allMangas = mangaStorage.GetAllMangas();
            foreach (var kvp in allMangas)
            {
                int mangaId = kvp.Key;
                Manga manga = kvp.Value;
                manga.Print(mangaId);
                Console.WriteLine();
            }

            PrintSystemMessage("Finished displaying all mangas.");
            Console.WriteLine();
        }

        static void AddMangaCommand(string[] args, MangaStorage mangaStorage)
        {
            Console.WriteLine();
            if (args.Length >= 5)
            {
                string title = args[1];
                string type = args[2];

                // Parse chapter number
                if (int.TryParse(args[3], out int chapters))
                {
                    // If manga is completed or not
                    bool isCompleted = args.Length > 4 && args[4].ToLower() == "true";
                    AddNewMangaUsingCommandLine(title, type, chapters, isCompleted, mangaStorage);
                }
                else
                {
                    PrintErrorMessage("Invalid number of chapters. Please try again.");
                }
            }
            else
            {   // Example: --add "Odd Girl Out" manhwa 200 no
                PrintErrorMessage("Insufficient arguments. Please use --add <title> <type> <chapters> <isCompleted>");
            }
        }

        static void SearchMangaCommand(string[] args, MangaStorage mangaStorage)
        {
            Console.WriteLine();

            // Make sure title is provided
            if (args.Length > 1)
            {
                string title = args[1];
                SearchForMangaUsingCommandLine(title, mangaStorage);
            }
            else
            {
                PrintErrorMessage("Title not provided. Use --search <title>");
            }
        }

        static void AddNewMangaUsingCommandLine(string title, string type, int chapters, bool isCompleted, MangaStorage mangaStorage)
        {
            string filePath = "mangaData.dat";

            try
            {
                // Create new manga object
                Manga newManga = CreateManga(type, title, chapters, isCompleted);

                if (newManga == null)
                {
                    PrintErrorMessage("Invalid type. Please use 'Manga', 'Manhua', or 'Manhwa'.");
                    return;
                }

                mangaStorage.AddNewManga(newManga, filePath);
                Console.WriteLine($"Manga '{title}' of type '{type}' added successfully.");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                PrintErrorMessage($"Error: {ex.Message}");
            }
        }

        static Dictionary<string, List<KeyValuePair<int, Manga>>> titleSearchCache = new Dictionary<string, List<KeyValuePair<int, Manga>>>(StringComparer.OrdinalIgnoreCase);

        static void SearchForMangaUsingCommandLine(string title, MangaStorage mangaStorage)
        {
            // Check cache for manga title
            if (!titleSearchCache.TryGetValue(title, out var cachedResults))
            {
                // If not in cache, then search for the manga
                var foundMangas = mangaStorage.SearchMangaByTitle(title);

                if (foundMangas.Any())
                {
                    cachedResults = foundMangas.Select(manga =>
                        new KeyValuePair<int, Manga>(
                            mangaStorage.GetAllMangas()
                                        .First(kvp => kvp.Value == manga).Key,manga))
                                        .ToList();

                    titleSearchCache[title] = cachedResults;
                }
                else
                {
                    PrintErrorMessage($"No mangas found with the title '{title}'.");
                    return;
                }
            }

            foreach (var kvp in cachedResults)
            {
                int mangaId = kvp.Key;
                Manga manga = kvp.Value;

                Console.WriteLine($"Found: {manga.GetTitle()}");
                manga.Print(mangaId);
                Console.WriteLine();
            }
        }
    }
}
