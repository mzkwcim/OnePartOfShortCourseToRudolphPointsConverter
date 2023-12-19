using HtmlAgilityPack;
using Npgsql;
using System;


class SCMRP
{
    static void Main()
    {
        string url = "https://www.swimrankings.net/index.php?page=athleteDetail&athleteId=5161550";
        int[] tab = GetPkt(url);
        string[] distnace = GetDistance(url);
        string name = GetName(url);
        Console.WriteLine(name);
        Dictionary<string, int> athlete = new Dictionary<string, int>();
        CreateandAdd();
    }

    public static void CreateandAdd()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Mzkwcim181099!;Database=WorldRecords";

        Dictionary<string, double> RudolphTableValues = new Dictionary<string, double>();
        double[] wo = ConvertStringToDouble(GettingShortCurseWorldRecordsMen());
        List<string> distancesarray = GettingDistances();
        List<double> records = new List<double>(wo);
        records.Remove(21.75);
        records.Remove(443.42);
        string pomocnik = "REAL";
        string createTableQueryTest = CreateTable(distancesarray, pomocnik);
        string addValuesQueryTest = AddValues(records, distancesarray);
        Connection(connectionString, createTableQueryTest, addValuesQueryTest);
    }
    public static void Connection(string connectionString, string createTableQueryTest, string addValuesQueryTest)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(createTableQueryTest, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Powodzenie");
                }
                using (NpgsqlCommand command2 = new NpgsqlCommand(addValuesQueryTest, connection))
                {
                    command2.ExecuteNonQuery();
                    Console.WriteLine("Powodzenie");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Błąd: {e.Message}");
            }
        }
    }
    public static string CreateTable(List<string> distancesarray, string pomocnik)
    {
        string createTableQueryTest = "CREATE TABLE WorldRecords (" +
                          "ID SERIAL PRIMARY KEY, ";
        for (int i = 0; i < distancesarray.Count; i++)
        {
            createTableQueryTest += (i < distancesarray.Count - 1) ? $"\"{distancesarray[i]}\" {pomocnik}, " : $"\"{distancesarray[i]}\" {pomocnik} )";
        }
        return createTableQueryTest;
    }
    public static string AddValues(List<double> records, List<string> distancesarray)
    {
        string addValuesQueryTest = "INSERT INTO WorldRecords(";
        for (int i = 0; i < distancesarray.Count; i++)
        {
            addValuesQueryTest += (i < distancesarray.Count - 1) ? $"\"{distancesarray[i]}\", " : $"\"{distancesarray[i]}\" ) VALUES (";
        }
        for (int i = 0; i < records.Count; i++)
        {
            addValuesQueryTest += (i < records.Count - 1) ? $"{records[i]}, " : $"{records[i]} );";
        }
        return addValuesQueryTest;
    }
    public static string[] GettingShortCurseWorldRecordsMen()
    {
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&genderCourse=SCM_1";
        var htmlDocument = Loader(url);
        var currentTimes = htmlDocument.DocumentNode.SelectNodes("//td[@class='swimtime']");
        string[] tab = new string[currentTimes.Count];
        Console.WriteLine(currentTimes.Count);
        for (int i = 0; i < currentTimes.Count; i++)
        {
            tab[i] = currentTimes[i].InnerText;
            Console.WriteLine(tab[i]);
        }
        return tab;
    }
    public static string[] GettingLongCurseWorldRecordsMen()
    {
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=LCM&styleId=1";
        var htmlDocument = Loader(url);
        var currentTimes = htmlDocument.DocumentNode.SelectNodes("//td[@class='swimtime']");
        string[] tab = new string[20];
        for (int i = 0; i < tab.Length; i++)
        {
            tab[i] = currentTimes[i].InnerText;
        }
        return tab;
    }
    static double [] ConvertStringToDouble(string[]doubles)
    {
        double[] converted2 = new double[doubles.Length]; 
        for (int i = 0; i < doubles.Length; i++)
        {
            string[] list = doubles[i].Split(":");
            converted2[i] = (list.Length != 1) ? (Math.Round(Convert.ToDouble(list[0]), 2) * 60 + Math.Round(Convert.ToDouble(list[1]), 2)) : Math.Round(Convert.ToDouble(list[0]), 2);
        }
        return converted2;
    }
    public static List<string> GettingDistances()
    {
        List<string> url = GettingDistancesFromLinks();
        List<string> strings = new List<string>();

        foreach (var u in url)
        {
            var htmlDocument = Loader(u);
            if (!String.IsNullOrEmpty(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText) && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 300m Freestyle") && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 1000m Freestyle"))
            {
                strings.Add(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText.Replace("Record history for ",""));
            }
        }
        string temp = strings[17];
        strings[17] = strings[16];
        strings[16] = strings[15];
        strings[15] = temp;
        for (int i = 0; i < strings.Count; i++)
        {
            Console.WriteLine(strings[i]);
        }
        return strings;
    }
    static HtmlAgilityPack.HtmlDocument Loader(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument;
    }
    public static List<string> GettingDistancesFromLinks()
    {
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=SCM&styleId=0";
        List<string> tab = new List<string>();
        for (int i = 1; i < 21; i++)
        {
            tab.Add(url.Replace("styleId=0", $"styleId={i}"));
        }
        return tab;
    }
    public static int[] GetPkt(string url)
    {
        var htmlDocument = Loader(url);
        var times = htmlDocument.DocumentNode.SelectNodes("//tr[@class='athleteBest0']//td[@class='code']");
        int[] tab = new int[times.Count];
        int inter = 0;
        for (int i = 0; i < tab.Length; i++)
        {
            string newby = Convert.ToString(times[i].InnerText.Trim().Replace("-", ""));
            if (!String.IsNullOrEmpty(newby))
            {
                tab[inter] = Convert.ToInt32(newby);
                inter++;
            }
        }
        return tab;
    }
    public static string[] GetDistance(string url)
    {
        var htmlDocument = Loader(url);
        var times = htmlDocument.DocumentNode.SelectNodes("//tr[@class='athleteBest0']//td[@class='event']//a");
        string[] tab = new string[times.Count];
        for (int i = 0; i < tab.Length; i++)
        {
            tab[i] = times[i].InnerText.Trim();
        }
        return tab;
    }
    public static string GetName(string url)
    {
        var htmlDocument = Loader(url);
        string times = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='name']").InnerText.Trim().Replace("(2009&nbsp;&nbsp;)", "").Replace(",", "");
        return times;
    }
}