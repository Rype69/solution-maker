// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingsForm.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.UI.Windows
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Core;

    using Utilities.ComponentModel;

    /// <summary>
    /// A UI component to specify mapping settings
    /// </summary>
    public partial class MappingsForm : Form
    {
        /// <summary>
        /// No-close button constant value.
        /// </summary>
        private const int CP_NOCLOSE_BUTTON = 0x200;

        /// <summary>
        /// The set of <see cref="Mapping"/>s to process.
        /// </summary>
        private readonly IMappingCollection mappingCollection;

        /// <summary>
        /// Initializes static members of the <see cref="MappingsForm"/> class. 
        /// </summary>
        static MappingsForm()
        {
            Instance = new MappingsForm();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MappingsForm"/> class from being created. 
        /// </summary>
        private MappingsForm() 
            : this(IocContainer.Resolver.Resolve<IMappingCollection>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingsForm"/> class.
        /// </summary>
        /// <param name="mappingCollection">
        /// The set of <see cref="Mapping"/>s to process
        /// </param>
        private MappingsForm(IMappingCollection mappingCollection)
        {
            if (mappingCollection == null)
            {
                throw new ArgumentNullException(nameof(mappingCollection));
            }

            this.mappingCollection = mappingCollection;

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the information needed when creating a control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="MappingsForm"/>.
        /// </summary>
        public static MappingsForm Instance { get; private set; }

        /// <summary>
        /// Occurs when the CancelButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Occurs when the value of a cell changes.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="DataGridViewCellEventArgs" /> object that contains the event data. </param>
        private void MappingsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.ValidateData();
        }

        /// <summary>
        /// Occurs after a data-binding operation has finished.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="DataGridViewBindingCompleteEventArgs" /> object that contains the event data. </param>
        private void MappingsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.MappingsDataGridView.ReadOnly = false;
            this.MappingsDataGridView.Columns[0].Width = 100;
            this.MappingsDataGridView.Columns[0].ReadOnly = true;
            this.MappingsDataGridView.Columns[0].Visible = false;
            this.MappingsDataGridView.Columns[1].ReadOnly = true;
            this.MappingsDataGridView.Columns[1].Visible = true;
            this.MappingsDataGridView.Columns[2].ReadOnly = true;
            this.MappingsDataGridView.Columns[2].Visible = true;
            this.MappingsDataGridView.Columns[3].ReadOnly = false;
            this.MappingsDataGridView.Columns[3].Visible = true;
            this.MappingsDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.MappingsDataGridView.Columns[4].ReadOnly = true;
            this.MappingsDataGridView.Columns[4].Visible = true;
            this.MappingsDataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.MappingsDataGridView.Columns[4].Width = 175;

            foreach (var row in this.MappingsDataGridView.Rows.Cast<DataGridViewRow>()
                .Where(row => string.Equals(row.Cells[1].Value?.ToString(), MappingType.Poco.ToString())))
            {
                row.Cells[3].ReadOnly = true;
            }

            // Hide all other columns
            if (this.MappingsDataGridView.ColumnCount > 5)
            {
                for (var i = 5; i < this.MappingsDataGridView.ColumnCount; i++)
                {
                    this.MappingsDataGridView.Columns[i].ReadOnly = true;
                    this.MappingsDataGridView.Columns[i].Visible = false;
                }
            }

            this.ValidateData();
        }

        /// <summary>
        /// Occurs when the content within a cell is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="DataGridViewCellEventArgs" /> object that contains the event data. </param>
        private void MappingsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= this.MappingsDataGridView.Rows.Count)
            {
                return;
            }

            var mappingType = (MappingType)Enum.Parse(typeof(MappingType), this.MappingsDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString());
            if (e.ColumnIndex != 4 || (mappingType != MappingType.DbStoredProcedure && mappingType != MappingType.DbTableValuedFunction))
            {
                return;
            }

            var mapping = this.mappingCollection.Items.First(m => string.Equals(m?.Id.ToString(), this.MappingsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString(), StringComparison.InvariantCultureIgnoreCase));

            this.Enabled = false;

            SqlParametersForm.Instance.ShowDialog(mapping.SqlParameters);

            this.Enabled = true;
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">A <see cref="EventArgs"/> containing event data</param>
        private void MappingsForm_Load(object sender, EventArgs e)
        {
            this.MappingsDataGridView.DataSource = null;
            var formattedGroupList = new SortableBindingList<IMapping>();
            foreach (var mapping in this.mappingCollection.Items)
            {
                formattedGroupList.Add(mapping);
            }

            this.MappingsDataGridView.DataSource = formattedGroupList;
            this.MappingsDataGridView.Invalidate();

            this.MappingsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        /// <summary>
        /// Occurs when the OKButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            this.ValidateData();

            if (!this.OKButton.Enabled)
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Validates an highlights the data in the grid
        /// </summary>
        private void ValidateData()
        {
            var duplicatesPresent = false;
            var emptyCellsPresent = false;
            var allMatchRegularExpression = true;

            foreach (DataGridViewRow row in this.MappingsDataGridView.Rows)
            {
                row.Cells[3].Style.ForeColor = SystemColors.WindowText;
                row.Cells[3].Style.SelectionForeColor = Color.White;

                var value = row.Cells[3].Value?.ToString();

                // Check for empty cells
                if (string.IsNullOrWhiteSpace(value))
                {
                    emptyCellsPresent = true;
                    row.Cells[3].Style.ForeColor = Color.Red;
                    row.Cells[3].Style.SelectionForeColor = Color.Red;
                    continue;
                }

                // Check for match in regular expression
                if (!Regex.IsMatch(value, @"^(?:((?!\d)\w+(?:\.(?!\d)\w+)*)\.)?((?!\d)\w+)$"))
                {
                    allMatchRegularExpression = false;
                    row.Cells[3].Style.ForeColor = Color.Red;
                    row.Cells[3].Style.SelectionForeColor = Color.Red;
                    continue;
                }

                if (string.Equals(row.Cells[1].Value?.ToString(), MappingType.DbStoredProcedure.ToString())
                    || string.Equals(row.Cells[1].Value?.ToString(), MappingType.DbTableValuedFunction.ToString())
                    || string.Equals(row.Cells[1].Value?.ToString(), MappingType.Poco.ToString()))
                {
                    continue;
                }

                // Check for duplicates
                if (this.MappingsDataGridView.Rows.Cast<DataGridViewRow>().Count(r =>
                !string.Equals(r.Cells[1].Value?.ToString(), MappingType.DbStoredProcedure.ToString()) &&
                !string.Equals(r.Cells[1].Value?.ToString(), MappingType.DbTableValuedFunction.ToString()) &&
                !string.Equals(r.Cells[1].Value?.ToString(), MappingType.Poco.ToString()) &&
                r.Cells[3].Value?.ToString() == value) > 1)
                {
                    duplicatesPresent = true;
                    row.Cells[3].Style.ForeColor = Color.Red;
                    row.Cells[3].Style.SelectionForeColor = Color.Red;
                }
            }

            // If there are duplicates, display a message
            this.DuplicatesMessageLabel.Visible = duplicatesPresent;

            // If there are duplicates, display a message
            this.EmptyCellMessageLabel.Visible = emptyCellsPresent;

            // Only allow them to click OK if all duplicates are corrected
            this.OKButton.Enabled = !duplicatesPresent && !emptyCellsPresent && allMatchRegularExpression;
        }
    }
}
