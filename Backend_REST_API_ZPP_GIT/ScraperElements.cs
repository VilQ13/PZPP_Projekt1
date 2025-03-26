using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ScraperElementGroup
{
    public string Faculty { get; set; }
    public string Form { get; set; }
    public string Course { get; set; }
    public string Degree { get; set; }
    public string Semester { get; set; }
    public string Specialization { get; set; }
    public string Group { get; set; }
    public string ExactGroup { get; set; }
}

public static class ScraperElements
{
    // Список групп элементов с соответствующими параметрами
    public static readonly List<ScraperElementGroup> ElementGroups = new List<ScraperElementGroup>
    {
        new ScraperElementGroup
        {
            Faculty = "//li[@id='6153']//img[@src='images/plus1.gif']",
            Form = "img_6941",
            Course = "img_6971",
            Degree = "img_59828",
            Semester = "img_12628",
            Specialization = "img_126133",
            Group = "img_99440",
            ExactGroup = "//a[text()='Inf/S/Ist/6sem/1gr/a/IO']"
        },
        // Дополнительные группы элементов могут быть добавлены по мере необходимости
    };

    // Метод для поиска группы элемента по exactGroup
    public static ScraperElementGroup GetElementGroupByExactGroup(string exactGroup)
    {
        Console.WriteLine($"Searching for exactGroup: '{exactGroup}'");

        // Проходим по всем элементам и сравниваем их ExactGroup с переданным exactGroup
        foreach (var group in ElementGroups)
        {
            // Извлекаем текст из XPath в ExactGroup
            string cleanedExactGroup = ExtractInnerText(group.ExactGroup);
            Console.WriteLine($"Available group (cleaned): '{cleanedExactGroup}'");

            // Сравниваем извлеченный текст с exactGroup
            if (cleanedExactGroup.Equals(exactGroup.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return group;
            }
        }

        return null;
    }

    // Метод для извлечения текста из XPath (например, //a[text()='...'])
    private static string ExtractInnerText(string xpath)
    {
        // Исправленное регулярное выражение для извлечения текста
        var match = Regex.Match(xpath, @"//a\[text\(\)='(.+?)'\]");
        return match.Success ? match.Groups[1].Value : xpath;
    }
}

