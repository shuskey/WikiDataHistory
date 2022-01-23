using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Text.Json;
using WikiDataHistory.Sparql;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace WikiDataHistory
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            int startYear = 1200;
            int endYear = DateTime.Now.Year;
            Console.WriteLine($"{DateTime.Now.ToString("h:mm:ss tt")} Hello World Top Events!  Collecting WikiData from {startYear} to {endYear}.");
            string sparqlFileName = "TopEvents_SPARQL.txt";
            string dbFileName = "TopEvents.db";
            string dbTableName = "topEvents";            
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string dbFullPath = Path.Combine(projectDirectory, $"Sqlite\\{dbFileName}");

            string cs = $"URI=file:{dbFullPath}";
            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DROP TABLE IF EXISTS {dbTableName}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @$"CREATE TABLE {dbTableName}(id INTEGER PRIMARY KEY, year TEXT, linkCount INT, item TEXT, itemLabel TEXT, picture TEXT, wikiLink TEXT, description TEXT, aliases TEXT, locations TEXT, countries TEXT, pointInTime TEXT, eventStartDate TEXT, eventEndDate TEXT)";
            cmd.ExecuteNonQuery();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Client");

            for (int year = startYear; year <= endYear; year++)
            {
                Console.WriteLine($"{DateTime.Now.ToString("h:mm:ss tt")} Getting ready to collect Top Events for the year {year}.");
                var urlEncodedSPARQLQuery = HttpUtility.UrlEncode(getSPARQLQueryFromFile(sparqlFileName, year));                
                try
                {
                    var topEventsResponse = await ProcessWikiDataQuery(urlEncodedSPARQLQuery);
                    Console.WriteLine($"Number of Events found for year {year} = {topEventsResponse.Results.Bindings.Length}");
                    foreach (var topEvent in topEventsResponse.Results.Bindings)
                    {
                        //WriteToConsole(topEvent);
                        WriteToSqliteDb(cmd, topEvent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Got an {ex.Message} exception when processing {year}.");
                }
                Console.WriteLine($"-=-=-=-=-=-=-=-=-=-=-");
            }
            Console.WriteLine($"{DateTime.Now.ToString("h:mm:ss tt")} Done.");
            con.Close();
        }
        private static SQLiteConnection InitializeSqliteDb(string dbFileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string dbFullPath = Path.Combine(projectDirectory, $"Sqlite\\{dbFileName}");

            string cs = $"URI=file:{dbFullPath}";
            using var con = new SQLiteConnection(cs);
            con.Open();
            return con;
        }

        private static void WriteToSqliteDb(SQLiteCommand cmd, Binding topEvent)
        {
            var year = topEvent.Year?.Value;  //Integer
            if (year == null) return;
            var linkCount = topEvent.LinkCount?.Value; //Integer
            if (linkCount == null) linkCount = "0";
            var item = topEvent.Item?.Value;  //Web Link
            var itemLabel = SparqlLabelToString(topEvent.ItemLabel);
            var picture = topEvent.Picture?.Value;  //Web Link
            var wikiLink = topEvent.WikiLink?.Value;  //Web Link                        
            var description = SparqlLabelToString(topEvent.Description);
            var aliases = SparqlLabelToString(topEvent.Aliases);
            var locations = SparqlLabelToString(topEvent.Locations);
            var countries = SparqlLabelToString(topEvent.Countries);
            var pointInTime = topEvent.PointInTime?.Value;  //DateTime
            var eventStartDate = topEvent.EventStartDate?.Value;  //DateTime
            var eventEndDate = topEvent.EventEndDate?.Value;  //DateTime

            cmd.CommandText = $"INSERT INTO topEvents(year, linkCount, item, itemLabel, picture, wikiLink, description, aliases, locations, countries, pointInTime, eventStartDate, eventEndDate) VALUES('{year}',{linkCount},'{item}','{itemLabel}', '{picture}', '{wikiLink}', '{description}', '{aliases}', '{locations}', '{countries}', '{pointInTime}', '{eventStartDate}', '{eventEndDate}')";
            cmd.ExecuteNonQuery();
        }

        private static string SparqlLabelToString(Label sparqlLabel)
        {
            var stringToReturn = sparqlLabel?.Value;
            if (stringToReturn != null)
                stringToReturn = stringToReturn.Replace("'", "''");
            return stringToReturn;
        }

        private static void WriteToConsole(Binding topEvent)
        {
            Console.Write($"Year = {topEvent.Year.Value}\t");
            Console.Write($"LinkCount = {topEvent.LinkCount?.Value}\t");
            Console.WriteLine($"EventLabel = {topEvent.ItemLabel?.Value}");
            Console.Write($"Item = {topEvent.Item?.Value}\t");
            Console.Write($"Picture = {topEvent.Picture?.Value}\t");
            Console.WriteLine($"WikiLink = {topEvent.WikiLink?.Value}");
            Console.Write($"Desc = {topEvent.Description?.Value}\t");
            Console.WriteLine($"Aliases = {topEvent.Aliases?.Value}");
            Console.Write($"Locations = {topEvent.Locations?.Value}\t");
            Console.WriteLine($"Countries = {topEvent.Countries?.Value}");
            Console.Write($"PointInTime = {topEvent.PointInTime?.Value}\t");
            Console.Write($"EventStartDate = {topEvent.EventStartDate?.Value}\t");
            Console.WriteLine($"EventEndDate = {topEvent.EventEndDate?.Value}");
        }

        private static async Task<ServerResponse> ProcessWikiDataQuery(string queryString)
        {            
            var stringTask = client.GetStringAsync($"https://query.wikidata.org/sparql?query={queryString}");
            var stringResult = await stringTask;

            return JsonSerializer.Deserialize<ServerResponse>(stringResult);            
        }

        private static string getSPARQLQueryFromFile(string sparqlFileName, int year)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
             
            var queryText = System.IO.File.ReadAllText(Path.Combine(directoryName, $"Sparql\\{sparqlFileName}"));
            queryText = queryText.Replace("1881-", $"{year}-");

            return queryText;
        }
    }
}
