// Replace these with real values from authenticated buckler site requests
const string urlToken = "AAAAAAAAAAAAAAAAAAAAA";
const string bucklerId = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB-CCCCCCCCCCCCCCCCCCCCCCC";
const string bucklerRId = "00000000-0000-0000-0000-000000000000";

var getUrl = (int league, int page) =>
{ return $"https://www.streetfighter.com/6/buckler/_next/data/{urlToken}/en/ranking/league.json?character_filter=1&character_id=luke&platform=1&user_status=1&home_filter=1&home_category_id=0&home_id=1&league_rank={league}&page={page}"; };

var handler = new HttpClientHandler()
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
};

HttpClient client = new HttpClient(handler);

for (int i = 1; i <= 36; i++)
{
    // Initialize this at a non=zero value so the loop doesn't immdiately exit.
    // Gets set to the real value within the loop.
    int pages = 10;
    for (int j = 1; j <= pages; j++)
    {
        var url = getUrl(i, j);

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Add("Cookie", $"buckler_id={bucklerId}; buckler_r_id={bucklerRId}; buckler_praise_date=1701090492787");
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
        request.Headers.Add("Cache-Content", "no-cache");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Host", "www.streetfighter.com");
        request.Headers.Add("Pragma", "no-cache");
        request.Headers.Add("Referer", "https://www.streetfighter.com/6/buckler/ranking/league?character_filter=1&character_id=luke&platform=1&user_status=1&home_filter=1&home_category_id=0&home_id=1&league_rank=12&page=2957");
        request.Headers.Add("Sec-Fetch-Dest", "empty");
        request.Headers.Add("Sec-Fetch-Mode", "cors");
        request.Headers.Add("Sec-Fetch-Site", "same-origin");
        request.Headers.Add("TE", "trailers");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0");

        var result = client.SendAsync(request).Result;
        var content = result.Content.ReadAsStringAsync().Result;
        var jsonDoc = System.Text.Json.Nodes.JsonNode.Parse(content);

        pages = jsonDoc!["pageProps"]!["league_point_ranking"]!["total_page"]!.GetValue<int>();

        var list = jsonDoc!["pageProps"]!["league_point_ranking"]!["ranking_fighter_list"];

        foreach (var item in list!.AsArray())
        {
            File.AppendAllText($"playerdump-rank{i:D2}.jsonl", System.Text.Json.JsonSerializer.Serialize(item) + Environment.NewLine);
        }
        Console.WriteLine($"Processed rank {i} page {j} of {pages}.");
    }
}









