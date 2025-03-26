using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Backend_REST_API_ZPP.Models;

[ApiController]
[Route("api/scraper")]
public class ScraperController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ScraperController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("scrape")]
    public async Task<IActionResult> ScrapeData([FromForm] string exactGroup)
    {
        if (string.IsNullOrEmpty(exactGroup))
        {
            return BadRequest("Exact group is required");
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient(); // Создаем экземпляр HttpClient
            Scraper scraper = new Scraper(httpClient); // Передаем HttpClient в Scraper
            var result = await scraper.StartScraping("", "", "", "", "", "", "", exactGroup);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred: {ex.Message}");
        }
    }
}

public class Scraper
{
    private readonly HttpClient _httpClient;

    public Scraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Course>> StartScraping(
        string faculty, string form, string courseParam, string degree,
        string semester, string specialization, string group, string exactGroup)
    {
        IWebDriver driver = InitializeWebDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        try
        {
            driver.Navigate().GoToUrl("https://plany.ubb.edu.pl/");
            Thread.Sleep(2000);

            var elementGroup = ScraperElements.GetElementGroupByExactGroup(exactGroup);

            if (elementGroup == null)
            {
                Console.WriteLine($"No matching element group found for exactGroup: {exactGroup}");
                throw new Exception("Matching exactGroup not found.");
            }

            driver.SwitchTo().Frame(driver.FindElement(By.XPath("//frame[contains(@src, 'left_menu.php')]")));

            ClickElementByXPath(driver, wait, elementGroup.Faculty);
            ClickElementById(driver, wait, elementGroup.Form);
            ClickElementById(driver, wait, elementGroup.Course);
            ClickElementById(driver, wait, elementGroup.Degree);
            ClickElementById(driver, wait, elementGroup.Semester);
            ClickElementById(driver, wait, elementGroup.Specialization);
            ClickElementById(driver, wait, elementGroup.Group);

            IWebElement link = wait.Until(d => d.FindElement(By.XPath($"//a[text()='{exactGroup}']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", link);
            Thread.Sleep(2000);

            driver.SwitchTo().DefaultContent();
            SwitchToFrame(driver, wait, "//frame[contains(@src, 'main.php')]");

            // Parse the courses
            Dictionary<string, string> subjectMap = ParseSubjectData(driver);
            var coursesList = ParseLectureCourses(driver, subjectMap);

            // Now update teacher's name by fetching full name using abbreviation
            foreach (var courseItem in coursesList)
            {
                if (!string.IsNullOrEmpty(courseItem.Teacher))
                {
                    // Fetch full name of the teacher from abbreviation
                    courseItem.Teacher = await GetTeacherFullNameByAbbreviation(courseItem.Teacher);
                }
            }

            return coursesList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<Course>();
        }
        finally
        {
            driver.Quit();
        }
    }

    private IWebDriver InitializeWebDriver()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        return new ChromeDriver();
    }

    // Helper methods for clicking elements, etc.
    static void ClickElementByXPath(IWebDriver driver, WebDriverWait wait, string xpath)
    {
        IWebElement element = wait.Until(d => d.FindElement(By.XPath(xpath)));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        Thread.Sleep(2000);
    }

    static void ClickElementById(IWebDriver driver, WebDriverWait wait, string elementId)
    {
        IWebElement element = wait.Until(d => d.FindElement(By.Id(elementId)));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        Thread.Sleep(2000);
    }

    static void SwitchToFrame(IWebDriver driver, WebDriverWait wait, string xpath)
    {
        IWebElement frame = wait.Until(d => d.FindElement(By.XPath(xpath)));
        driver.SwitchTo().Frame(frame);
    }

    static Dictionary<string, string> ParseSubjectData(IWebDriver driver)
    {
        Dictionary<string, string> subjectMap = new Dictionary<string, string>();
        try
        {
            IWebElement dataDiv = driver.FindElement(By.XPath("//div[@class='data']"));
            string dataText = dataDiv.Text;
            string[] lines = dataText.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains(" - ") && line.Contains("występowanie"))
                {
                    string[] parts = line.Split(" - ", 2);
                    if (parts.Length == 2)
                    {
                        string courseCode = parts[0].Trim();
                        string fullCourseName = parts[1].Trim();
                        subjectMap[courseCode] = fullCourseName;
                    }
                }
            }
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("❌ Nie znaleziono bloku 'data'!");
        }
        return subjectMap;
    }

    static List<Course> ParseLectureCourses(IWebDriver driver, Dictionary<string, string> subjectMap)
    {
        List<Course> coursesList = new List<Course>();
        var courseElements = driver.FindElements(By.XPath("//div[@name='course']"));

        foreach (var courseElement in courseElements) // Переименовано в courseElement
        {
            string text = courseElement.Text.Trim(); // Используем courseElement вместо course

            if (!string.IsNullOrEmpty(text) && text.Contains("wyk"))
            {
                foreach (var entry in subjectMap)
                {
                    if (text.Contains(entry.Key))
                    {
                        text = text.Replace(entry.Key, entry.Key + " - " + entry.Value);
                    }
                }

                text = CleanUpLectureText(text);
                var courseDetails = text.Split(" - ", 2);
                if (courseDetails.Length == 2)
                {
                    string courseCode = courseDetails[0].Trim();
                    string courseName = CleanUpCourseName(courseDetails[1].Trim());
                    string type = ParseTypeFromText(text);

                    string teacher = ExtractTeacherFromCourseName(courseName);

                    if (!string.IsNullOrEmpty(teacher))
                    {
                        courseName = courseName.Replace(teacher, "").Trim();
                    }

                    coursesList.Add(new Course
                    {
                        CourseCode = courseCode,
                        CourseName = courseName,
                        Type = type,
                        CourseType = "wyk",
                        Teacher = teacher
                    });
                }
            }
        }
        return coursesList;
    }

    private async Task<string> GetTeacherFullNameByAbbreviation(string teacherAbbreviation)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5096/api/prowadzacy/get-full-name?abbreviation={teacherAbbreviation}");

            if (response.IsSuccessStatusCode)
            {
                var fullNameData = await response.Content.ReadAsAsync<dynamic>();
                if (fullNameData?.fullName != null)
                {
                    return fullNameData.fullName;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while fetching teacher full name: {ex.Message}");
        }

        return teacherAbbreviation;
    }

    static string CleanUpLectureText(string text)
    {
        return Regex.Replace(text, @"(A|B|L)\d+.*", "").Trim();
    }

    static string CleanUpCourseName(string courseName)
    {
        return Regex.Replace(courseName, @"\s*występowanie:.*|\d{2}:\d{2} - \d{2}:\d{2}.*|\(L\) Stacjonarne|\r\n.*", "").Trim();
    }

    static string ParseTypeFromText(string text)
    {
        var match = Regex.Match(text, @"\(L\) (Stacjonarne|Zdalne)");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    static string ExtractTeacherFromCourseName(string courseName)
    {
        var match = Regex.Match(courseName, @"\n(.*)$");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }
}

public class Course
{
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public string Type { get; set; }
    public string CourseType { get; set; }
    public string Teacher { get; set; }
}
