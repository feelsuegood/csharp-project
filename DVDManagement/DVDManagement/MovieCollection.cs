namespace DVDManagement
{
    public class MovieCollection : iMovieCollection
    {
        private const int MaxMovies = 1000;
        private Movie[] movies;
        private int movieCount;
        private const string filePath = "movies.txt";

        public MovieCollection()
        {
            movies = new Movie[MaxMovies];
            movieCount = 0;
            LoadMovies();
        }

        public int MovieCount => movieCount;

        // Hash function: division method
        private int GetHash(string title)
        {
            int hash = 0;
            foreach (char c in title)
            {
                hash += c;
            }
            return hash % MaxMovies;
        }

        public void AddMovie(Movie movie)
        {
            int index = GetHash(movie.Title);

            // Collision handling: quadratic probing (open addressing)
            while (movies[index] != null && movies[index].Title != movie.Title)
            {
                index = (index + 1) % MaxMovies;
            }

            if (movies[index] != null && movies[index].Title == movie.Title)
            {
                movies[index].Copies += movie.Copies;
                SaveMovies();
            }
            else if (movieCount < MaxMovies)
            {
                while (movies[index] != null)
                {
                    index = (index + 1) % MaxMovies;
                }
                movies[index] = movie;
                movieCount++;
                SaveMovies();
            }
            else
            {
                throw new InvalidOperationException("Cannot add more movies.");
            }
        }

        public void RemoveMovie(string title, int copies)
        {
            int index = GetHash(title);

            // Collision handling: quadratic probing (open addressing)
            while (movies[index] != null && movies[index].Title != title)
            {
                index = (index + 1) % MaxMovies;
            }

            if (movies[index] != null && movies[index].Title == title)
            {
                if (movies[index].Copies < copies)
                {
                    throw new InvalidOperationException("The number of copies to remove exceeds the number of copies in the library.");
                }

                movies[index].Copies -= copies;

                if (movies[index].Copies == 0)
                {
                    movies[index] = null;
                    movieCount--;
                    for (int i = (index + 1) % MaxMovies; movies[i] != null; i = (i + 1) % MaxMovies)
                    {
                        Movie temp = movies[i];
                        movies[i] = null;
                        movieCount--;
                        if (temp != null)
                        {
                            AddMovie(temp);
                        }
                    }
                }

                SaveMovies();
            }
            else
            {
                throw new InvalidOperationException("Movie not found.");
            }
        }

        public Movie FindMovie(string title)
        {
            int index = GetHash(title);

            // Collision handling: quadratic probing (open addressing)
            while (movies[index] != null && movies[index].Title != title)
            {
                index = (index + 1) % MaxMovies;
            }
            return movies[index];
        }

        public Movie[] GetAllMovies()
        {
            Movie[] allMovies = new Movie[movieCount];
            int count = 0;
            for (int i = 0; i < MaxMovies; i++)
            {
                if (movies[i] != null)
                {
                    allMovies[count++] = movies[i];
                }
            }
            Array.Sort(allMovies, (x, y) => x.Title.CompareTo(y.Title));
            return allMovies;
        }

        public void BorrowMovie(string title)
        {
            Movie movie = FindMovie(title);
            if (movie == null || movie.Copies < movie.CurrentBorrowCount)
            {
                throw new InvalidOperationException("Movie not available.");
            }
            movie.CurrentBorrowCount++;
            movie.TotalBorrowCount++;

            SaveMovies();
        }

        public void ReturnMovie(string title)
        {
            Movie movie = FindMovie(title);
            if (movie == null)
            {
                throw new InvalidOperationException("Movie not found.");
            }
            if (movie.CurrentBorrowCount > 0)
            {
                movie.CurrentBorrowCount--;
            }
            else
            {
                throw new InvalidOperationException("No borrowed copies to return.");
            }

            SaveMovies();
        }

        public Movie[] GetMoviesInDictionaryOrder()
        {
            Movie[] sortedMovies = new Movie[movieCount];
            int count = 0;
            for (int i = 0; i < MaxMovies; i++)
            {
                if (movies[i] != null)
                {
                    sortedMovies[count++] = movies[i];
                }
            }
            Array.Sort(sortedMovies, (x, y) => x.Title.CompareTo(y.Title));
            return sortedMovies;
        }

        // Use Merge Sort to sort movies by borrow count
        private void Merge((string Title, int Count)[] array, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            var leftArray = new (string Title, int Count)[n1];
            var rightArray = new (string Title, int Count)[n2];

            for (int i = 0; i < n1; ++i)
                leftArray[i] = array[left + i];
            for (int j = 0; j < n2; ++j)
                rightArray[j] = array[mid + 1 + j];

            int k = left;
            int iIndex = 0, jIndex = 0;

            while (iIndex < n1 && jIndex < n2)
            {
                if (leftArray[iIndex].Count >= rightArray[jIndex].Count)
                {
                    array[k] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    array[k] = rightArray[jIndex];
                    jIndex++;
                }
                k++;
            }

            while (iIndex < n1)
            {
                array[k] = leftArray[iIndex];
                iIndex++;
                k++;
            }

            while (jIndex < n2)
            {
                array[k] = rightArray[jIndex];
                jIndex++;
                k++;
            }
        }

        private void MergeSort((string Title, int Count)[] array, int left, int right)
        {
            if (left < right)
            {
                int mid = left + (right - left) / 2;

                MergeSort(array, left, mid);
                MergeSort(array, mid + 1, right);

                Merge(array, left, mid, right);
            }
        }

        public (string Title, int Count)[] GetTop3Movies()
        {
            Movie[] allMovies = GetAllMovies();
            (string Title, int Count)[] borrowInfo = allMovies
                .Select(m => (m.Title, m.TotalBorrowCount))
                .ToArray();

            MergeSort(borrowInfo, 0, borrowInfo.Length - 1);

            return borrowInfo.Take(3).ToArray();
        }

        public void SaveMovies()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int i = 0; i < MaxMovies; i++)
                    {
                        if (movies[i] != null)
                        {
                            writer.WriteLine(movies[i].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save movies: {ex.Message}");
            }
        }

        public void LoadMovies()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var movie = Movie.FromString(line);
                            int index = GetHash(movie.Title);
                            while (movies[index] != null)
                            {
                                index = (index + 1) % MaxMovies;
                            }
                            movies[index] = movie;
                            movieCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load movies: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Movies file not found.");
            }
        }
    }
}
