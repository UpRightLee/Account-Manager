using InOutNote.DataBase;
using InOutNote.Models;
using log4net;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ExcelManage
{
    public class ExcelService : IExcelService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataBaseService));
        private static ExcelService instance = new ExcelService();
        public static ExcelService Instance
        {
            get 
            {
                if (instance == null) instance= new ExcelService();
                return instance; 
            }
        }

        public bool SaveInOutDataList(ObservableCollection<InOutModel> inOutDataList)
        {
            string basicPath = Directory.GetCurrentDirectory() + @"\DataList";
            if (!Directory.Exists(basicPath)) Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\DataList");
            string excelFilePath = basicPath + @$"\InOutDataList_{DateTime.Now.ToString("yyyy_MM_dd")}.xlsx";
            try
            {
                Application excelApp = new Application();
                Workbook excelWorkbook = excelApp.Workbooks.Add();
                Worksheet excelWorksheet = excelWorkbook.Worksheets.Item["Sheet1"];
                excelWorksheet.Name = "내역 목록";

                excelWorksheet.Cells.Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                excelWorksheet.Cells[1, 1] = "날짜";
                excelWorksheet.Cells[1, 2] = "입출";
                excelWorksheet.Cells[1, 3] = "금액";
                excelWorksheet.Cells[1, 4] = "분류";
                excelWorksheet.Cells[1, 5] = "은행";
                excelWorksheet.Cells[1, 6] = "카드";
                excelWorksheet.Cells[1, 7] = "용도";
                excelWorksheet.Cells[1, 8] = "세부내역";

                for (int i = 0; i < inOutDataList.Count; i++)
                {
                    excelWorksheet.Cells[2 + i, 1] = inOutDataList[i].UseDate;
                    excelWorksheet.Cells[2 + i, 2] = inOutDataList[i].InOut;
                    excelWorksheet.Cells[2 + i, 3] = inOutDataList[i].Money;
                    excelWorksheet.Cells[2 + i, 3].Style.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    excelWorksheet.Cells[2 + i, 4] = inOutDataList[i].Kind;
                    excelWorksheet.Cells[2 + i, 5] = inOutDataList[i].Bank;
                    excelWorksheet.Cells[2 + i, 6] = inOutDataList[i].Card;
                    excelWorksheet.Cells[2 + i, 7] = inOutDataList[i].Use;
                    excelWorksheet.Cells[2 + i, 8] = inOutDataList[i].Detail;
                }

                excelWorksheet.Columns.AutoFit();
                excelWorkbook.SaveAs(excelFilePath, XlFileFormat.xlWorkbookDefault);
                excelWorkbook.Close();
                excelApp.Quit();

                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool SaveBankCardCodeList(ObservableCollection<Bank> bankList)
        {
            string basicPath = Directory.GetCurrentDirectory() + @"\CodeList";
            if (!Directory.Exists(basicPath)) Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\CodeList");
            string excelFilePath = basicPath + @$"\BankCard_CodeList_{DateTime.Now.ToString("yyyy_MM_dd")}.xlsx";

            try
            {
                Application excelApp = new Application();
                Workbook excelWorkbook = excelApp.Workbooks.Add();
                Worksheet excelWorksheet = excelWorkbook.Worksheets.Item["Sheet1"];
                excelWorksheet.Name = "내역 목록";

                excelWorksheet.Cells.Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                excelWorksheet.Cells[1, 1] = "분류";
                excelWorksheet.Cells[1, 2] = "은행이름";
                excelWorksheet.Cells[1, 3] = "카드이름";

                for (int i = 0; i < bankList.Count; i++)
                {
                    excelWorksheet.Cells[2 + i, 1] = bankList[i].Kind;
                    excelWorksheet.Cells[2 + i, 2] = bankList[i].Description;
                    excelWorksheet.Cells[2 + i, 3] = bankList[i].Card;
                }

                excelWorksheet.Columns.AutoFit();
                excelWorkbook.SaveAs(excelFilePath, XlFileFormat.xlWorkbookDefault);
                excelWorkbook.Close();
                excelApp.Quit();

                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool SaveUseCodeList(ObservableCollection<Use> useList)
        {
            string basicPath = Directory.GetCurrentDirectory() + @"\CodeList";
            if (!Directory.Exists(basicPath)) Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\CodeList");
            string excelFilePath = basicPath + @$"\Use_CodeList_{DateTime.Now.ToString("yyyy_MM_dd")}.xlsx";

            try
            {
                Application excelApp = new Application();
                Workbook excelWorkbook = excelApp.Workbooks.Add();
                Worksheet excelWorksheet = excelWorkbook.Worksheets.Item["Sheet1"];
                excelWorksheet.Name = "내역 목록";

                excelWorksheet.Cells.Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                excelWorksheet.Cells[1, 1] = "번호";
                excelWorksheet.Cells[1, 2] = "용도";

                for (int i = 0; i < useList.Count; i++)
                {
                    excelWorksheet.Cells[2 + i, 1] = useList[i].Name;
                    excelWorksheet.Cells[2 + i, 2] = useList[i].Description;
                }

                excelWorksheet.Columns.AutoFit();
                excelWorkbook.SaveAs(excelFilePath, XlFileFormat.xlWorkbookDefault);
                excelWorkbook.Close();
                excelApp.Quit();

                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
    }
}
