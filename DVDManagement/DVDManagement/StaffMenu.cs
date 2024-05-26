using System;
using System.Net;
using static System.Console;

namespace DVDManagement
{
    public static class StaffMenu
    {
        public static void StaffLogin(MemberCollection memberCollection, MovieCollection movieCollection)
        {
            while (true)
            {
                WriteLine("* 0 to go back");
                Write("* Please enter staff username ==> ");
                string username = ReadLine() ?? string.Empty;
                if (username == "0") return;

                WriteLine("* 0 to go back");
                Write("* Please enter staff password ==> ");
                string password = ReadLine() ?? string.Empty;
                if (password == "0") return;

                if (username == "staff" && password == "today123")
                {
                    StaffOnlyMenu(memberCollection, movieCollection);
                    return;
                }
                else
                {
                    WriteLine("Invalid username or password");
                    WriteLine("Please enter any key to retry");
                    if (ReadLine() == "0") return;
                }
            }
        }
        private static void StaffOnlyMenu(MemberCollection memberCollection, MovieCollection movieCollection)
        {
            while (true)
            {
                Clear();
                WriteLine("Staff Menu");
                WriteLine("----------------------------------------------------------------");
                WriteLine("1. Add DVDs to system");
                WriteLine("2. Remove DVDs from system");
                WriteLine("3. Register a new member to system");
                WriteLine("4. Remove a registered member from system");
                WriteLine("5. Find a member contact phone number, given the member's name");
                WriteLine("6. Find members who are currently renting a particular movie");
                WriteLine("0. Return to main menu");

                switch (ReadLine())
                {
                    case "1":
                        AddMovie(movieCollection);
                        break;
                    case "2":
                        RemoveMovie(movieCollection);
                        break;
                    case "3":
                        RegisterMember(memberCollection);
                        break;
                    case "4":
                        RemoveMember(memberCollection);
                        break;
                    case "5":
                        FindMemberPhoneNumber(memberCollection);
                        break;
                    case "6":
                        ListMembersBorrowingMovie(memberCollection);
                        break;
                    case "0":
                        return;
                    default:
                        WriteLine("Invalid input.");
                        WriteLine("Please enter from 0 to 8.");
                        ReadLine();
                        break;
                }
            }

        }
        static void AddMovie(MovieCollection movieCollection)
        {
            string? title = null;
            string? genre = null;
            string? classification = null;
            int duration = -1;
            int copies = -1;

            while (true)
            {
                WriteLine("* 0 to go back");
                Write("* Please enter movie title ==> ");
                title = ReadLine();
                if (title == "0") return;
                if (string.IsNullOrWhiteSpace(title))
                {
                    WriteLine("Invalid title. Please try again.");
                    WriteLine();
                    continue;
                }

                Movie existingMovie = movieCollection.FindMovie(title);
                if (existingMovie != null)
                {
                    WriteLine("* Movie already exists. Please enter the number of copies to add.");
                    while (copies <= 0)
                    {
                        Write("* Enter the number of copies (must be greater than 0) ==> ");
                        string? copiesInput = ReadLine();
                        if (copiesInput == "0") return;
                        if (!int.TryParse(copiesInput, out copies) || copies <= 0)
                        {
                            WriteLine("Invalid number of copies. Please try again.");
                            WriteLine();
                        }
                    }
                    existingMovie.Copies += copies;
                    movieCollection.SaveMovies();
                    WriteLine("Copies added successfully. Please enter any key to add another movie.");
                    if (ReadLine() == "0") return;
                }
                else
                {
                    while (!CheckGenre(genre))
                    {
                        WriteLine("You can choose genre from Drama, Adventure, Family, Action, Sci-fi, Comedy, Animated, Thriller, or Other");
                        Write("Enter genre ==> ");
                        genre = ReadLine();
                        if (genre == "0") return;
                        if (!CheckGenre(genre))
                        {
                            WriteLine("Invalid genre. Please try again.");
                            WriteLine();

                        }
                    }

                    while (!CheckClass(classification))
                    {
                        WriteLine("You can choose classification from General (G), Parental Guidance (PG), Mature (M15+), or Mature Accompanied (MA15+)");
                        WriteLine("Only code input is valid. e.g. Parental Guidance (X) PG (O)");
                        Write("Enter classification ==> ");
                        classification = ReadLine();
                        if (classification == "0") return;
                        if (!CheckClass(classification))
                        {
                            WriteLine("Invalid classification. Please try again.");
                            WriteLine();
                        }
                    }

                    while (duration < 0 || duration > 300)
                    {
                        WriteLine("You can enter duration in minutes from 0 to 300 minutes.");
                        Write("Enter duration in minutes ==> ");
                        string? durationInput = ReadLine();
                        if (durationInput == "0") return;
                        if (!int.TryParse(durationInput, out duration) || duration < 0 || duration > 300)
                        {
                            WriteLine("Invalid duration. Please try again.");
                            WriteLine();
                        }
                    }

                    while (copies <= 0)
                    {
                        WriteLine("You can enter the number of copies (must be greater than 0).");
                        Write("Enter the number of copies ==> ");
                        string? copiesInput = ReadLine();
                        if (copiesInput == "0") return;
                        if (!int.TryParse(copiesInput, out copies) || copies <= 0)
                        {
                            WriteLine("Invalid number of copies. Please try again.");
                            WriteLine();
                        }
                    }

                    Movie movie = new Movie(title, genre!, classification!, duration, copies);
                    movieCollection.AddMovie(movie);
                    movieCollection.SaveMovies();

                    WriteLine("Movie added successfully. Please enter any key to add another movie.");
                    if (ReadLine() == "0") return;
                }
            }
        }

        static bool CheckGenre(string? genre)
        {
            string[] validGenres = { "Drama", "Adventure", "Family", "Action", "Sci-fi", "Comedy", "Animated", "Thriller", "Other" };
            return genre != null && validGenres.Contains(genre);
        }

        static bool CheckClass(string? classification)
        {
            string[] validClassifications = { "G", "PG", "M15+", "MA15+" };
            return classification != null && validClassifications.Contains(classification);
        }

        static void RemoveMovie(MovieCollection movieCollection)
        {
            while (true)
            {
                WriteLine("* 0 to go back");
                Write("* Enter movie title to remove ==> ");
                string? title = ReadLine();
                if (title == "0") return;

                if (string.IsNullOrWhiteSpace(title))
                {
                    WriteLine("Invalid input. Please try again.");
                    continue;
                }

                int copies = -1;
                while (copies <= 0)
                {
                    WriteLine("* 0 to go back");
                    Write("Enter number of copies to remove ==> ");
                    string? copiesInput = ReadLine();
                    if (copiesInput == "0") return;
                    if (!int.TryParse(copiesInput, out copies) || copies <= 0)
                    {
                        WriteLine("Invalid number of copies. Please try again.");
                    }
                }


                try
                {
                    movieCollection.RemoveMovie(title, copies);
                    WriteLine("* 0 to go back");
                    Write("Movies removed successfully. Please enter any key to remove another movie.");
                    if (ReadLine() == "0") return;
                }
                catch (InvalidOperationException ex)
                {
                    WriteLine(ex.Message);
                    Write("Please enter any key to try again.");
                    if (ReadLine() == "0") return;
                }
            }
        }


        static void RegisterMember(MemberCollection memberCollection)
        {
            while (true)
            {
                Write("Enter first name ==> ");
                string? firstName = ReadLine();
                Write("Enter last name ==> ");
                string? lastName = ReadLine();
                Write("Enter phone number ==> ");
                string? phoneNumber = ReadLine();

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(phoneNumber))
                {
                    WriteLine("* 0 to go back");
                    Write("Invalid input. Please enter any key to try again.");
                    if (ReadLine() == "0") return;
                    continue;
                }

                if (memberCollection.IsMemberExist(firstName, lastName))
                {
                    WriteLine("* 0 to go back");
                    Write("Member already exists. Please enter any key to try again.");
                    if (ReadLine() == "0") return;
                    continue;
                }

                string? password = null;
                while (string.IsNullOrWhiteSpace(password) || password.Length != 4 || !int.TryParse(password, out _))
                {
                    Write("Enter a 4-digit password for the member ==> ");
                    password = ReadLine();
                    if (string.IsNullOrWhiteSpace(password) || password.Length != 4 || !int.TryParse(password, out _))
                    {
                        WriteLine("* 0 to go back");
                        Write("Invalid password. Please enter a valid 4-digit password.");
                        if (ReadLine() == "0") return;
                        continue;
                    }
                }

                Member member = new Member(firstName, lastName, phoneNumber, password);
                memberCollection.AddMember(member);
                WriteLine("* 0 to go back");
                Write("Member registered successfully. Please enter any key to register another member.");
                if (ReadLine() == "0") return;
                continue;
            }

        }

        static void RemoveMember(MemberCollection memberCollection)
        {
            while (true)
            {
                Write("Enter first name ==> ");
                string? firstName = ReadLine();
                Write("Enter last name ==> ");
                string? lastName = ReadLine();

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    WriteLine("* 0 to go back");
                    WriteLine("Invalid input. Please enter any key to retry");
                    if (ReadLine() == "0") return;
                    continue;
                }

                try
                {
                    memberCollection.RemoveMember(firstName, lastName);
                    WriteLine("* 0 to go back");
                    Write("Member removed successfully. Please enter any key to remove another member.");
                    if (ReadLine() == "0") return;
                    continue;
                }
                catch (InvalidOperationException ex)
                {
                    WriteLine(ex.Message);
                }

                ReadLine();
            }
        }

        static void FindMemberPhoneNumber(MemberCollection memberCollection)
        {
            while (true)
            {
                Write("Enter first name ==> ");
                string? firstName = ReadLine();
                Write("Enter last name ==> ");
                string? lastName = ReadLine();

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    WriteLine("* 0 to go back");
                    Write("Invalid input. Please enter any key to find another member.");
                    if (ReadLine() == "0") return;
                    continue;
                }

                string phoneNumber = memberCollection.FindMemberPhoneNumber(firstName, lastName);
                WriteLine($"Phone Number: {phoneNumber}");
                WriteLine("Please enter any key to go back to the menu.");
                ReadLine();
            }

        }

        static void ListMembersBorrowingMovie(MemberCollection memberCollection)
        {
            Write("Enter movie title ==> ");
            string? title = ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                WriteLine("Invalid input. Please enter any key to go back to the menu.");
                ReadLine();
                return;
            }

            Member[] rentingMembers = memberCollection.FindMembersByMovie(title);
            if (rentingMembers.Length > 0)
            {
                WriteLine("Members currently renting this movie:");
                foreach (var member in rentingMembers)
                {
                    WriteLine($"{member.FirstName} {member.LastName} - {member.PhoneNumber}");
                }
            }
            else
            {
                WriteLine("No members are currently renting this movie.");
            }

            WriteLine("Please enter any key to go back to the menu.");
            ReadLine();
        }
    }
}

