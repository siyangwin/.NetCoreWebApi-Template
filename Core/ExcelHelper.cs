using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Core
{
    /// <summary>
    /// 操作Excel
    /// </summary>
    public class ExcelHelper
    {
        #region 导入
        /// <summary>
        /// 将Excel以文件流转换DataTable
        /// </summary>
        /// <param name="hasTitle">是否有表头</param>
        /// <param name="path">文件路径</param>
        /// <param name="tableindex">文件簿索引</param>
        public static DataTable ExcelToDataTableFormPath(bool hasTitle = true, string path = "", int tableindex = 0)
        {
            //新建Workbook
            Workbook workbook = new Workbook();
            //将当前路径下的文件内容读取到workbook对象里面
            workbook.LoadFromFile(path);
            //得到第一个Sheet页
            Worksheet sheet = workbook.Worksheets[tableindex];
            return SheetToDataTable(hasTitle, sheet);
        }
        /// <summary>
        /// 将Excel以文件流转换DataTable
        /// </summary>
        /// <param name="hasTitle">是否有表头</param>
        /// <param name="stream">文件流</param>
        /// <param name="tableindex">文件簿索引</param>
        public static DataTable ExcelToDataTableFormStream(bool hasTitle = true, Stream stream = null, int tableindex = 0)
        {
            //新建Workbook
            Workbook workbook = new Workbook();
            //将文件流内容读取到workbook对象里面
            workbook.LoadFromStream(stream);
            //得到第一个Sheet页
            Worksheet sheet = workbook.Worksheets[tableindex];

            int iRowCount = sheet.Rows.Length;
            int iColCount = sheet.Columns.Length;
            DataTable dt = new DataTable();
            //生成列头
            for (int i = 0; i < iColCount; i++)
            {
                var name = "column" + i;
                if (hasTitle)
                {
                    var txt = sheet.Range[1, i + 1].Text;
                    if (!string.IsNullOrEmpty(txt)) name = txt;
                }
                while (dt.Columns.Contains(name)) name = name + "_1";//重复行名称会报错。
                dt.Columns.Add(new DataColumn(name, typeof(string)));
            }
            //生成行数据
            int rowIdx = hasTitle ? 2 : 1;
            for (int iRow = rowIdx; iRow <= iRowCount; iRow++)
            {
                DataRow dr = dt.NewRow();
                for (int iCol = 1; iCol <= iColCount; iCol++)
                {
                    dr[iCol - 1] = sheet.Range[iRow, iCol].Text;
                }
                dt.Rows.Add(dr);
            }
            return SheetToDataTable(hasTitle, sheet);
        }

        private static DataTable SheetToDataTable(bool hasTitle, Worksheet sheet)
        {
            int iRowCount = sheet.Rows.Length;
            int iColCount = sheet.Columns.Length;
            var dt = new DataTable();
            //生成列头
            for (var i = 0; i < iColCount; i++)
            {
                var name = "column" + i;
                if (hasTitle)
                {
                    var txt = sheet.Range[1, i + 1].Text;
                    if (!string.IsNullOrEmpty(txt)) name = txt;
                }
                while (dt.Columns.Contains(name)) name = name + "_1";//重复行名称会报错。
                dt.Columns.Add(new DataColumn(name, typeof(string)));
            }
            //生成行数据
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            var rowIdx = hasTitle ? 2 : 1;
            for (var iRow = rowIdx; iRow <= iRowCount; iRow++)
            {
                var dr = dt.NewRow();
                for (var iCol = 1; iCol <= iColCount; iCol++)
                {
                    var text = sheet.Range[iRow, iCol].Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        dr[iCol - 1] = text;
                    }
                    else
                    {
                        dr[iCol - 1] = sheet.Range[iRow, iCol].DisplayedText;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion

        #region 导出
        /// <summary>
        /// 将DaTaTable转成byte[]类型
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="hasTitle">是否有表头</param>
        /// <returns></returns>
        public static void Save(DataTable dt, string path, bool hasTitle = true)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];//第一个工作簿
            if (hasTitle) //表头
            {
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    sheet.Range[1, j + 1].Text = dt.Columns[j].ColumnName;
                    sheet.Range[1, j + 1].ColumnWidth = 22;
                    sheet.Range[1, j + 1].Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;//边框
                    sheet.Range[1, j + 1].Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;//边框
                    sheet.Range[1, j + 1].Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;//边框
                    sheet.Range[1, j + 1].Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;//边框
                }
            }

            //循环表数据
            for (var i = 0; i < dt.Rows.Count; i++)//循环赋值
            {
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var dyg = sheet.Range[i + 2, j + 1];
                    string text = dt.Rows[i][j].ToString();
                    if (text.Length>= 32767)
                    {
                        text = text.Substring(0, 32767);
                    }
                   
                    dyg.Text = text;
                    dyg.ColumnWidth = 22;
                    dyg.Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;//边框
                    dyg.Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                    dyg.Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                    dyg.Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                }
            }
            workbook.SaveToFile(path);
        }
        #endregion

    }
}
