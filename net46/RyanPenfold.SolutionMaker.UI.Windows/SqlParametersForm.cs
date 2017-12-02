// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParametersForm.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.UI.Windows
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using Core;

    /// <summary>
    /// A UI component to specify stored procedure parameter values
    /// </summary>
    public partial class SqlParametersForm : Form
    {
        /// <summary>
        /// No-close button constant value.
        /// </summary>
        private const int CP_NOCLOSE_BUTTON = 0x200;

        /// <summary>
        /// The set of <see cref="SqlParameter"/>s to process.
        /// </summary>
        private ISqlParameterCollection sqlParameters;

        /// <summary>
        /// Initializes static members of the <see cref="SqlParametersForm"/> class. 
        /// </summary>
        static SqlParametersForm()
        {
            Instance = new SqlParametersForm();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlParametersForm"/> class from being created. 
        /// </summary>
        private SqlParametersForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="SqlParametersForm"/>.
        /// </summary>
        public static SqlParametersForm Instance { get; private set; }

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
        /// Shows the form as a modal dialog box.
        /// </summary>
        /// <param name="parameters">
        /// The set of <see cref="SqlParameter"/>s to process
        /// </param>
        /// <returns>
        /// A <see cref="DialogResult"/>.
        /// </returns>
        public DialogResult ShowDialog(ISqlParameterCollection parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.sqlParameters = parameters;

            return base.ShowDialog();
        }

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
        /// Occurs when the OKButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            // this.ValidateData();
            if (!this.OKButton.Enabled)
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Occurs after a data-binding operation has finished.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="DataGridViewBindingCompleteEventArgs" /> object that contains the event data. </param>
        private void SqlParametersDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.SqlParametersDataGridView.ReadOnly = false;
            this.SqlParametersDataGridView.Columns[0].ReadOnly = true;
            this.SqlParametersDataGridView.Columns[1].ReadOnly = true;
            this.SqlParametersDataGridView.Columns[2].ReadOnly = true;
            this.SqlParametersDataGridView.Columns[3].ReadOnly = false;
            this.SqlParametersDataGridView.Columns[4].ReadOnly = false;

            foreach (var row in this.SqlParametersDataGridView.Rows.Cast<DataGridViewRow>())
            {
                row.Cells[3].Value = true;
            }
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">A <see cref="EventArgs"/> containing event data</param>
        private void SqlParametersForm_Load(object sender, EventArgs e)
        {
            this.SqlParametersDataGridView.DataSource = null;

            this.SqlParametersDataGridView.DataSource = this.sqlParameters.Items;

            this.SqlParametersDataGridView.Invalidate();

            this.SqlParametersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
