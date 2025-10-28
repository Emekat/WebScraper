// // See https://aka.ms/new-console-template for more information
//
// var cookieContainer = new System.Net.CookieContainer();
// var handler = new System.Net.Http.HttpClientHandler { CookieContainer = cookieContainer };
//
// var client = new HttpClient(handler);
//
// //Get login page
// var loginPage = await client.GetStringAsync("https://emekatorti.com/");
// var doc = new HtmlAgilityPack.HtmlDocument();
// doc.LoadHtml(loginPage);
//
// //extract hidden token using XPath or CSS selector
// var csrfToken = doc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']")
//     ?.GetAttributeValue("value", "");
// Console.WriteLine("Hello, World!");

// csharp
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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


