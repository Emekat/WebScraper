using System.Net;
using HtmlAgilityPack;

var cookieContainer = new CookieContainer();
var handler = new HttpClientHandler
{
    CookieContainer = cookieContainer,
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    UseCookies = true,
    AllowAutoRedirect = true
};

using var client = new HttpClient(handler);

// Add browser-like headers
client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

// Request and check status without throwing
var response = await client.GetAsync("https://emekatorti.com/wp-login.php");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
    return;
}

var loginPage = await response.Content.ReadAsStringAsync();
var doc = new HtmlDocument();
doc.LoadHtml(loginPage);

// extract hidden token
var csrfToken = doc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']")
    ?.GetAttributeValue("value", "");

var formContent = new Dictionary<string, string>
{
    ["username"] = "username@username.com",
    ["password"] = "password!",
    ["__RequestVerificationToken"] = csrfToken,
};
var content = new FormUrlEncodedContent(formContent);

var mainPage = await client.PostAsync("https://emekatorti.com/wp-login.php", content);

Console.WriteLine(mainPage.StatusCode);