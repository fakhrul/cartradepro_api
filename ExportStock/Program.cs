

using System;
using System.IO;
using OfficeOpenXml;

// Specify the path to your PDF file
//string pdfPath = @"C:\Google Drive\01-SAFA INTEGRATED\01-PROJECTS\171-SINAR AUTO SDN BHD\LIST.pdf";


// Specify the path to your Excel file
string excelPath = @"C:\Google Drive\01-SAFA INTEGRATED\01-PROJECTS\171-SINAR AUTO SDN BHD\LIST.xlsx";

// Load the Excel file
ProcessFile(excelPath);

Console.ReadLine();

static void ProcessFile(string excelPath)
{
    string status = "AVAILABLE";
    string brand = "";
    //IDictionary<string, string> fullLineInfoList = new Dictionary<string, string>();
    IList<string> fullLineInfoList = new List<string>();

    FileInfo fileInfo = new FileInfo(excelPath);

    // Make sure EPPlus license context is set (required for non-commercial use)
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using (ExcelPackage package = new ExcelPackage(fileInfo))
    {
        // Loop through all worksheets in the workbook
        foreach (var worksheet in package.Workbook.Worksheets)
        {
            // Get the total number of rows and columns in the current worksheet
            int rows = worksheet.Dimension.Rows;
            int columns = worksheet.Dimension.Columns;

            // Loop through all rows and columns to read the cell values
            for (int row = 1; row <= rows; row++)
            {
                string fullLine = "";

                for (int col = 1; col <= columns; col++)
                {
                    try
                    {
                        string text = "";

                        var cellValue = worksheet.Cells[row, col].Value;

                        if (cellValue != null && DateTime.TryParse(cellValue.ToString(), out DateTime dateValue))
                        {
                            // Format the date before displaying
                            text = dateValue.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            // Display the cell value as text
                            text = cellValue?.ToString();
                        }

                        if(text == null && cellValue != null)
                        {

                        }
                        if (text == null && cellValue == null)
                        {
                            text = "";
                        }
                        //if (text == null)
                        //    text = "";

                        //string text = worksheet.Cells[row, col].Text;

                        text = text.Replace("\n", " ");
                        ////if (string.IsNullOrEmpty(text))
                        ////    break;
                        //if (text.StartsWith("PRICELIST"))
                        //    break;
                        ////else if (text.StartsWith("NO"))
                        ////    break;
                        ////else if (text.StartsWith("Booking"))
                        ////    break;
                        if (text.StartsWith("BOOKING") ||
                            text.StartsWith("LOU") ||
                            text.StartsWith("DUTY TO BE PAID") ||
                            text.StartsWith("INCOMING STOCK")
                            )
                        {
                            status = text;
                            brand = "UNKNOWN";
                            break;
                        }
                        if (text.Equals("DAIHATSU") ||
                            text.Equals("LAND ROVER") ||
                            text.Equals("HONDA") ||
                            text.Equals("LEXUS") ||
                            text.Equals("MERCEDES") ||
                            text.Equals("SUZUKI") ||
                            text.Equals("TOYOTA")
                        )
                        {
                            brand = text;
                            break;
                        }
                        //else
                        //{
                        //    //Console.Write(text + "|");
                        fullLine = fullLine + text + "|";
                        //}



                    }
                    catch (Exception x)
                    {
                        //Console.WriteLine("Error " + x.Message);
                    }
                }
                if (string.IsNullOrEmpty(fullLine))
                    continue;

                if (fullLine.StartsWith("PRICELIST"))
                    continue;
                if (fullLine.StartsWith("NO"))
                    continue;
                if (fullLine.StartsWith("Booking"))
                    continue;


                //fullLine = status + "|" + brand + "|" + fullLine + "|" ;
                fullLine = fullLine + "|" + status + "|" + brand + "|" ;
                var countSplit = fullLine.Split("|").Length.ToString();
                fullLine = countSplit + "|" + fullLine;
                fullLineInfoList.Add(fullLine);
            }

            //Console.WriteLine(); // Add a blank line between sheets for clarity
        }

      
    }

    //for (int i = 50; i < fullLineInfoList.Count; i++)
    //{
    //    try
    //    {
    //        string fistindex = fullLineInfoList[i].Split("|")[2];
    //        int index = int.Parse(fistindex);
    //    }
    //    catch (Exception)
    //    {
    //        try
    //        {
    //            fullLineInfoList[i - 1] += fullLineInfoList[i];
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //    }
    //}


    //Console.WriteLine("--------------------------------");
    foreach (var str in fullLineInfoList)
    {
        Console.WriteLine(str);
    }
}



//using (PdfReader pdfReader = new PdfReader(pdfPath))
//{
//    IDictionary<int, string> fullLineInfoList = new Dictionary<int, string>();
//    IDictionary<int, string> fullLineStatusList = new Dictionary<int, string>();
//    bool isStartStatus = false;
//    var header = "UNKNOWN";

//    using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
//    {
//        // Loop through all pages in the PDF
//        for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
//        {
//            // Extract text from each page
//            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
//            string pageContent = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);



//            // Display the content of the page line by line
//            foreach (var line in pageContent.Split(new[] { '\n' }, StringSplitOptions.None))
//            {
//                Console.WriteLine(line);
//                try
//                {
//                    if (line.Split(' ').Count() == 1)
//                        header = line.Split(' ')[0];

//                    var filtered = line.Split(' ')[0];

//                    if(filtered.ToLower() == "booking")
//                    {
//                        isStartStatus = true;
//                    }

//                    if(!isStartStatus)
//                    {
//                        try
//                        {

//                            var lineNumber = int.Parse(filtered);
//                            var previousKey = fullLineInfoList.LastOrDefault().Key;

//                            if (lineNumber > 1 && lineNumber - 1 == previousKey)
//                            {
//                                fullLineInfoList.Add(lineNumber, header + " " + line);
//                            }
//                            else if (lineNumber == 1)
//                            {
//                                fullLineInfoList.Add(lineNumber, header + " " + line);
//                            }
//                            else
//                            {
//                                fullLineInfoList[previousKey] = fullLineInfoList[previousKey] + line;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine("ERROR: " +line);
//                            try
//                            {
//                                var previousKey = fullLineInfoList.LastOrDefault().Key;
//                                fullLineInfoList[previousKey] = fullLineInfoList[previousKey] + line;
//                            }
//                            catch (Exception ex2)
//                            {
//                            }

//                        }
//                    }
//                    else
//                    {
//                        try
//                        {
//                            var lineNumber = int.Parse(filtered);
//                            var previousKey = fullLineStatusList.LastOrDefault().Key;

//                            if (lineNumber > 1 && lineNumber - 1 == previousKey)
//                            {
//                                fullLineStatusList.Add(lineNumber, header + " " + line);
//                            }
//                            else if (lineNumber == 1)
//                            {
//                                fullLineStatusList.Add(lineNumber, header + " " + line);
//                            }
//                            else if (fullLineStatusList.Count() == 0)
//                            {
//                                fullLineStatusList.Add(lineNumber, header + " " + line);

//                            }
//                            else
//                            {
//                                fullLineStatusList[previousKey] = fullLineStatusList[previousKey] + line;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine("ERROR: " + line);
//                            try
//                            {
//                                var previousKey = fullLineStatusList.LastOrDefault().Key;
//                                fullLineStatusList[previousKey] = fullLineStatusList[previousKey] + line;
//                            }
//                            catch (Exception ex2)
//                            {
//                            }
//                        }
//                    }

//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("EXCEPTION: " + ex.Message);

//                }
//            }


//        }
//        Console.WriteLine("---------------------------");
//        foreach (var line in fullLineInfoList)
//        {
//            Console.WriteLine(line);
//        }

//        Console.WriteLine("---------------------------");
//        foreach (var line in fullLineStatusList)
//        {
//            Console.WriteLine(line);
//        }
//        Console.ReadLine();
//    }
//}