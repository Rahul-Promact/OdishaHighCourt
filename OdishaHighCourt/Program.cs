using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Patagames.Ocr.Exceptions;
using Patagames.Ocr;
namespace OdishaHighCourt;


class Program
{
    static async Task Main(string[] args)
    {
        // Set up Chrome options
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\CourtDetails\OdishaCourt"); // Set your download folder path
        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false); // Disable download prompt
        chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);


        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {

            string url = "https://hcservices.ecourts.gov.in/ecourtindiaHC/cases/s_order.php?state_cd=11&dist_cd=1&court_code=1&stateNm=Odisha"; // Replace with your URL
            int maxRetries = 3;
            int retryCount = 0;
            bool success = false;

            // Retry navigation in case of failure
            while (retryCount < maxRetries && !success)
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    success = true;
                }
                catch (WebDriverException ex)
                {
                    Console.WriteLine($"Error navigating to URL: {ex.Message}");
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        Console.WriteLine("Retrying...");
                        await Task.Delay(3000); // Wait for 3 seconds before retrying
                    }
                    else
                    {
                        Console.WriteLine("Max retries reached. Exiting...");
                    }
                }
            }

            if (success)
            {

                string[] judgeNames = new string[]
                     {
                           
                           
                         
                            "MR. JUSTICE BISWANATH RATH",
                            "MR. JUSTICE SUJIT NARAYAN PRASAD",
                            "MR. JUSTICE S.K.SAHOO",
                            "MR. JUSTICE K.R.MOHAPATRA",
                            "MR. JUSTICE J.P.DAS",
                            "DR. JUSTICE D.P.CHOUDHURY",
                            "DR. JUSTICE A.K.MISHRA",
                            "MR. JUSTICE B. P. ROUTRAY",
                            "DR. JUSTICE S.K. PANIGRAHI",
                            "MISS JUSTICE SAVITRI RATHO",
                            "MR. JUSTICE M.S.SAHOO",
                            "MR. JUSTICE R.K.PATTANAIK",
                            "MR. JUSTICE SASHIKANTA MISHRA",
                            "MR. JUSTICE ADITYA KUMAR MOHAPATRA",
                            "MR. JUSTICE V. NARASINGH",
                            "MR. JUSTICE BIRAJA PRASANNA SATAPATHY",
                            "MR. JUSTICE MURAHARI SRI RAMAN",
                            "MR. JUSTICE SANJAY KUMAR MISHRA",
                            "MR. JUSTICE GOURISHANKAR SATAPATHY",
                            "MR. JUSTICE CHITTARANJAN DASH",
                            "MR. JUSTICE SIBO SANKAR MISHRA",
                            "MR. JUSTICE ANANDA CHANDRA BEHERA",
                            "MR. JUSTICE C.R.DASH",
                            "DR. JUSTICE A.K.RATH",
                            "MR. JUSTICE A.PASAYAT",
                            "MR. JUSTICE MOHAMMAD RAFIQ(CJ)",
                            "KUMARI JUSTICE SANJU PANDA",
                            "MR. JUSTICE K.P.MOHAPATRA",
                            "MR. JUSTICE PRAMATH PATNAIK",
                            "MR. JUSTICE S.K.MISHRA",
                            "MR. JUSTICE HARILAL AGRAWAL",
                            "MR. JUSTICE JUDGE-1",
                            "MR. JUSTICE JUDGE-2",
                            "MR JUSTICE S P MOHAPATRA",
                            "MR. JUSTICE G.K.MISHRA",
                            "MR. JUSTICE J.P. DAS (RETD.)",
                            "MR JUSTICE G C DAS",
                            "DR. JUSTICE A.K. MISHRA (RETD.)",
                            "MR. JUSTICE B.J.DAS",
                            "MR.JUSTICE SUKANTA KISHORE RAY"
                     };

                int judgeIndex = 0;



                while (judgeIndex < judgeNames.Length)
                {
                    DateTime today = DateTime.Today;
                    DateTime oneMonthAgo = today.AddMonths(-1);
                    string fromDate = "01-09-1954";  // Adjust as needed
                    string toDate = "16-09-2024";    // Adjust as needed

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    // Select the judge from the dropdown (note this is a search box)
                    SelectElement judgeDropdown = new SelectElement(wait.Until(d => d.FindElement(By.Id("nnjudgecode1"))));
                    judgeDropdown.SelectByText(judgeNames[judgeIndex]);  // Enter the judge's name



                    // Fill the 'from_date' field
                    IWebElement fromDateElement = wait.Until(d => d.FindElement(By.Id("from_date")));
                    fromDateElement.Clear();
                    fromDateElement.SendKeys(fromDate);

                    // Fill the 'to_date' field
                    IWebElement toDateElement = wait.Until(d => d.FindElement(By.Id("to_date")));
                    toDateElement.Clear();
                    toDateElement.SendKeys(toDate);

                    // Click anywhere on the page to close date picker (optional)
                    IWebElement bodyElement = driver.FindElement(By.TagName("body"));
                    bodyElement.Click();

                    // Select "Yes" for Reportable Judgements
                    SelectElement reportableJudgesDropdown = new SelectElement(wait.Until(d => d.FindElement(By.Id("reportableJudges"))));
                    reportableJudgesDropdown.SelectByValue("Y");

                    // Select "Judgment" for Type of Orders
                    SelectElement typeOfOrdersDropdown = new SelectElement(wait.Until(d => d.FindElement(By.Id("typeOfOrders"))));
                    typeOfOrdersDropdown.SelectByValue("31");

                    // Handle the CAPTCHA
                    bool captchaCracked = false;

                    while (!captchaCracked)
                    {


                       
                            // Capture the CAPTCHA image element
                            IWebElement captchaImageElement = wait.Until(d => d.FindElement(By.Id("captcha_image")));

                            // Capture a screenshot of the entire page
                            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                            string captchaImagePath = string.Empty;
                            using (var fullImage = new Bitmap(new MemoryStream(screenshot.AsByteArray)))
                            {
                                // Get the location and size of the CAPTCHA element
                                var elementLocation = captchaImageElement.Location;
                                var elementSize = captchaImageElement.Size;

                                // Create a new bitmap with the size of the CAPTCHA image
                                using (var elementScreenshot = new Bitmap(elementSize.Width, elementSize.Height))
                                {
                                    using (var graphics = Graphics.FromImage(elementScreenshot))
                                    {
                                        // Draw the portion of the screenshot that corresponds to the CAPTCHA element
                                        graphics.DrawImage(fullImage, new Rectangle(0, 0, elementSize.Width, elementSize.Height),
                                            new Rectangle(elementLocation.X, elementLocation.Y, elementSize.Width, elementSize.Height),
                                            GraphicsUnit.Pixel);
                                    }

                                    // Save the CAPTCHA image to a file
                                    string projectDirectory = Directory.GetCurrentDirectory();
                                    string imgFolderPath = System.IO.Path.Combine(projectDirectory, "img");

                                    if (!Directory.Exists(imgFolderPath))
                                    {
                                        Directory.CreateDirectory(imgFolderPath);
                                    }

                                    captchaImagePath = System.IO.Path.Combine(imgFolderPath, "captcha.png");
                                    elementScreenshot.Save(captchaImagePath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }

                            // Process the CAPTCHA image to get the text (you need to implement this OCR function)
                            string captchaText = ScanTextFromImage(captchaImagePath);

                            // Fill the CAPTCHA input field
                            IWebElement captchaInputElement = wait.Until(d => d.FindElement(By.Id("captcha")));
                            captchaInputElement.SendKeys(captchaText);

                            // Locate the submit button and submit the form
                            IWebElement submitButton = wait.Until(d => d.FindElement(By.XPath("//input[@name='submit1']")));

                            // Scroll into view to ensure it's visible
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
                            Thread.Sleep(1000);

                            // Click the button to submit the form
                            submitButton.Click();


                            // Wait for the CAPTCHA field to clear, indicating a success
                            wait.Until(d => string.IsNullOrEmpty(d.FindElement(By.Id("captcha")).GetAttribute("value")));




                            // You can add more logic here to verify if the CAPTCHA was correct, e.g., checking if you're redirected
                            await Task.Delay(1000);

                            try
                            {
                                // Check if the table with id="showList3" is present (CAPTCHA solved and records found)
                                try
                                {
                                    IWebElement tableElement = wait.Until(d => d.FindElement(By.Id("showList3")));
                                    Thread.Sleep(1000);
                                    if (tableElement.Displayed)
                                    {
                                        Console.WriteLine("CAPTCHA solved and records found for the judge.");

                                        // Extract table headings
                                        var headings = tableElement.FindElement(By.TagName("thead"))
                                                                   .FindElement(By.TagName("tr"))
                                                                   .FindElements(By.TagName("th"))
                                                                   .Select(th => th.Text.Trim())
                                                                   .ToArray();

                                        // Extract table rows
                                        var rows = tableElement.FindElement(By.TagName("tbody"))
                                                               .FindElements(By.TagName("tr"));

                                        var tableData = new List<Dictionary<string, string>>();

                                        foreach (var row in rows)
                                        {
                                            var casedetail = new caseDetail();
                                            casedetail.Court = "Orissa High Court";
                                            casedetail.Abbr = "OHC";
                                            var cells = row.FindElements(By.TagName("td"));
                                            if (cells.Count >= 4)
                                            {
                                                var pdfLink = cells[3].FindElement(By.TagName("a")).GetAttribute("href");
                                                var pdfLinkElement = cells[3].FindElement(By.TagName("a"));
                                                var caseNo = cells[1].Text.Trim();
                                                var rowData = new Dictionary<string, string>
                                            {
                                                { headings[0], cells[0].Text.Trim() },
                                                { headings[1], cells[1].Text.Trim() },
                                                { headings[2], cells[2].Text.Trim() },
                                                { headings[3], cells[3].FindElement(By.TagName("a")).GetAttribute("href") }
                                            };
                                                var caseInfo = rowData["Case Type/Case Number/Case Year"].Split('/');

                                                if (caseInfo.Length == 3)
                                                {
                                                    casedetail.Type = rowData["Case Type/Case Number/Case Year"];
                                                    casedetail.CaseNo = $"WA {caseInfo[1]} of {caseInfo[2]}";
                                                }
                                                string orderDate = rowData["Order Date"];
                                                // Parse the date from dd-MM-yyyy format
                                                DateTime parsedDate = DateTime.ParseExact(orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                                                // Format the date to MM-dd-yyyy for database insertion
                                                string formattedDate = parsedDate.ToString("MM-dd-yyyy");
                                                var options = new JsonSerializerOptions
                                                {
                                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                };

                                                string currentJudgeName = judgeNames[judgeIndex];

                                                // Remove unwanted characters or perform necessary replacements
                                                string cleanJudgeName = currentJudgeName.Replace("\u0027", "'");

                                                // Add the cleaned name to the list
                                                var members = new List<string> { cleanJudgeName };

                                                // Serialize the list to a JSON string with custom options
                                                string jsonMembers = JsonSerializer.Serialize(members, options);
                                                casedetail.Coram = jsonMembers;
                                                casedetail.CoramCount = members?.Count ?? 0;
                                                casedetail.Dated = formattedDate;
                                                casedetail.Reportable = true;

                                                // Print the key-value pairs for the row
                                                foreach (var kvp in rowData)
                                                {
                                                    Console.WriteLine($"{kvp.Key} - {kvp.Value}");
                                                }
                                                Console.WriteLine(); // New line for better readability

                                                // Add the row data to the list
                                                tableData.Add(rowData);



                                                string downloadDirectory = @"C:\CourtDetails\OdishaCourt";
                                                string sanitizedCaseNo = caseNo.Replace("/", "_"); // Replace / with _ or any valid character
                                                string newFileName = $"{sanitizedCaseNo}_{cleanJudgeName}.pdf";


                                                var existingFiles = new HashSet<string>(Directory.GetFiles(downloadDirectory));

                                                // Click the PDF link to initiate download
                                                pdfLinkElement.Click();

                                                string downloadedFile = null;

                                            int retryLimit = 10; // Maximum number of retries (20 seconds in this case)
                                            int retrycount = 0;
                                            int delayInMilliseconds = 2000; // 2-second delay

                                            while (downloadedFile == null && retrycount < retryLimit)
                                            {
                                                // Check for any new files in the download directory
                                                var currentFiles = Directory.GetFiles(downloadDirectory);
                                                var newFiles = currentFiles.Except(existingFiles).ToList();

                                                if (newFiles.Count > 0)
                                                {
                                                    // Get the latest new file
                                                    downloadedFile = newFiles.OrderByDescending(f => new FileInfo(f).CreationTime).FirstOrDefault();

                                                    // Ensure the file is fully downloaded
                                                    if (downloadedFile != null && IsFileLocked(downloadedFile))
                                                    {
                                                        downloadedFile = null; // File is still being written, so wait
                                                    }
                                                }

                                                retrycount++;
                                                await Task.Delay(delayInMilliseconds); // Wait for 2 seconds before checking again
                                            }

                                            // If no file is found after retry limit
                                            if (downloadedFile == null)
                                            {
                                                Console.WriteLine("No new file was downloaded after waiting for the specified duration.");
                                            }
                                            else
                                            {
                                                // Wait until the file is fully downloaded and not locked (if found)
                                                retrycount = 0; // Reset retry count for this part
                                                while (IsFileLocked(downloadedFile) && retrycount < retryLimit)
                                                {
                                                    await Task.Delay(delayInMilliseconds); // Keep waiting if the file is still locked (downloading)
                                                    retrycount++;
                                                }

                                                if (IsFileLocked(downloadedFile))
                                                {
                                                    Console.WriteLine("The file is still locked after waiting for the specified duration.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("File download completed and is not locked.");
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(downloadedFile) && !IsFileLocked(downloadedFile))
                                                {
                                                    string newFilePath = System.IO.Path.Combine(downloadDirectory, newFileName);
                                                    int renameRetries = 3; // Retry renaming up to 3 times

                                                    while (renameRetries > 0)
                                                    {
                                                        try
                                                        {
                                                            if (!File.Exists(newFilePath))
                                                            {
                                                                // Attempt to rename the file
                                                                File.Move(downloadedFile, newFilePath); // Rename the file
                                                                Thread.Sleep(2000);
                                                                Console.WriteLine($"Downloaded file renamed to: {newFilePath}");

                                                                // Once the file is confirmed at the new path, update the link
                                                                casedetail.PdfLink = newFilePath;
                                                                Console.WriteLine($"File is successfully renamed and available at: {newFilePath}");
                                                                break; // Break if renaming is successful
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine($"File already exists at {newFilePath}. Skipping renaming.");
                                                                casedetail.PdfLink = newFilePath;
                                                                break; // File exists, so exit the loop
                                                            }
                                                        }
                                                        catch (IOException ex)
                                                        {
                                                            Console.WriteLine($"Error renaming file: {ex.Message}. Retrying...");
                                                            renameRetries--;
                                                            await Task.Delay(2000); // Wait before retrying
                                                        }
                                                    }

                                                    if (renameRetries == 0)
                                                    {
                                                        Console.WriteLine($"Failed to rename the file after multiple attempts: {downloadedFile}");
                                                    }



                                                    casedetail.Reportable = true;

                                                    using (var db = new AppDbContext())
                                                    {
                                                        bool exists = db.CaseDetails.Any(cd =>
                                                             cd.CaseNo == casedetail.CaseNo &&
                                                             cd.Dated == casedetail.Dated &&
                                                             cd.Coram == casedetail.Coram &&
                                                             cd.Type == casedetail.Type
                                                             );

                                                        if (!exists)
                                                        {
                                                            // Add and save if no existing record is found
                                                            db.CaseDetails.Add(casedetail);
                                                            db.SaveChanges();
                                                            Console.WriteLine("New case detail added.");
                                                        }
                                                        else
                                                        {
                                                            // Skip if record already exists
                                                            Console.WriteLine("Case detail already exists. Skipping insert.");
                                                        }
                                                    }



                                                }
                                                else
                                                {
                                                    Console.WriteLine("No downloaded file found.");
                                                }


                                            }
                                        }



                                        //database operation 

                                        captchaCracked = true; // Move to the next judge
                                    }
                                    else
                                    {
                                        IWebElement txtMsgElement = wait.Until(d => d.FindElement(By.Id("txtmsg")));
                                        string textValue = txtMsgElement.GetAttribute("value");

                                        // Handle "Invalid Captcha"
                                        if (textValue == "Invalid Captcha")
                                        {
                                            Console.WriteLine("Invalid CAPTCHA detected, retrying...");
                                            await Task.Delay(1000); // Small delay before retrying
                                        }
                                        // Handle "No records found" (CAPTCHA cracked but no records for the judge)
                                        else if (textValue == "Record not found")
                                        {
                                            Console.WriteLine("CAPTCHA cracked but no records found for the judge. Moving to next judge...");
                                            captchaCracked = true; // Proceed to the next judge
                                        }
                                    }
                                }
                                catch (WebDriverTimeoutException)
                                {

                                    Console.WriteLine("Unexpected error: No element found.");

                                }
                            }
                            catch (NoSuchElementException)
                            {
                                Console.WriteLine("Unexpected error: No element found.");
                            }
                            Thread.Sleep(2000);
                      
                       

                        // Move to the next judge if CAPTCHA is solved
                        if (captchaCracked)
                        {
                            judgeIndex++;
                        }
                    }
                    if (judgeIndex >= judgeNames.Length)
                    {
                        Console.WriteLine("All judges processed.");
                    }

                }






            }
        }
    }

    private static bool IsFileLocked(string filePath)
    {
        FileStream stream = null;

        try
        {
            stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException)
        {
            // The file is locked (still being downloaded)
            return true;
        }
        finally
        {
            stream?.Close();
        }

        // The file is not locked, meaning it's fully downloaded
        return false;
    }

    private static string GetLatestDownloadedFile(string downloadDirectory)
    {
        var directoryInfo = new DirectoryInfo(downloadDirectory);

        // Get the most recent PDF file
        var file = directoryInfo.GetFiles("*.pdf")
                                .OrderByDescending(f => f.LastWriteTime)
                                .FirstOrDefault();

        return file?.FullName;
    }



    private static string ScanTextFromImage(string imagePath)
    {
        if (!System.IO.File.Exists(imagePath))
        {
            Console.WriteLine("Image file not found.");
            return string.Empty;
        }

        try
        {
            // Preprocess the image to enhance OCR accuracy (grayscale, resize)
            string preprocessedImagePath = PreprocessImage(imagePath);

            using (var objOcr = OcrApi.Create())
            {
                // Initialize the OCR engine with the English language (detects alphabets and numbers by default)
                objOcr.Init(Patagames.Ocr.Enums.Languages.English);

                // Extract and return the text from the preprocessed image
                string plainText = objOcr.GetTextFromImage(preprocessedImagePath);
                string cleanedText = System.Text.RegularExpressions.Regex.Replace(plainText, @"[^a-zA-Z0-9]", "");

                return string.IsNullOrEmpty(cleanedText) ? "No text found." : cleanedText.Length > 5 ? cleanedText.Substring(0, 5) : cleanedText;
            }
        }
        catch (OcrException ocrEx)
        {
            Console.WriteLine($"OCR error occurred: {ocrEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return string.Empty;
    }

    private static string PreprocessImage(string imagePath)
    {
        // Define the directory for preprocessed images
        string directoryPath = System.IO.Path.GetDirectoryName(imagePath);
        string preprocessedPath = System.IO.Path.Combine(directoryPath, "preprocessed_" + System.IO.Path.GetFileName(imagePath));

        try
        {
            using (Bitmap originalImage = new Bitmap(imagePath))
            {
                // Convert the image to grayscale for better contrast
                using (Bitmap grayscaleImage = ConvertToGrayscale(originalImage))
                {
                    // Optionally resize the image to improve small text detection
                    using (Bitmap resizedImage = ResizeImage(grayscaleImage, 2.0)) // Resize 2x
                    {
                        // Save the preprocessed image
                        resizedImage.Save(preprocessedPath, ImageFormat.Png);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during image preprocessing: {ex.Message}");
        }

        return preprocessedPath;
    }

    /// <summary>
    /// Converts an image to grayscale to reduce noise and improve OCR accuracy.
    /// </summary>
    private static Bitmap ConvertToGrayscale(Bitmap originalImage)
    {
        Bitmap grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);

        for (int x = 0; x < originalImage.Width; x++)
        {
            for (int y = 0; y < originalImage.Height; y++)
            {
                Color originalColor = originalImage.GetPixel(x, y);
                int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                grayscaleImage.SetPixel(x, y, grayColor);
            }
        }

        return grayscaleImage;
    }

    /// <summary>
    /// Resizes the image by the given scale factor to help the OCR engine detect smaller text.
    /// </summary>
    private static Bitmap ResizeImage(Bitmap originalImage, double scale)
    {
        int newWidth = (int)(originalImage.Width * scale);
        int newHeight = (int)(originalImage.Height * scale);
        Bitmap resizedImage = new Bitmap(newWidth, newHeight);

        using (Graphics g = Graphics.FromImage(resizedImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }

        return resizedImage;
    }

}