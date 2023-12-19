using HtmlAgilityPack;
using Npgsql;


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

        for (int i = 0; i < tab.Length; i++)
        {
            if (!distnace[i].Contains("Lap") && !distnace[i].Contains("25m"))
            {
                athlete.Add(distnace[i], tab[i]);
            }
        }
        foreach (var wart in athlete)
        {
            Console.WriteLine($"{wart.Key}, {wart.Value}");
        }
        CreateandAdd();
    }

    public static void CreateandAdd()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=WorldRecords";

        Dictionary<string, double> RudolphTableValues = new Dictionary<string, double>();
        double[] wo = ConvertStringToDouble(GettingShortCurseWorldRecordsMen());
        string[] distancesarray = GettingDistances();
        List<double> records = new List<double>(wo);
        List<string> distances = new List<string>();
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
    public static string CreateTable(string[] distancesarray, string pomocnik)
    {
        string createTableQueryTest = "CREATE TABLE WorldRecords (" +
                          "ID SERIAL PRIMARY KEY, ";
        for (int i = 0; i < distancesarray.Length; i++)
        {
            if (i < distancesarray.Length - 1)
            {
                createTableQueryTest += $"\"{distancesarray[i]}\" {pomocnik}, ";
            }
            else
            {
                createTableQueryTest += $"\"{distancesarray[i]}\" {pomocnik} )";
            }
        }
        return createTableQueryTest;
    }
    public static string AddValues(List<double> records, string[] distancesarray)
    {
        string addValuesQueryTest = "INSERT INTO WorldRecords(";
        for (int i = 0; i < distancesarray.Length; i++)
        {
            if (i < distancesarray.Length - 1)
            {
                addValuesQueryTest += $"\"{distancesarray[i]}\", ";
            }
            else
            {
                addValuesQueryTest += $"\"{distancesarray[i]}\" ) VALUES (";
            }
        }
        for (int i = 0; i < records.Count; i++)
        {
            if (i < records.Count - 1)
            {
                addValuesQueryTest += $"{records[i]}, ";
            }
            else
            {
                addValuesQueryTest += $"{records[i]} ";
            }

        }
        addValuesQueryTest += ");";
        return addValuesQueryTest;
    }
    public static string[] GettingShortCurseWorldRecordsMen()
    {
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&genderCourse=SCM_1";
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var times = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList0']//td[@class='swimtime']//a[@class='time']");
        var times1 = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList1']//td[@class='swimtime']//a[@class='time']");
        string[] tab = new string[20];
        for (int i = 0; i < 20; i++)
        {
            if (i < 7)
            {
                if (i % 2 == 0)
                {
                    string dziwak = times[i / 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i == 1)
                {
                    string dziwak = times1[0].InnerText.Trim();
                    tab[1] = dziwak;
                }
                else if (i % 2 == 1 && i != 1)
                {
                    string dziwak = times1[i / 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 7 && i < 10)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 10 && i < 13)
            {
                if (i % 2 == 0)
                {
                    string dziwak = times[(i / 2) + 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 1)
                {
                    string dziwak = times1[(i / 2) - 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 13 && i < 17)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 17 && i < 20)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
        }
        return tab;
    }
    public static string[] GettingLongCurseWorldRecordsMen()
    {
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001";
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var times = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList0']//td[@class='swimtime']//a[@class='time']");
        var times1 = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList1']//td[@class='swimtime']//a[@class='time']");
        string[] tab = new string[20];
        for (int i = 0; i < 20; i++)
        {
            if (i < 9)
            {
                if (i % 2 == 0)
                {
                    string dziwak = times[i / 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i == 1)
                {
                    string dziwak = times1[0].InnerText.Trim();
                    tab[1] = dziwak;
                }
                else if (i % 2 == 1 && i != 1)
                {
                    string dziwak = times1[i / 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 7 && i < 10)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 10 && i < 13)
            {
                if (i % 2 == 0)
                {
                    string dziwak = times[(i / 2) + 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 1)
                {
                    string dziwak = times1[(i / 2) - 1].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 13 && i < 17)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
            else if (i >= 17 && i < 20)
            {
                if (i % 2 == 1)
                {
                    string dziwak = times[(i / 2) + 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
                else if (i % 2 == 0)
                {
                    string dziwak = times1[(i / 2) - 2].InnerText.Trim();
                    tab[i] = dziwak;
                }
            }
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
    public static string[] GettingDistances()
    {
        string url = "https://www.swimrankings.net/index.php?page=rankingDetail&club=POL";
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var Distance = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList0']//td[@class='swimstyle']//a");
        var Distance1 = htmlDocument.DocumentNode.SelectNodes("//tr[@class='rankingList1']//td[@class='swimstyle']//a");
        string[] tab = new string[18];
        for (int i = 0; i < tab.Length; i++)
        {
            if (i % 2 == 0)
            {
                string dziwak = Distance[i / 2].InnerText.Trim();
                tab[i] = dziwak;
            }
            else if (i % 2 == 1)
            {
                string dziwak = Distance1[i / 2].InnerText.Trim();
                tab[i] = dziwak;
            }
        }
        return tab;

    }

    public static int[] GetPkt(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var times = htmlDocument.DocumentNode.SelectNodes("//tr[@class='athleteBest0']//td[@class='code']");
        int[] tab = new int[times.Count];
        int inter = 0;
        for (int i = 0; i < tab.Length; i++)
        {

            string newby = Convert.ToString(times[i].InnerText.Trim().Replace("-", ""));
            if (string.IsNullOrEmpty(newby))
            {

            }
            else
            {
                tab[inter] = Convert.ToInt32(newby);
                inter++;
            }
        }
        return tab;

    }
    public static string[] GetDistance(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
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
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var times = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='name']");
        string name = times.InnerText.Trim().Replace("(2009&nbsp;&nbsp;)", "").Replace(",", "");
        return name;
    }
}


