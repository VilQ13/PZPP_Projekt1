using System;
using System.Net;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using UglyToad.PdfPig;
using System.IO;

class Program
{
    static void Main()
    {
        IWebDriver driver = null;

        try
        {
            // Inicjalizacja ChromeDriver
            driver = new ChromeDriver();
            Console.WriteLine("Przeglądarka została uruchomiona.");
            driver.Navigate().GoToUrl("https://plany.ubb.edu.pl/");
            Console.WriteLine("Strona została załadowana.");

            // Czekamy na załadowanie strony
            Thread.Sleep(2000);  // Czekamy 2 sekundy na załadowanie strony

            // Przełączamy się na frame zawierający 'left_menu.php'
            Console.WriteLine("Przechodzimy do frame 'left_menu.php'.");
            driver.SwitchTo().Frame(driver.FindElement(By.XPath("//frame[contains(@src, 'left_menu.php')]")));
            Console.WriteLine("Przełączono do frame 'left_menu.php'.");

            // Czekamy chwilę, aby elementy w frame były dostępne
            Thread.Sleep(1000);  // Czekamy 1 sekundę na załadowanie frame

            // Czekamy na link "Nauczyciele" i klikamy go
            IWebElement nauczycieleLink = driver.FindElement(By.XPath("//area[@alt='Nauczyciele']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", nauczycieleLink);

            // Czekamy chwilę na przeładowanie strony po kliknięciu
            Thread.Sleep(2000);  // Czekamy 2 sekundy na załadowanie nowej strony

            // Tworzymy obiekt WebDriverWait, aby czekać na element
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));  // Ustawiamy czas oczekiwania na 5 sekund

            // Czekamy aż element będzie widoczny i dostępny do kliknięcia
            IWebElement plusik = wait.Until(d => d.FindElement(By.XPath("//li[@id='6168']//img[@src='images/plus1.gif']")));

            // Przewijamy stronę do elementu (żeby upewnić się, że jest widoczny)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", plusik);

            // Klikamy na obrazek za pomocą JavaScript
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", plusik);

            Thread.Sleep(2000);

            // Czekamy aż element z obrazkiem 'images/plus.gif' będzie dostępny
            IWebElement drugiPlusik = wait.Until(d => d.FindElement(By.Id("img_30847")));

            // Klikamy na obrazek (plusik) za pomocą JavaScript
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", drugiPlusik);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);  // Czekamy 2 sekundy, aby zobaczyć efekt

            // Klikamy na wykładowcę "Bernaś Marcin"
            IWebElement wykładowca = wait.Until(d => d.FindElement(By.XPath("//a[text()='Bernaś Marcin']")));
            wykładowca.Click();
            Thread.Sleep(2000);

            // Upewniamy się, że wracamy do głównego frame po kliknięciu w wykładowcę
            Console.WriteLine("Przełączamy się na główny frame 'main.php'.");

            // Teraz przełączamy się na frame zawierający 'main.php'
            driver.SwitchTo().DefaultContent();  // Wracamy do głównej zawartości strony
            WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Spróbuj znaleźć iframe po src, a nie name
            IWebElement mainFrame = wait.Until(d =>
                d.FindElement(By.XPath("//frame[contains(@src, 'main.php')]")));  // Użycie 'src' zamiast 'name'
            driver.SwitchTo().Frame(mainFrame);
            Console.WriteLine("Przełączono do frame 'main.php'.");

            // Wybieramy opcję z listy 'wBWeekDef' (L Stacjonarne)
            IWebElement selectElement = driver.FindElement(By.Id("wBWeekDef"));
            SelectElement select = new SelectElement(selectElement);
            select.SelectByValue("169");  // Wybieramy (L) Stacjonarne
            Console.WriteLine("Wybrano opcję 'L Stacjonarne'.");

            // Czekamy chwilę, aż opcje się załadują
            Thread.Sleep(2000);

            // Wybieramy opcję z listy 'wBWeek'
            IWebElement weekSelectElement = driver.FindElement(By.Id("wBWeek"));
            SelectElement weekSelect = new SelectElement(weekSelectElement);
            weekSelect.SelectByValue("0");  // Wybieramy opcję '- - - - - - - - - - -'
            Console.WriteLine("Wybrano opcję '- - - - - - - - - - -'.");

            Thread.Sleep(2000);

            // Znajdujemy przycisk "Wyświetl" i klikamy go
            IWebElement wyswietlButton = wait.Until(d => d.FindElement(By.Id("wBButton")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", wyswietlButton);
            Console.WriteLine("Kliknięto przycisk 'Wyświetl'.");

            Thread.Sleep(2000);

            // Przewijamy do elementu "Lista"
            IWebElement lastElement = driver.FindElement(By.XPath("//a[contains(text(), 'Lista')]"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", lastElement);
            Console.WriteLine("Przewinięto do elementu 'Lista'.");

            // Czekamy, aż element 'Lista' będzie dostępny i klikamy
            WebDriverWait wait3 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement listaLink = wait3.Until(d => d.FindElement(By.XPath("//a[contains(@href, 'plan.php') and contains(text(), 'Lista')]")));

            listaLink.Click();
            Console.WriteLine("Kliknięto 'Lista'.");

            // Czekamy 10-15 sekund na załadowanie nowej strony
            Thread.Sleep(15000);  // Możesz dostosować czas jeśli strona ładuje się szybciej

            Console.WriteLine("Odczekano 15 sekund na załadowanie strony.");

            string pdfUrl = "https://plany.ubb.edu.pl/plan.php?type=10&id=112201&emural=true&pdf=true&wd=169";

            // Pobranie pliku PDF
            WebClient webClient = new WebClient();
            string pdfFilePath = "plan.pdf";
            webClient.DownloadFile(pdfUrl, pdfFilePath);
            Console.WriteLine("Pobrano plik PDF.");

            // Odczytanie zawartości PDF
            using (PdfDocument document = PdfDocument.Open(pdfFilePath))
            {
                string allText = string.Empty;
                foreach (var page in document.GetPages())
                {
                    string text = page.Text;
                    allText += $"Strona {page.Number}:\n{text}\n";
                }

                // Pobieramy ścieżkę folderu projektu
                string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Tworzymy pełną ścieżkę do pliku plan.txt w folderze projektu
                string filePath = Path.Combine(projectDirectory, "plan.txt");

                // Zapisz zawartość do pliku tekstowego w folderze projektu
                File.WriteAllText(filePath, allText);
                Console.WriteLine($"Dane z pliku PDF zostały zapisane do pliku '{filePath}'.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Wystąpił błąd: " + ex.Message);
        }
        finally
        {
            // Sprawdzamy, czy driver został zainicjowany przed zamknięciem
            if (driver != null)
            {
                driver.Quit();
            }
        }
    }
}
