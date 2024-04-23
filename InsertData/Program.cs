using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;


namespace InsertData
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Створення таблиць
            //CreateTables();
            // DisplayAllData();
            InsertIntoAnimeGenres();
            Insert();

            Console.ReadLine();
        }
        private static void CreateTables()
        {
            using (NpgsqlConnection connection = GetConnection())
            {
                connection.Open();

                // Створення таблиць, якщо вони ще не існують
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"Anime\" (\"Id\" SERIAL PRIMARY KEY, \"Name\" VARCHAR, \"Text\" TEXT, \"Year\" INTEGER, \"Photo\" VARCHAR, \"Imdbrate\" DOUBLE PRECISION);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"Genre\" (\"Id\" SERIAL PRIMARY KEY, \"Name\" VARCHAR);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"UserInfo\" (\"Id\" SERIAL PRIMARY KEY, \"Username\" VARCHAR, \"Password\" VARCHAR, \"Email\" VARCHAR, \"Name\" VARCHAR, \"Level\" INTEGER, \"Text\" TEXT, \"Photo\" VARCHAR, \"WatchedCount\" INTEGER, \"isAdmin\" BOOLEAN);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"Review\" (\"Id\" SERIAL PRIMARY KEY, \"UserId\" INTEGER, \"AnimeId\" INTEGER, \"Text\" TEXT, \"Rate\" INTEGER);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"AnimeGenre\" (\"Id\" SERIAL PRIMARY KEY, \"AnimeId\" INTEGER, \"GenreId\" INTEGER);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"AddedAnime\" (\"Id\" SERIAL PRIMARY KEY, \"UserId\" INTEGER, \"AnimeId\" INTEGER);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"LikedAnime\" (\"Id\" SERIAL PRIMARY KEY, \"UserId\" INTEGER, \"AnimeId\" INTEGER);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"DislikedAnime\" (\"Id\" SERIAL PRIMARY KEY, \"UserId\" INTEGER, \"AnimeId\" INTEGER);");
                CreateTable(connection, "CREATE TABLE IF NOT EXISTS public.\"WatchedAnime\" (\"Id\" SERIAL PRIMARY KEY, \"UserId\" INTEGER, \"AnimeId\" INTEGER);");
            }
        }
        private static void CreateTable(NpgsqlConnection connection, string createTableQuery)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void InsertIntoAnimeGenres()
        {
            // Базовий URL сторінки з фільмами на сайті uakino.club
            string baseUrl = "https://anitube.in.ua/anime/page/";

            // Лічильник записів
            int recordCount = 0;

            // З'єднання з базою даних
            using (NpgsqlConnection connection = GetConnection())
            {
                connection.Open();

                // Очистимо дані з усіх таблиць перед додаванням нових
                //ClearTables(connection);

                // Цикл по сторінкам
                for (int page = 1; recordCount < 301; page++)
                {
                    // URL сторінки з фільмами
                    string uakinoUrl = $"{baseUrl}{page}/";

                    // Завантаження HTML-сторінки
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(uakinoUrl);

                    // Отримання інформації про фільми
                    var movieNodes = doc.DocumentNode.SelectNodes("//div[@class='box']");

                    if (movieNodes == null || movieNodes.Count == 0)
                    {
                        Console.WriteLine($"No more records found. Exiting...");
                        break;
                    }

                    foreach (var movieNode in movieNodes)
                    {
                        // Отримання інформації про фільм
                        var titleNode = movieNode.SelectSingleNode(".//div[@class='story_c']/h2[@itemprop='name']/a");
                        var descriptionNode = movieNode.SelectSingleNode(".//div[@class='story_c_text']");
                        var yearNode = movieNode.SelectSingleNode(".//div[@class='story_infa']/a");

                        // Отримання жанрів
                        var genreNode = movieNode.Descendants("dt")
                            .FirstOrDefault(n => n.InnerText.Contains("Категорія:"));
                        string genreText = genreNode?.NextSibling?.InnerText?.Trim();
                        string[] genres = genreText?.Split(',')?.Select(g => g.Trim())?.ToArray();

                        // Отримання рейтингу
                        var ratingNode =
                            movieNode.SelectSingleNode(".//div[@class='story_c_rate']//div[@class='div1']//span");
                        string ratingText = ratingNode?.InnerText?.Trim();
                        double rating = double.TryParse(ratingText, out var parsedRating) ? parsedRating : 0.0;
                        var imageNode = movieNode.SelectSingleNode(".//div[@class='story_c_l']//img");
                        string temp = imageNode?.GetAttributeValue("src", "");
                        string photo = "https://anitube.in.ua" + temp;

                        // Перевірка на null перед отриманням InnerText
                        string title = titleNode?.InnerText?.Trim();
                        string description = descriptionNode?.InnerText?.Trim();
                        string year = yearNode?.InnerText?.Trim();

                        // Виведення інформації
                        Console.WriteLine($"Title: {title}");
                        Console.WriteLine($"Description: {description}");
                        Console.WriteLine($"Year: {year}");
                        Console.WriteLine($"Rating: {rating}");

                        // Перевірка наявності жанрів перед виведенням
                        if (genres?.Length > 0)
                        {
                            Console.WriteLine($"Genres: {string.Join(", ", genres)}");
                        }

                        Console.WriteLine("------------------------------");

                        // Інкремент лічильника записів
                        recordCount++;

                        // Перевірка лічильника записів
                        if (recordCount >= 300)
                        {
                            Console.WriteLine("Reached the limit of 50 records. Exiting...");
                            break;
                        }

                        if (title != null)
                        {
                            // Додавання даних до бази даних
                            InsertDataIntoDatabase(connection, title, description, year, rating, genres, photo);
                        }

                    }
                }
            }
        }

        private static void Insert()
        {
            using (NpgsqlConnection con = GetConnection())
            {
                con.Open();


                Random random = new Random();
                int rowCount = 51;
                for (int i = 1; i < rowCount; i++)
                {
                    // Генерація випадкових даних за допомогою Faker
                    var faker = new Bogus.Faker();
                    //var animeName = faker.Random.Word();
                    //var animeYear = faker.Random.Number(1950, 2024);
                    //var animeImdbRate = faker.Random.Double(0, 10);
                    //var animeText = faker.Lorem.Paragraph();
                    //string[] photoPaths = { "https://github.com/yuliiapalamar/animatch/blob/master/animatch/AniWPF/photo/BungoStrayDogs.jpg?raw=true", "https://github.com/yuliiapalamar/animatch/blob/master/animatch/AniWPF/photo/AtackOnTitanS1.jpg?raw=true", "https://github.com/yuliiapalamar/animatch/blob/master/animatch/AniWPF/photo/SpyFamily.jpg?raw=true" };
                    //var animePhoto = photoPaths[random.Next(photoPaths.Length)];
                    //var genreName = faker.Random.Word();

                    var username = faker.Internet.UserName();
                    var password = faker.Internet.Password();
                    var email = faker.Internet.Email();
                    var name = faker.Name.FirstName();
                    var text = faker.Lorem.Sentence();
                    var photo = "https://github.com/yuliiapalamar/animatch/blob/master/animatch/AniWPF/photo/defaultUserPhoto.jpg?raw=true";
                    var level = 1;
                    var watchedcount = 1;

                    var review = faker.Lorem.Paragraph();
                    var rate = faker.Random.Number(1, 4);

                    int Id = i;

                    // Вставка даних у таблиці anime
                    // InsertDataIntoAnime(con, Id, animeName, animeYear, animeImdbRate, animeText, animePhoto);

                    // Вставка даних у таблицю genres
                    // InsertDataIntoGenres(con, Id, genreName);

                    // Вставка даних у таблицю userinfo
                    InsertDataIntoUserInfo(con, Id, username, password, email, name, text, photo, level, watchedcount);

                    // Вставка даних у таблицю review
                    InsertDataIntoReview(con, Id, Id, Id + 49, review, rate);

                    // Вставка даних у таблицю animegenres
                    //InsertDataIntoAnimeGenres(con, Id, Id, Id);

                    // Вставка даних у таблицю added
                    InsertDataIntoAdded(con, Id, Id, Id + 49);

                    // Вставка даних у таблицю liked
                    InsertDataIntoLiked(con, Id, Id, Id + 49);

                    // Вставка даних у таблицю disliked
                    InsertDataIntoDisLiked(con, Id, Id, Id + 49);

                    // Вставка даних у таблицю watched
                    InsertDataIntoWatched(con, Id, Id, Id + 49);
                }
            }
        }

        private static void InsertDataIntoAnime(NpgsqlConnection connection, int id, string name, int year, double imdbrate, string text, string photo)
        {
            string insertQuery = "INSERT INTO public.\"Anime\" (\"Id\", \"Name\", \"Text\", \"Imdbrate\", \"Photo\", \"Year\") VALUES (@id, @name, @text, @imdbrate, @photo, @year)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@text", text);
                command.Parameters.AddWithValue("@imdbrate", imdbrate);
                command.Parameters.AddWithValue("@photo", photo);
                command.Parameters.AddWithValue("@year", year);

                command.ExecuteNonQuery();
            }
        }
        private static void ClearTables(NpgsqlConnection connection)
        {
            // Видалення даних з усіх таблиць
            ExecuteNonQuery(connection, "DELETE FROM public.\"Anime\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"Genre\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"UserInfo\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"Review\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"AnimeGenre\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"AddedAnime\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"LikedAnime\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"DislikedAnime\";");
            ExecuteNonQuery(connection, "DELETE FROM public.\"WatchedAnime\";");
        }
        private static void ExecuteNonQuery(NpgsqlConnection connection, string query)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoGenres(NpgsqlConnection connection, int id, string name)
        {
            string insertQuery = "INSERT INTO public.\"Genre\" (\"Id\",\"Name\") VALUES (@id, @name)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);

                command.ExecuteNonQuery();
            }
        }
        private static void InsertDataIntoDatabase(NpgsqlConnection connection, string title, string description, string year, double rating, string[] genres, string photo)
        {
            // Insert genres first
            List<int> genreIds = InsertGenres(connection, genres);

            // Insert anime
            int animeId = InsertAnime(connection, title, description, year, rating, photo);

            // Associate anime with genres
            AssociateAnimeWithGenres(connection, animeId, genreIds);
        }

        private static List<int> InsertGenres(NpgsqlConnection connection, string[] genres)
        {
            List<int> genreIds = new List<int>();

            foreach (var genre in genres)
            {
                // Check if the genre already exists in the database
                string selectGenreQuery = "SELECT \"Id\" FROM public.\"Genre\" WHERE \"Name\" = @genre";
                using (NpgsqlCommand selectGenreCommand = new NpgsqlCommand(selectGenreQuery, connection))
                {
                    selectGenreCommand.Parameters.AddWithValue("@genre", genre);
                    object result = selectGenreCommand.ExecuteScalar();
                    if (result != null)
                    {
                        // Genre already exists, use its Id
                        genreIds.Add((int)result);
                        continue;
                    }
                }

                // Genre doesn't exist, insert it
                string insertGenreQuery = "INSERT INTO public.\"Genre\" (\"Name\") VALUES (@genre) RETURNING \"Id\"";
                using (NpgsqlCommand insertGenreCommand = new NpgsqlCommand(insertGenreQuery, connection))
                {
                    insertGenreCommand.Parameters.AddWithValue("@genre", genre);
                    int genreId = (int)insertGenreCommand.ExecuteScalar();
                    genreIds.Add(genreId);
                }
            }

            return genreIds;
        }

        private static int InsertAnime(NpgsqlConnection connection, string title, string description, string year, double rating, string photo)
        {
            string insertAnimeQuery = "INSERT INTO public.\"Anime\" (\"Name\", \"Text\", \"Year\", \"Photo\", \"Imdbrate\") VALUES (@title, @description, @year, @photo, @rating) RETURNING \"Id\"";

            using (NpgsqlCommand command = new NpgsqlCommand(insertAnimeQuery, connection))
            {
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@year", int.Parse(year));
                command.Parameters.AddWithValue("@photo", photo);
                command.Parameters.AddWithValue("@rating", rating);

                return (int)command.ExecuteScalar();
            }
        }

        private static void AssociateAnimeWithGenres(NpgsqlConnection connection, int animeId, List<int> genreIds)
        {
            foreach (var genreId in genreIds)
            {
                string insertAnimeGenreQuery = "INSERT INTO public.\"AnimeGenre\" (\"AnimeId\", \"GenreId\") VALUES (@animeId, @genreId)";
                using (NpgsqlCommand command = new NpgsqlCommand(insertAnimeGenreQuery, connection))
                {
                    command.Parameters.AddWithValue("@animeId", animeId);
                    command.Parameters.AddWithValue("@genreId", genreId);

                    command.ExecuteNonQuery();
                }
            }
        }



        private static void InsertDataIntoUserInfo(NpgsqlConnection connection, int id, string username, string password, string email, string name, string text, string photo, int level, int watchedcount)
        {
            string insertQuery = "INSERT INTO public.\"UserInfo\"(\"Id\", \"Username\", \"Password\", \"Email\", \"Name\", \"Level\", \"Text\", \"Photo\", \"WatchedCount\", \"isAdmin\") VALUES (@id, @username, @password, @email, @name, @level, @text, @photo, @watchedcount, false)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@level", level);
                command.Parameters.AddWithValue("@text", text);
                command.Parameters.AddWithValue("@photo", photo);
                command.Parameters.AddWithValue("@watchedcount", watchedcount);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoReview(NpgsqlConnection connection, int id, int userId, int animeId, string text, int rate)
        {
            string insertQuery = "INSERT INTO public.\"Review\" (\"Id\", \"UserId\", \"AnimeId\", \"Text\", \"Rate\") VALUES (@id, @userId, @animeId, @text, @rate)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@animeId", animeId);
                command.Parameters.AddWithValue("@text", text);
                command.Parameters.AddWithValue("@rate", rate);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoAnimeGenres(NpgsqlConnection connection, int id, int animeId, int genreId)
        {
            string insertQuery = "INSERT INTO public.\"AnimeGenre\" (\"Id\", \"AnimeId\", \"GenreId\") VALUES (@id, @animeId, @genreId)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@animeId", animeId);
                command.Parameters.AddWithValue("@genreId", genreId);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoAdded(NpgsqlConnection connection, int id, int userId, int animeId)
        {
            string insertQuery = "INSERT INTO public.\"AddedAnime\" (\"Id\", \"UserId\", \"AnimeId\") VALUES (@id, @userId, @animeId)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@animeId", animeId);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoLiked(NpgsqlConnection connection, int id, int userId, int animeId)
        {
            string insertQuery = "INSERT INTO public.\"LikedAnime\" (\"Id\", \"UserId\", \"AnimeId\") VALUES (@id, @userId, @animeId)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@animeId", animeId);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoDisLiked(NpgsqlConnection connection, int id, int userId, int animeId)
        {
            string insertQuery = "INSERT INTO public.\"DislikedAnime\" (\"Id\", \"UserId\", \"AnimeId\") VALUES (@id, @userId, @animeId)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@animeId", animeId);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertDataIntoWatched(NpgsqlConnection connection, int id, int userId, int animeId)
        {
            string insertQuery = "INSERT INTO public.\"WatchedAnime\" (\"Id\", \"UserId\", \"AnimeId\") VALUES (@id, @userId, @animeId)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@animeId", animeId);

                command.ExecuteNonQuery();
            }
        }

        //private static void Test()
        //{
        //    using(NpgsqlConnection con=GetConnection())
        //    {
        //        con.Open();
        //        if(con.State==ConnectionState.Open) 
        //        {
        //            Console.WriteLine("yes");
        //        }
        //    }    
        //}
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=yuliya2005;Database=animatchWeb;");
        }
    }
}
