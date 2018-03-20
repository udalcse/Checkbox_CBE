using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Newtonsoft.Json;

namespace Checkbox.Users
{
    /// <summary>
    /// Custom user field - matrix type
    /// </summary>
    [Serializable]
    public class MatrixField
    {
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        [XmlIgnore]
        public Guid UserID { get; set; }

        /// <summary>
        /// Gets the rows count.
        /// </summary>
        /// <value>
        /// The rows count.
        /// </value>
        public int RowsCount {
            get
            {
                if (this.Cells.Any())
                {
                   return this.RowsHeaders.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Determines whether default grid lines should be rendered on survey item
        /// </summary>
        public string GridLines { get; set; }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        public int ColumnCount
        {
            get
            {
                if (this.Cells.Any())
                {
                        return this.ColumnHeaders.Count;
                }


                return 0;
            }
        }

        /// <summary>
        /// Gets the base column count.
        /// </summary>
        /// <value>
        /// The base column count.
        /// </value>
        public int BaseColumnCount
        {
            get
            {
                if (this.Cells.Any())
                {
                    return this.ColumnHeaders.Count(header => !header.CustomUserCell);
                }


                return 0;
            }
        }

        /// <summary>
        /// Gets the base rows count.
        /// </summary>
        /// <value>
        /// The base rows count.
        /// </value>
        public int BaseRowsCount
        {
            get
            {
                if (this.Cells.Any())
                {
                    return this.RowsHeaders.Count(header => !header.CustomUserCell);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the column headers.
        /// </summary>
        /// <value>
        /// The column headers.
        /// </value>
        public List<Cell> ColumnHeaders
        {
            get
            {
                if (this.Cells.Any())
                    return this.Cells.Where(cell => cell.IsHeader).ToList();

                return new List<Cell>();

            }
        }

        /// <summary>
        /// Gets the rows headers.
        /// </summary>
        /// <value>
        /// The rows headers.
        /// </value>
        public List<Cell> RowsHeaders
        {
            get
            {
                if (this.Cells.Any())
                     return this.Cells.Where(cell => cell.IsRowHeader).ToList();

                return new List<Cell>();
            }
        }

        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public List<Cell> Cells
        {
            get { return this._cells; }
            set
            {
                this._cells = value;
                SetRows();
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<Row> Rows => this._rows;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rows headers fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rows headers fixed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsRowsFixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is headers fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is headers fixed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsColumnsFixed { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has fixed structure.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has fixed structure; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HasFixedStructure
        {

            get { return IsColumnsFixed && IsRowsFixed; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has fixed structure.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has fixed structure; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HasRowHeaders
        {

            get { return this.Cells.Any(item => item.IsRowHeader && !string.IsNullOrEmpty(item.Data)); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has fixed structure.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has fixed structure; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HasHeaders
        {
            get { return this.Cells.Any(item => item.IsHeader && !string.IsNullOrEmpty(item.Data)); }
        }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        private readonly List<Row> _rows;

        /// <summary>
        /// The cells
        /// </summary>
        private List<Cell> _cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixField"/> class.
        /// </summary>
        public MatrixField()
        {
            this.Cells = new List<Cell>();
            this.IsRowsFixed = true;
            this.IsColumnsFixed = true;
            this._rows = new List<Row>();
        }

        /// <summary>
        /// Fills up rows.
        /// </summary>
        public void FillUpRows()
        {
            SetRows();
        }

        /// <summary>
        /// Creates the data cells.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        /// <param name="customCell">if set to <c>true</c> [custom cell].</param>
        public void CreateDataCells(int rows, int columns, List<string> values, bool customCell = true)
        {

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                var rowCells = values.Take(columns).ToList();
                values.RemoveRange(0, columns);

                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    this.Cells.Add(new Cell()
                    {
                        ColumnNumber = columnIndex,
                        RowNumber = rowIndex,
                        IsRowHeader = false,
                        IsHeader = false,
                        Data = rowCells[columnIndex],
                        CustomUserCell = customCell
                    });
                }
            }
        }

        /// <summary>
        /// Creates the custom row data cells.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        public void CreateCustomRowDataCells(int rows, int columns, List<string> values)
        {
            var lastRowNumber =
                this.Cells.Where(item => !item.IsRowHeader && !item.IsHeader).Max(item => item.RowNumber) + 1;

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                var rowCells = values.Take(columns).ToList();
                values.RemoveRange(0, columns);

                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    this.Cells.Add(new Cell()
                    {
                        ColumnNumber = columnIndex,
                        RowNumber = lastRowNumber + rowIndex,
                        IsRowHeader = false,
                        IsHeader = false,
                        Data = rowCells[columnIndex],
                        CustomUserCell = true
                    });
                }
            }
        }

        /// <summary>
        /// Creates the custom column data cells.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        public void CreateCustomColumnDataCells(int rows, int columns, List<string> values)
        {
            var lastColumnNumber =
                this.Cells.Where(item => !item.IsRowHeader && !item.IsHeader).Max(item => item.ColumnNumber) + 1;

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                var rowCells = values.Take(columns).ToList();
                values.RemoveRange(0, columns);

                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    this.Cells.Add(new Cell()
                    {
                        ColumnNumber = lastColumnNumber + columnIndex,
                        RowNumber = rowIndex,
                        IsRowHeader = false,
                        IsHeader = false,
                        Data = rowCells[columnIndex],
                        CustomUserCell = true
                    });
                }
            }
        }

        /// <summary>
        /// Creates the custom row headers.
        /// </summary>
        /// <param name="values">The values.</param>
        public void CreateCustomRowHeaders(List<string> values)
        {
            // saving row headers to the matrix
            for (int i = 0; i < values.Count; i++)
            {
                this.Cells.Add(new Cell()
                {
                    ColumnNumber = i,
                    RowNumber = i + 1,
                    IsRowHeader = true,
                    IsHeader = false,
                    Data = values[i],
                    CustomUserCell = true
                });
            }
        }

        /// <summary>
        /// Builds the matrix cells.
        /// </summary>
        /// <param name="headerValues">The header values.</param>
        /// <param name="rowHeaderValues">The row header values.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        public void BuildMatrixCells(List<MatrixProfileProperty.Cell> headerValues, List<MatrixProfileProperty.Cell> rowHeaderValues, int rows,
          int columns)
        {
            //----- generate headers -----//

            if (headerValues != null && headerValues.Any())
            {
                for (int i = 0; i < headerValues.Count; i++)
                    this.Cells.Add(new Cell() { RowNumber = 0, ColumnNumber = i, Data = headerValues[i].Name, IsHeader = true, ColumnWidth = headerValues[i].ColumnWidth });
            }

            if (rowHeaderValues != null && rowHeaderValues.Any())
            {
                for (int i = 0; i < rowHeaderValues.Count; i++)
                    this.Cells.Add(new Cell()
                    {
                        RowNumber = 0,
                        ColumnNumber = i,
                        Data = rowHeaderValues[i].Name,
                        IsRowHeader = true,
                        ColumnWidth = rowHeaderValues[i].ColumnWidth
                    });
            }

            // if we don't have any valus for header we need to generate headers with empty values with empty data
            if (headerValues != null && !headerValues.Any())
            {
                for (int i = 0; i < columns; i++)
                    this.Cells.Add(new Cell() { RowNumber = i, ColumnNumber = i, Data = string.Empty, IsHeader = true });
            }

            if (rowHeaderValues != null && !rowHeaderValues.Any())
            {
                for (int i = 0; i < rows; i++)
                    this.Cells.Add(new Cell() { RowNumber = i, ColumnNumber = i, Data = string.Empty, IsRowHeader = true });
            }
        }

        /// <summary>
        /// Creates the custom column headers.
        /// </summary>
        /// <param name="values">The values.</param>
        public void CreateCustomColumnHeaders(List<string> values)
        {
            // saving row headers to the matrix
            for (int i = 0; i < values.Count; i++)
            {
                this.Cells.Add(new Cell()
                {
                    ColumnNumber = i + 1,
                    RowNumber = i ,
                    IsRowHeader = false,
                    IsHeader = true,
                    Data = values[i],
                    CustomUserCell = true
                });
            }
        }

        /// <summary>
        /// Removes the data cells.
        /// </summary>
        public void RemoveDataCells()
        {
            if (this.Cells.Any())
            {
                this.Cells.RemoveAll(cell => !cell.IsRowHeader && !cell.IsHeader);
            }
        }

        /// <summary>
        /// Removes the custom structure.
        /// </summary>
        public void RemoveCustomStructure()
        {
            if (this.Cells.Any())
            {
                this.Cells.RemoveAll(cell => (cell.IsRowHeader || cell.IsHeader) && cell.CustomUserCell);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ConvertMatrixToCsvState()
        {
            string result = string.Empty;

            if (this.Cells.Any())
            {
                int rowsCount = this.Cells.Max(item => item.RowNumber);
                int columnCount = this.Cells.Max(item => item.ColumnNumber);

                var rowHeaders = this.Cells.Where(item => item.IsRowHeader).ToList();
                var headers = this.Cells.Where(item => item.IsHeader).ToList();

                if (rowHeaders.Any())
                    rowsCount = rowHeaders.Count - 1;

                if (headers.Any())
                    columnCount = headers.Count - 1;


                var cells = this.Cells.Except(rowHeaders).Except(headers).ToList();


                for (int i = 0; i <= rowsCount; i++)
                {
                    for (int j = 0; j <= columnCount; j++)
                    {
                        string rowTitle = rowHeaders.Any() && !string.IsNullOrEmpty(rowHeaders[i].Data) ? rowHeaders[i].Data : $"RowHeader{i}";
                        string columnTitle = headers.Any() && !string.IsNullOrEmpty(headers[j].Data) ? headers[j].Data : $"ColumnHeader{j}";
                        var cell = cells.FirstOrDefault(item => item.RowNumber == i && item.ColumnNumber == j);

                        result += string.Format("{0}/{1}:{2};", columnTitle, rowTitle,
                            cell != null ? cell.Data : string.Empty);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileProperties"></param>
        /// <param name="userId"></param>
        public static void PopulateMatrixFields(ref List<ProfileProperty> profileProperties, Guid? userId)
        {
            if (!userId.HasValue) return;
            var matrixProperties = profileProperties.Where(t => t.FieldType == CustomFieldType.Matrix);
            foreach (var prop in matrixProperties)
            {
                var matrixField = ProfileManager.GetMatrixField(prop.Name, userId.Value);
                if (matrixField != null)
                {
                    var headers = matrixField.Cells.Where(c => c.IsHeader
                        && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.ColumnNumber).ToList();
                    var rowHeaders = matrixField.Cells.Where(c => c.IsRowHeader
                        && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.RowNumber).ToList();
                    matrixField.FillUpRows();

                    matrixField.Rows.RemoveAll(r => r.Cells.Count == 0);

                    if (matrixField.Rows.Count == 0 || matrixField.Rows.Any(r => r.Cells.Count < matrixField.ColumnCount))
                    {
                        FillMatrixWithEmptyCells(ref matrixField);
                    }
                    if (headers.Count > 0)
                    {
                        AddHeaders(ref matrixField, headers);
                    }
                    if (rowHeaders.Count > 0)
                    {
                        AddRowHeaders(ref matrixField, rowHeaders);
                    }
                    var matrixFieldJson = JsonConvert.SerializeObject(matrixField.Rows);
                    var propIndex = profileProperties.IndexOf(prop);
                    profileProperties[propIndex].Value = matrixFieldJson;
                }
            }
        }

        /// <summary>
        /// Binds the matrix profiel field to item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="renderMode">The render mode.</param>
        /// <param name="responseGuid">The response unique identifier.</param>
        public static void BindMatrixProfielFieldToItem(ItemProxyObject item, RenderMode renderMode, Guid? responseGuid = null)
        {

            //check if item is matrix type and it has binding to profile matrix then  recreate matrix from profile field 
            if (item.TypeName == "Matrix")
            {
                var profileProperties = ProfileManager.GetPropertiesList();

                var bindedField =
                    profileProperties.FirstOrDefault(
                        prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == item.ItemId));

                var userGuid = PropertyBindingManager.GetCurrentUserGuid();

                if (bindedField != null)
                {
                    MatrixField matrix = null;

                    if (!responseGuid.HasValue)
                    {
                        //if anonymous user take matrix json from current session 
                        if (userGuid.Equals(Guid.Empty))
                        {
                            var matrixJson = PropertyBindingManager.GetSessionAnonymousBindedFieldJson(bindedField.Name);
                            if (!string.IsNullOrWhiteSpace(matrixJson))
                                matrix = JsonToMatrix(matrixJson);
                        }

                        if (matrix == null)
                        {
                            matrix = ProfileManager.GetMatrixField(bindedField.Name, userGuid);
                        }
                    }
                    else
                    {
                        var response = PropertyBindingManager.GetResponseFieldState(item.ItemId, responseGuid.Value,
                            HttpContext.Current.User?.Identity.Name);
                        if (!string.IsNullOrWhiteSpace(response))
                            matrix = JsonToMatrix(response);

                    }


                    var additionalData = item.AdditionalData as MatrixAdditionalData;

                    if (additionalData != null && matrix != null)
                    {
                        additionalData.Columns = new MatrixItemColumn[matrix.ColumnCount + 1];

                        //if first element then add primary key to the columns data
                        MatrixItemColumn primaryColumn = new MatrixItemColumn
                        {
                            Alias = "",
                            ColumnType = "PrimaryKey",
                            OptionTexts = new string[0],
                            PrototypeItemId = -1,
                            ValidationErrors = new string[0],
                            ColumnNumber = 1
                        };

                        additionalData.Columns[0] = primaryColumn;

                        //add columns to the matrix from profile fields
                        for (int columnIndex = 1; columnIndex < matrix.ColumnCount + 1; columnIndex++)
                        {

                            additionalData.Columns[columnIndex] = new MatrixItemColumn()
                            {
                                Alias = "",
                                ColumnType = "SingleLineText",
                                HeaderText = matrix.ColumnHeaders[columnIndex - 1].Data,
                                ColumnNumber = columnIndex + 1,
                                OptionTexts = new string[0],
                                ValidationErrors = new string[0],
                                ScaleEndText = "",
                                ScaleMidText = "",
                                ScaleStartText = "",
                                Width = matrix.ColumnHeaders[columnIndex - 1].ColumnWidth
                            };
                        }

                        additionalData.Rows = new MatrixItemRow[matrix.RowsCount];

                        //adding rows to the matrix 
                        for (int i = 0; i < matrix.RowsCount; i++)
                            additionalData.Rows[i] = new MatrixItemRow()
                            {
                                RowNumber = i + 1,
                                RowType = "Normal",
                                Text = matrix.RowsHeaders[i].Data,
                                Alias = ""
                            };

                        //creating additional data 

                        IItemProxyObject[][] itemProxyObjectMain = new ItemProxyObject[matrix.RowsCount][];

                        //genera list of unique ids for answers 
                        var uniqueIds = Utilities.GenerateRandom(matrix.RowsCount*matrix.ColumnCount, 1, 1000);

                        var matrixTypeId = ItemConfigurationManager.GetTypeIdFromName("MatrixMessage");

                        Cell rowHeaderCell = null;

                        if (matrix.Cells != null && matrix.Cells.Any())
                            rowHeaderCell = matrix.Cells.FirstOrDefault(cell => cell.IsRowHeader);


                        for (int rowIndex = 0; rowIndex < matrix.RowsCount; rowIndex++)
                        {
                            var itemProxyObjects = new ItemProxyObject[matrix.ColumnCount + 1];

                            for (int columnIndex = 0; columnIndex < matrix.ColumnCount + 1; columnIndex++)
                            {
                                if (columnIndex == 0)
                                {
                                    itemProxyObjects[columnIndex] = new SurveyResponseItem()
                                    {
                                        Text = matrix.RowsHeaders[rowIndex].Data,
                                        LanguageCode = "en-US",
                                        TypeName = "Message",
                                        ValidationErrors = new string[0],
                                        Visible = true,
                                        Answers = new SurveyResponseItemAnswer[0],
                                        AdditionalData =
                                            matrixTypeId.HasValue
                                                ? AppearanceDataManager.GetDefaultAppearanceDataForType(
                                                    matrixTypeId.Value)
                                                : null,
                                        Width = rowHeaderCell?.ColumnWidth
                                    };
                                }
                                else
                                {

                                    int elementId = int.Parse(rowIndex.ToString() + columnIndex.ToString());
                                    itemProxyObjects[columnIndex] = new SurveyResponseItem()
                                    {
                                        Text = matrix.RowsHeaders[rowIndex].Data,
                                        LanguageCode = "en-US",
                                        ItemId = elementId,
                                        TypeName = "SingleLineText",
                                        ValidationErrors = new string[0],
                                        Visible = true,
                                        Answers = new SurveyResponseItemAnswer[1]
                                    };

                                    var noDataBind = renderMode == RenderMode.SurveyEditor
                                                     || renderMode == RenderMode.SurveyPreview;

                                    var matrixCell =
                                        matrix.Cells.FirstOrDefault(
                                            cell =>
                                                cell.ColumnNumber ==
                                                (columnIndex - 1) && cell.RowNumber == rowIndex && !cell.IsHeader &&
                                                !cell.IsRowHeader);

                                    ((SurveyResponseItem) itemProxyObjects[columnIndex]).Answers[0] =
                                        new SurveyResponseItemAnswer()
                                        {
                                            Alias = "",
                                            AnswerText = matrixCell != null && !noDataBind ? matrixCell.Data : ""
                                        };


                                    SetMetaData((SurveyResponseItem) itemProxyObjects[columnIndex]);
                                }
                            }

                            //remove used ids 
                            uniqueIds.RemoveRange(0, matrix.ColumnCount);

                            itemProxyObjectMain[rowIndex] = itemProxyObjects;
                        }

                        additionalData.ChildItems = itemProxyObjectMain;
                        additionalData.GridLines = matrix.GridLines;

                        additionalData.BindedMatrixInfo = new BindedMatrixInfo
                        {
                            IsRowsStatic = matrix.IsRowsFixed,
                            IsColumnsStatic = matrix.IsColumnsFixed,
                            BaseColumnCount = matrix.BaseColumnCount,
                            BaseRowCount = matrix.BaseRowsCount,
                            RowsCount = matrix.RowsCount,
                            ColumnCount = matrix.ColumnCount,
                            HasRowHeaders = matrix.HasRowHeaders,
                            HasColumnHeaders = matrix.HasHeaders
                        };
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public static string GetBindedMatrixJsonByItemId(int itemId, Guid uniqueIdentifier)
        {
            var profileField = ProfileManager.GetPropertiesList()
                .FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == itemId));

            if (profileField != null)
            {
                var matrixField = ProfileManager.GetMatrixField(profileField.Name, uniqueIdentifier);

                if (matrixField != null)
                {
                    return MatrixToJson(matrixField);
                }
            }

            return string.Empty;
        }

       


        /// <summary>
        /// Matrixes to json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public static MatrixField JsonToMatrix(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException("json");

            return JsonConvert.DeserializeObject<MatrixField>(json);
        }

        /// <summary>
        /// Matrixes to json.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">matrix</exception>
        public static string MatrixToJson(MatrixField matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return JsonConvert.SerializeObject(matrix);
        }

        /// <summary>
        /// Get json on response matrix
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userName"></param>
        /// <param name="responseTemplateGuid"></param>
        /// <returns></returns>
        public static string GetResponseMatrixJsonByItemId(int itemId, string userName, Guid responseTemplateGuid)
        {
            return PropertyBindingManager.GetResponseFieldState(itemId, responseTemplateGuid, userName);
        }

        /// <summary>
        /// Builds html markup from matrix json
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        public static string BuildMatrixHtmlFromJobject(string jsonValue)
        {
            var matrix = JsonConvert.DeserializeObject<List<HtmlMatrix>>(jsonValue);
            var sb = new StringBuilder();
            //start
            sb.Append("<br><br><div style=\"page-break-inside : avoid\"><table cellspacing=\"0\" cellpadding=\"10\" class=\"DefaultGrid border999 shadow999\" border=\"1\">");

            for (int row = 0; row < matrix.Count; row++)
            {
                //row start
                if (matrix[row].Cells[0].IsHeader)
                {
                    sb.Append("<tr style=\"font-weight: bold; background-color: #c5dbe5; height: 34px;\">");
                }
                else
                {
                    if (row % 2 == 0)
                    {
                        sb.Append("<tr style=\"background-color: #e3e3e3;\">");
                    }
                    else
                    {
                        sb.Append("<tr style=\"background-color: #f0f0f0;\">");
                    }
                }

                for (int rowCell = 0; rowCell < matrix[row].Cells.Count; rowCell++)
                {
                    sb.Append("<td>");
                    sb.Append(matrix[row].Cells[rowCell].Data);
                    sb.Append("</td>");
                }

                //row end
                sb.Append("</tr>");
            }

            //end
            sb.Append("</table></div>");

            var matrixMarkup = sb.ToString();

            return matrixMarkup;
        }

        /// <summary>
        /// Returns true if matrix exists in response table
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="responseTemplateGuid"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsMatrixInResponces(int itemId, Guid responseTemplateGuid, string userName)
        {
            return !string.IsNullOrEmpty(PropertyBindingManager.GetResponseFieldState(itemId, responseTemplateGuid, userName));
        }

        private static void SetMetaData(SurveyResponseItem item)
        {
            item.Metadata = new SimpleNameValueCollection();
            item.Metadata.Add("answerRequired", "False");
            item.Metadata.Add("isHtmlFormattedData", "False");
            item.Metadata.Add("answerFormat", "None");
            item.Metadata.Add("maxLength", "");
            item.Metadata.Add("minLength", "");
            item.Metadata.Add("defaultText", "");
            item.Metadata.Add("minNumericValue", "");
            item.Metadata.Add("maxNumericValue", "");
            item.Metadata.Add("MinDateValue", "");
            item.Metadata.Add("MaxDataValue", "");
            item.Metadata.Add("AutocompleteListId", "");
            item.Metadata.Add("AutocompleteRemote", "");

        }

        private static void AddHeaders(ref MatrixField matrixField, List<Cell> headers)
        {
            Row headerRow = new Row();

            if (matrixField.ColumnHeaders.Count > 0
                && matrixField.RowsHeaders.Count > 0
                && matrixField.HasRowHeaders)
            {
                Cell firstCell = new Cell()
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    IsHeader = true,
                    IsRowHeader = true,
                    CustomUserCell = false,
                    Data = "Headers"
                };

                headers.Insert(0, firstCell);
            }

            headerRow.Cells = headers;
            matrixField.Rows.Insert(0, headerRow);
        }

        private static void AddRowHeaders(ref MatrixField matrixField, List<Cell> rowHeaders)
        {
            bool hasHeaders = matrixField.Rows.First().Cells.First().IsHeader;
            int dataRowCount = hasHeaders ? matrixField.Rows.Count - 1 : matrixField.Rows.Count;
            if (rowHeaders.Count != dataRowCount) return;

            int dataRowStartIdx = hasHeaders ? 1 : 0;
            for (int i = dataRowStartIdx; i < matrixField.Rows.Count; i++)
            {
                int rowHeaderIdx = hasHeaders ? i - 1 : i;
                matrixField.Rows[i].Cells.Insert(0, rowHeaders[rowHeaderIdx]);
            }
        }

        /// <summary>
        /// Sets the rows.
        /// </summary>
        private void SetRows()
        {
            if (this.Cells.Any())
            {
                var rowCount = this.Cells.Max(item => item.RowNumber);
                var columnCount = this.Cells.Max(item => item.ColumnNumber);

                for (int i = 0; i <= rowCount; i++)
                {
                    Row row = new Row();
                    for (int j = 0; j <= columnCount; j++)
                    {
                        var cell = this.Cells.FirstOrDefault(item => item.RowNumber == i && item.ColumnNumber == j
                            && !item.IsHeader && !item.IsRowHeader);
                        if (cell != null)
                            row.Cells.Add(new Cell()
                            {
                                RowNumber = i,
                                ColumnNumber = j,
                                Data = cell.Data,
                                IsHeader = cell.IsHeader,
                                IsRowHeader = cell.IsRowHeader
                            });

                    }

                    this._rows.Add(row);
                }

            }
        }

        private static void FillMatrixWithEmptyCells(ref MatrixField matrixField)
        {
            //Create empty cells
            matrixField.Rows.Clear();
            for (int i = 0; i < matrixField.RowsHeaders.Count; i++)
            {
                var row = new Row();
                for (int j = 0; j < matrixField.ColumnHeaders.Count; j++)
                {
                    row.Cells.Add(new Cell()
                    {
                        ColumnNumber = j,
                        RowNumber = i,
                        IsHeader = false,
                        IsRowHeader = false,
                        CustomUserCell = false,
                        Data = string.Empty
                    });
                }
                matrixField.Rows.Add(row);
            }
        }

        /// <summary>
        /// Converts matrix rows to json to render on view as tables 
        /// </summary>
        /// <param name="matrixField">The matrix field.</param>
        /// <returns></returns>
        public static string MatrixRowsToJson(MatrixField matrixField)
        {
            var headers = matrixField.Cells.Where(c => c.IsHeader
                                                       && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.ColumnNumber).ToList();
            var rowHeaders = matrixField.Cells.Where(c => c.IsRowHeader
                                                          && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.RowNumber).ToList();
            matrixField.FillUpRows();

            matrixField.Rows.RemoveAll(r => r.Cells.Count == 0);

            if (matrixField.Rows.Count == 0 || matrixField.Rows.Any(r => r.Cells.Count < matrixField.ColumnCount))
            {
                FillMatrixWithEmptyCells(ref matrixField);
            }
            if (headers.Count > 0)
            {
                AddHeaders(ref matrixField, headers);
            }
            if (rowHeaders.Count > 0)
            {
                AddRowHeaders(ref matrixField, rowHeaders);
            }
            var matrixFieldJson = JsonConvert.SerializeObject(matrixField.Rows);

            return matrixFieldJson;
        }
    }

    /// <summary>
    /// Cell class for matrix types
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Gets or sets the row number.
        /// </summary>
        /// <value>
        /// The row number.
        /// </value>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>
        /// The column number.
        /// </value>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is header.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is header; otherwise, <c>false</c>.
        /// </value>
        public bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is header.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is header; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [custom user cell].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [custom user cell]; otherwise, <c>false</c>.
        /// </value>
        public bool CustomUserCell { get; set; }

        /// <summary>
        /// Gets or sets column width where the cell is displayed
        /// </summary>
        public int? ColumnWidth { get; set; }
    }

    /// <summary>
    /// Row class for matrix implementation
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public List<Cell> Cells { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        public Row()
        {
            this.Cells = new List<Cell>();
        }

    }

    /// <summary>
    /// Used for deserizlization
    /// </summary>
    public class HtmlMatrix
    {
        public List<HtmlCell> Cells { get; set; }
    }

    /// <summary>
    /// Used for deserizlization
    /// </summary>
    public class HtmlCell
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Data { get; set; }
        public bool IsHeader { get; set; }
        public bool IsRowHeader { get; set; }
        public bool CustomUserCell { get; set; }
    }
}
