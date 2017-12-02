// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.UI.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using Core;
    using Core.IO;

    using Utilities.Collections.Generic;

    using Utilities.Windows.Forms.CheckedListBox;

    using MappingType = Core.MappingType;

    /// <summary>
    /// The main form
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Denotes the select all / deselect all db objects function. 
        /// When true, the select all function is denoted.
        /// When false, the deselect all function is denoted.
        /// </summary>
        private bool allDbObjectsSelected = true;

        /// <summary>
        /// Denotes the select all / deselect all poco types function. 
        /// When true, the select all function is denoted.
        /// When false, the deselect all function is denoted.
        /// </summary>
        private bool allPocoTypesSelected = true;

        /// <summary>
        /// A <see cref="BackgroundWorker"/>.
        /// </summary>
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// A list of built-in types to include in the POCO mappings.
        /// </summary>
        private readonly IList<Type> builtInTypes;

        /// <summary>
        /// The UI name prefix for built-in types.
        /// </summary>
        private const string BuiltInTypeNamePrefix = "Built-In";

        /// <summary>
        /// Denotes whether or not the form.load event has occurred
        /// </summary>
        private bool loaded;

        /// <summary>
        /// A set of mappings between DB objects and proposed type names.
        /// This also contains names of POCO classes from the specified assembly.
        /// The set of mappings is effectively the "to do" list for the app.
        /// </summary>
        private readonly IMappingCollection mappings;

        /// <summary>
        /// Produces the mapping code.
        /// </summary>
        private readonly IMappingEngine mappingEngine;

        /// <summary>
        /// The maximum value for the progress bar.
        /// </summary>
        private const int ProgressBarMaximumValue = 100;

        /// <summary>
        /// The minimum value for the progress bar.
        /// </summary>
        private const int ProgressBarMinimumValue = 0;

        /// <summary>
        /// An instance of a settings file
        /// </summary>
        private readonly ISettingsFile settingsFile;

        /// <summary>
        /// A delegate to allow cross-thread UI component access
        /// </summary>
        private delegate void ToDoDelegate();

        /// <summary>
        /// Gets the singleton instance of the <see cref="MainForm"/>.
        /// </summary>
        public static MainForm Instance { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="MainForm"/> class. 
        /// </summary>
        static MainForm()
        {
            Instance = new MainForm();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MainForm"/> class from being created.
        /// </summary>
        private MainForm() : this(
            IocContainer.Resolver.Resolve<IMappingCollection>(),
            IocContainer.Resolver.Resolve<IMappingEngine>(),
            IocContainer.Resolver.Resolve<ISettingsFile>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class
        /// </summary>
        /// <param name="mappings">
        /// A collection of mappings.
        /// </param>
        /// <param name="mappingEngine">
        /// Produces the mapping code.
        /// </param>
        /// <param name="settingsFile">
        /// An instance of a settings file
        /// </param>
        private MainForm(IMappingCollection mappings, IMappingEngine mappingEngine, ISettingsFile settingsFile)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            if (mappingEngine == null)
            {
                throw new ArgumentNullException(nameof(mappingEngine));
            }

            if (settingsFile == null)
            {
                throw new ArgumentNullException(nameof(settingsFile));
            }

            this.mappings = mappings;
            this.mappingEngine = mappingEngine;
            this.settingsFile = settingsFile;

            this.builtInTypes = new List<Type>();

            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when the AddBuiltInTypeButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>

        private void AddBuiltInTypeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Attempt to create a type from the string
                var builtInType = Type.GetType(this.BuiltInTypeTextBox.Text);

                // NULL-check the type
                if (builtInType == null)
                {
                    throw new TypeLoadException($"Unable to load type {this.BuiltInTypeTextBox.Text}");
                }

                // Clear the text in the builtIn types textbox
                this.BuiltInTypeTextBox.Text = string.Empty;

                if (this.builtInTypes.Any(a => a == builtInType))
                {
                    return;
                }

                // Add the type to the list of builtIn types
                this.builtInTypes.UniqueAdd(builtInType);

                // Add an entry to the checked box list
                this.AddTypeToPocoTypesCheckedListBox(builtInType, true, BuiltInTypeNamePrefix);

                // Scroll to the position of the newly added item
                this.POCOTypesCheckedListBox.TopIndex = (this.POCOTypesCheckedListBox.Items.Count - 1);
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        /// <summary>
        /// Adds a type to the POCO types checked list box
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <param name="checked">Denotes whether to set the newly added item as checked.</param>
        /// <param name="prefix">The prefix</param>
        private void AddTypeToPocoTypesCheckedListBox(Type type, bool @checked = false, string prefix = null)
        {
            // NULL-check the parameter
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                return;
            }

            var prefixString = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $" ({prefix})";

            var typeType = type.IsValueType ? type.IsEnum ? "Enum" : "Struct" : type.IsInterface ? "Interface" : "Class";
            this.POCOTypesCheckedListBox.Items.Add($"{typeType}{prefixString} : {type.FullName}", @checked);
        }

        /// <summary>
        /// Occurs when <see cref="BackgroundWorker.RunWorkerAsync()"/> is called.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="DoWorkEventArgs" /> object that contains the event data. </param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Log "started" message
                this.Invoke(new ToDoDelegate(() => this.Log("Started!")));

                // Reset the progress bar to the minimum value
                this.ProgressBar.Value = ProgressBarMinimumValue;

                // Start the mapping!
                this.mappingEngine.Process(this.backgroundWorker);
            }
            catch (AggregateException ae)
            {
                foreach (var ee in ae.InnerExceptions)
                {
                    this.Invoke(new ToDoDelegate(() => this.Log(ee)));
                }
            }
            catch (Exception ee)
            {
                this.Invoke(new ToDoDelegate(() => this.Log(ee)));
            }
        }

        /// <summary>
        /// Occurs when <see cref="BackgroundWorker.ReportProgress(int)" /> is called.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="ProgressChangedEventArgs" /> object that contains the event data. </param>
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update progress bar. The progress percentage is a property of e
            this.Invoke(new ToDoDelegate(() => this.ProgressBar.Value = e.ProgressPercentage));
        }

        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="RunWorkerCompletedEventArgs" /> object that contains the event data. </param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Set the progress bar to maximum
            this.Invoke(new ToDoDelegate(() => this.ProgressBar.Value = ProgressBarMaximumValue));

            // Re-enable the UI of the Main form
            this.Invoke(new ToDoDelegate(() => this.Enabled = true));

            // Indicate success!
            System.Media.SystemSounds.Asterisk.Play();
            this.Invoke(new ToDoDelegate(() => this.Log("Complete!")));
        }

        /// <summary>
        /// Occurs when the BrowseIocMappingsConfigFileButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void BrowseIocMappingsConfigFileButton_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.IocMappingsConfigFilePathTextBox.Text = this.OpenFileDialog.FileName;
            }
        }

        /// <summary>
        /// Occurs when the BrowseOutputDirectoryButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void BrowseOutputDirectoryButton_Click(object sender, EventArgs e)
        {
            if (this.OutputFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.OutputDirectoryTextBox.Text = this.OutputFolderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Occurs when the BrowseAssemblyLocationButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void BrowseAssemblyLocationButton_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.AssemblyLocationTextBox.Text = this.OpenFileDialog.FileName;
            }
        }

        /// <summary>
        /// Occurs when the ClearDBObjectsLinkLabel control is clicked.
        /// Clears the DB objects checkbox list.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="LinkLabelLinkClickedEventArgs" /> object that contains the event data. </param>
        private void ClearDBObjectsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Clear items
            this.DBObjectsCheckedListBox.Items.Clear();

            // Reset the select / deselect all function
            this.allDbObjectsSelected = true;
        }

        /// <summary>
        /// Occurs when the ClearLogLinkLabel control is clicked.
        /// Clears the log.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="LinkLabelLinkClickedEventArgs" /> object that contains the event data. </param>
        private void ClearLogLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Clear the log
            this.ResetLog();
        }

        /// <summary>
        /// Occurs when the ClearPOCOTypesLinkLabel control is clicked.
        /// Clears the POCO types checkbox list.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="LinkLabelLinkClickedEventArgs" /> object that contains the event data. </param>
        private void ClearPOCOTypesLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Clear items
            this.POCOTypesCheckedListBox.Items.Clear();

            // Clear the builtIn types list
            this.builtInTypes.Clear();

            // Reset the select / deselect all function
            this.allPocoTypesSelected = true;
        }

        /// <summary>
        /// Occurs when the CopyToClipboardLinkLabel control is clicked.
        /// Copies the contents of the log to the clipboard
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="LinkLabelLinkClickedEventArgs" /> object that contains the event data. </param>

        private void CopyToClipboardLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(this.LogTextBox.Text);
        }

        /// <summary>
        /// Occurs when the DeleteContentsLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void DeleteContentsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.OutputDirectoryTextBox.Text))
            {
                MessageBox.Show("Please specify an output directory.");
                return;
            }

            if (!Directory.Exists(this.OutputDirectoryTextBox.Text))
            {
                MessageBox.Show($"Cannot find directory \"{this.OutputDirectoryTextBox.Text}\"");
                return;
            }

            var dialogResult = MessageBox.Show("Are you sure you want to delete the contents of the output directory?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            foreach (var directoryPath in Directory.GetDirectories(this.OutputDirectoryTextBox.Text))
            {
                Utilities.IO.Directory.Delete(directoryPath);
            }

            foreach (var filePath in Directory.GetFiles(this.OutputDirectoryTextBox.Text))
            {
                File.Delete(filePath);
            }

            // Indicate success!
            System.Media.SystemSounds.Asterisk.Play();
        }

        /// <summary>
        /// Occurs when the GoButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void GoButton_Click(object sender, EventArgs e)
        {
            // Disable the UI
            this.Enabled = false;

            // Reset UI state
            this.ResetLog();
            this.ProgressBar.Value = ProgressBarMinimumValue;

            // Clear new mappings
            this.mappings.Clear();

            // Verify the output directory exists on disk
            if (!Directory.Exists(this.OutputDirectoryTextBox.Text))
            {
                MessageBox.Show("Please select a valid output directory.", "Directory not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Re-enable the UI
                this.Enabled = true;

                return;
            }

            // Get the set of selected db objects and poco types
            var selectedDbObjectsStrings = this.DBObjectsCheckedListBox.GetNonNullCheckedItemValues().ToList();
            var selectedPocoTypesStrings = this.POCOTypesCheckedListBox.GetNonNullCheckedItemValues().ToList();

            // If no data is present in the checked list boxes, show a message and return
            if (this.DBObjectsCheckedListBox.Items.Count == 0 && this.POCOTypesCheckedListBox.Items.Count == 0)
            {
                MessageBox.Show("Please load some data into the checked-list boxes and make some selections.", "Selections required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Re-enable the UI
                this.Enabled = true;

                return;
            }

            // If nothing is selected, show a message and return
            if (!selectedDbObjectsStrings.Any() && !selectedPocoTypesStrings.Any())
            {
                MessageBox.Show("Please make some selections in the database objects list and/or POCO types list.", "Selections required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Re-enable the UI
                this.Enabled = true;

                return;
            }

            // Create a pluralisation service
            var pluralisationService = PluralizationService.CreateService(Thread.CurrentThread.CurrentCulture);

            // Add the DB object mappings to the mappings list
            foreach (var selectedDbObjectString in selectedDbObjectsStrings)
            {
                var fullTypeName = $"{this.ModelProjectNamespaceTextBox.Text}.{pluralisationService.Singularize(selectedDbObjectString.Substring(selectedDbObjectString.LastIndexOf(".", StringComparison.Ordinal) + 1))}";
                var mapping = this.mappings.Add(fullTypeName, selectedDbObjectString, (MappingType)Enum.Parse(typeof(MappingType), $"Db{selectedDbObjectString.Split(':').First().Replace(" ", string.Empty)}"), this.settingsFile.Data.ModelProjectNamespace);
                if (mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction)
                {
                    var parameters =
                        Utilities.Data.SqlClient.SqlCommand.DeriveParameters(selectedDbObjectString.Split(':').Last(), this.ConnectionStringTextBox.Text).ToList();
                    if (parameters.Count > 0)
                    {
                        foreach (var sqlParameter in parameters)
                        {
                            mapping.SqlParameters.Add(new SqlParameter(sqlParameter));
                        }
                    }
                }
            }

            // Add the POCO types to the mappings list
            foreach (var selectedPocoTypeString in selectedPocoTypesStrings)
            {
                var initialFullTypeName = selectedPocoTypeString.Split(':').Last().Trim();
                var namespaceParts = initialFullTypeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var fullTypeNameBuilder = new StringBuilder();
                for (var namespacePartId = 0; namespacePartId < namespaceParts.Length - 1; namespacePartId++)
                {
                    fullTypeNameBuilder.Append($"{namespaceParts[namespacePartId].Trim()}.");
                }

                fullTypeNameBuilder.Append(pluralisationService.Singularize(namespaceParts.Last()));

                if (selectedPocoTypeString.IndexOf($"({BuiltInTypeNamePrefix})", StringComparison.Ordinal) == -1)
                {
                    this.mappings.Add(
                        fullTypeNameBuilder.ToString(),
                        $"Assembly: {Path.GetFileName(this.AssemblyLocationTextBox.Text)}",
                        MappingType.Poco,
                        this.settingsFile.Data.ModelProjectNamespace);
                }
                else
                {
                    this.mappings.Add(
                        fullTypeNameBuilder.ToString(),
                        null,
                        MappingType.Poco,
                        this.settingsFile.Data.ModelProjectNamespace,
                        isForBuiltInType: true);
                }
            }

            if (MappingsForm.Instance.ShowDialog() == DialogResult.Cancel)
            {
                // Re-enable the UI
                this.Enabled = true;

                return;
            }

            // Produce the code!
            try
            {
                this.backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                this.backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
                this.backgroundWorker.ProgressChanged += this.BackgroundWorker_ProgressChanged;
                this.backgroundWorker.RunWorkerCompleted += this.BackgroundWorker_RunWorkerCompleted;
                this.backgroundWorker.RunWorkerAsync();
            }
            catch (AggregateException ae)
            {
                foreach (var ee in ae.InnerExceptions)
                {
                    this.Log(ee);
                }
            }
            catch (Exception ee)
            {
                this.Log(ee);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="TextBox.Text"/> property value of the <see cref="BuiltInTypeTextBox"/> changes.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void BuiltInTypeTextBox_TextChanged(object sender, EventArgs e)
        {
            this.SaveSettings(sender, e);

            if (string.IsNullOrWhiteSpace(this.BuiltInTypeTextBox.Text) ||
                    this.BuiltInTypeTextBox.Text.Contains(",") ||
                    Type.GetType(this.BuiltInTypeTextBox.Text) == null)
            {
                this.BuiltInTypeTextBox.ForeColor = Color.Red;
                this.AddBuiltInTypeButton.Enabled = false;
            }
            else
            {
                this.BuiltInTypeTextBox.ForeColor = SystemColors.WindowText;
                this.AddBuiltInTypeButton.Enabled = true;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="TextBox.Text"/> property value of the <see cref="IocMappingsConfigFilePathTextBox"/> changes.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void IocMappingsConfigFileTextBox_TextChanged(object sender, EventArgs e)
        {
            this.IocMappingsConfigFilePathTextBox.ForeColor = SystemColors.WindowText;

            if (!File.Exists(this.IocMappingsConfigFilePathTextBox.Text))
            {
                this.IocMappingsConfigFilePathTextBox.ForeColor = Color.Red;
            }

            this.SaveSettings(sender, e);
        }

        /// <summary>
        /// Reads the contents of the settings file, if it exists, and sets the values of the UI components
        /// </summary>
        private void LoadSettings()
        {
            // Read the contents of the settings file
            this.settingsFile.Load();

            // TablesCheckBox
            this.TablesCheckBox.Checked = this.settingsFile.Data.Tables;

            // ViewsCheckBox
            this.ViewsCheckBox.Checked = this.settingsFile.Data.Views;

            // StoredProceduresCheckBox
            this.StoredProceduresCheckBox.Checked = this.settingsFile.Data.StoredProcedures;

            // TableValuedFunctionsCheckBox
            this.TableValuedFunctionsCheckBox.Checked = this.settingsFile.Data.TableValuedFunctions;

            // CSharpRadioButton
            this.CSharpRadioButton.Checked = this.settingsFile.Data.Language == Language.CSharp;

            // VBRadioButton
            this.VisualBasicRadioButton.Checked = this.settingsFile.Data.Language == Language.VisualBasic;

            // UpdateProjectFilesCheckBox
            this.UpdateProjectFilesCheckBox.Checked = this.settingsFile.Data.UpdateProjectFiles;

            // ConnectionStringTextBox
            this.ConnectionStringTextBox.Text = this.settingsFile.Data.ConnectionString;

            // CompanyNameTextBox
            this.CompanyNameTextBox.Text = this.settingsFile.Data.CompanyName;

            // RootNamespaceTextBox
            this.RootNamespaceTextBox.Text = this.settingsFile.Data.RootNamespace;

            // ModelProjectNamespaceTextBox
            this.ModelProjectNamespaceTextBox.Text = this.settingsFile.Data.ModelProjectNamespace;

            // RepositoryProjectNamespaceTextBox
            this.RepositoryProjectNamespaceTextBox.Text = this.settingsFile.Data.RepositoryProjectNamespace;

            // ServiceProjectNamespaceTextBox
            this.ServiceProjectNamespaceTextBox.Text = this.settingsFile.Data.ServiceProjectNamespace;

            // OutputDirectoryTextBox
            this.OutputDirectoryTextBox.Text = this.settingsFile.Data.OutputDirectory;

            // AssemblyLocationTextBox
            this.AssemblyLocationTextBox.Text = this.settingsFile.Data.AssemblyLocation;

            // BuiltInTypeTextBox
            this.BuiltInTypeTextBox.Text = this.settingsFile.Data.BuiltInType;

            // IocMappingsConfigFilePathTextBox
            this.IocMappingsConfigFilePathTextBox.Text = this.settingsFile.Data.IocMappingsConfigFilePath;
        }

        /// <summary>
        /// Appends some text to the log text box
        /// </summary>
        /// <param name="message">The text to append</param>
        /// <param name="isError">Indicates whether the message relates to an error</param>
        private void Log(string message, bool isError = false)
        {
            this.LogTextBox.Text = string.IsNullOrEmpty(this.LogTextBox.Text)
                ? $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} : {message}"
                : $"{this.LogTextBox.Text}\r\n\r\n{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} : {message}";

            if (isError)
            {
                this.LogTextBox.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Appends some text to the log text box
        /// </summary>
        /// <param name="exception">Contains the text to append</param>
        private void Log(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            this.Log($"{exception.Message}\r\n{exception.StackTrace}", true);
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">A <see cref="EventArgs"/> containing event data</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // For some reason the size is capped to 869 x 492
            this.LoadSettings();
            this.loaded = true;
        }

        /// <summary>
        /// Attempts to open a file explorer window
        /// </summary>
        /// <param name="directoryPath">The path to open</param>
        private void OpenFileExplorerWindow(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                MessageBox.Show("Please specify a directory path.");
                return;
            }

            switch (Directory.Exists(directoryPath))
            {
                case true:
                    using (var pr = new Process())
                    {
                        pr.StartInfo.FileName = "EXPLORER";
                        pr.StartInfo.Arguments = $"/n, /e, \"{directoryPath}\"";
                        pr.Start();
                    }

                    break;
                case false:
                    MessageBox.Show($"Cannot find directory \"{directoryPath}\"");
                    break;
            }
        }

        /// <summary>
        /// Occurs when the OpenIocMappingsConfigFileButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void OpenIocMappingsConfigFileButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.IocMappingsConfigFilePathTextBox.Text))
            {
                MessageBox.Show("Please specify an IoC mappings config file path.");
                return;
            }

            switch (File.Exists(this.IocMappingsConfigFilePathTextBox.Text))
            {
                case true:
                    using (var pr = new Process())
                    {
                        try
                        {
                            pr.StartInfo.FileName = "NOTEPAD++";
                            pr.StartInfo.Arguments = this.IocMappingsConfigFilePathTextBox.Text;
                            pr.Start();

                        }
                        catch (Exception)
                        {
                            pr.StartInfo.FileName = this.IocMappingsConfigFilePathTextBox.Text;
                            pr.Start();
                        }
                    }

                    break;
                case false:
                    MessageBox.Show($"Cannot find file \"{this.IocMappingsConfigFilePathTextBox.Text}\"");
                    break;
            }
        }

        /// <summary>
        /// Occurs when the OpenOutputDirectoryButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void OpenOutputDirectoryButton_Click(object sender, EventArgs e)
        {
            this.OpenFileExplorerWindow(this.OutputDirectoryTextBox.Text);
        }

        /// <summary>
        /// Occurs when the OpenAssemblyLocationButton control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void OpenAssemblyLocationButton_Click(object sender, EventArgs e)
        {
            this.OpenFileExplorerWindow(Path.GetDirectoryName(this.AssemblyLocationTextBox.Text));
        }

        /// <summary>
        /// Occurs when the <see cref="TextBox.Text"/> property value of the <see cref="OutputDirectoryTextBox"/> changes.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void OutputDirectoryTextBox_TextChanged(object sender, EventArgs e)
        {
            this.OutputDirectoryTextBox.ForeColor = SystemColors.WindowText;

            if (!Directory.Exists(this.OutputDirectoryTextBox.Text))
            {
                this.OutputDirectoryTextBox.ForeColor = Color.Red;
            }

            this.SaveSettings(sender, e);
        }

        /// <summary>
        /// Occurs when the <see cref="ListBox.SelectedIndex"/> property or the <see cref="ListBox.SelectedIndices"/> collection has changed.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void POCOTypesCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Determine the selected index of the checked list box
            var selectedIndex = this.POCOTypesCheckedListBox.SelectedIndex;

            // If the index is less than one
            if (selectedIndex == -1)
            {
                return;
            }

            // Determine whether the item is checked
            var isChecked = this.POCOTypesCheckedListBox.GetItemChecked(selectedIndex);

            // If it's checked, don't worry about it.
            if (isChecked)
            {
                return;
            }

            // Get the value from of the selected item.
            var value = this.POCOTypesCheckedListBox.Items[this.POCOTypesCheckedListBox.SelectedIndex];

            // If it's not a built-in type, return.
            if (value.ToString().IndexOf($"({BuiltInTypeNamePrefix})", StringComparison.Ordinal) == -1)
            {
                return;
            }

            // Extract the built-in type name
            var typeName = value.ToString().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();

            // Remove from built-in types list
            foreach (var typeToRemove in this.builtInTypes.Where(k => k.FullName == typeName).ToList())
            {
                this.builtInTypes.Remove(typeToRemove);
            }

            // Remove from checkedboxlist
            this.POCOTypesCheckedListBox.Items.Remove(value);
        }

        /// <summary>
        /// Occurs when the RefreshDBListLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void RefreshDBListLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ResetLog();

            try
            {
                this.DBObjectsCheckedListBox.Items.Clear();
                this.allDbObjectsSelected = true;

                if (!this.StoredProceduresCheckBox.Checked && !this.ViewsCheckBox.Checked
                    && !this.TablesCheckBox.Checked && !this.TableValuedFunctionsCheckBox.Checked)
                {
                    return;
                }

                var dbObjects = Utilities.Data.SqlClient.SqlFunctions.GetObjectNames(
                    this.ConnectionStringTextBox.Text,
                    this.StoredProceduresCheckBox.Checked,
                    this.TablesCheckBox.Checked,
                    this.TableValuedFunctionsCheckBox.Checked,
                    this.ViewsCheckBox.Checked);

                var type = string.Empty;
                foreach (var dbObject in dbObjects)
                {
                    switch (Convert.ToString(dbObject.Item3).ToUpper().Trim())
                    {
                        case "IF":
                            type = "Table Valued Function";
                            break;
                        case "P":
                            type = "Stored Procedure";
                            break;
                        case "U":
                            type = "Table";
                            break;
                        case "V":
                            type = "View";
                            break;
                    }

                    this.DBObjectsCheckedListBox.Items.Add($"{type} : {dbObject.Item1}.{dbObject.Item2}", false);
                }
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        /// <summary>
        /// Occurs when the RefreshPOCOListLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void RefreshPOCOListLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ResetLog();

            if (string.IsNullOrWhiteSpace(this.AssemblyLocationTextBox.Text))
            {
                return;
            }

            this.POCOTypesCheckedListBox.Items.Clear();
            this.allPocoTypesSelected = true;

            if (!File.Exists(this.AssemblyLocationTextBox.Text))
            {
                MessageBox.Show($"Cannot find file {this.AssemblyLocationTextBox.Text}");
                return;
            }

            try
            {
                // "LoadFrom" loads the dependencies if they are in the same directory
                var assembly = System.Reflection.Assembly.LoadFrom(this.AssemblyLocationTextBox.Text);
                foreach (var type in assembly.GetTypes())
                {
                    this.AddTypeToPocoTypesCheckedListBox(type);
                }
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }

            try
            {
                foreach (var type in this.builtInTypes)
                {
                    this.AddTypeToPocoTypesCheckedListBox(type, true, BuiltInTypeNamePrefix);
                }
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        /// <summary>
        /// Occurs when the ResetLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void ResetLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ProgressBar.Value = ProgressBarMinimumValue;
        }

        /// <summary>
        /// Resets the state of the log text box
        /// </summary>
        private void ResetLog()
        {
            this.LogTextBox.ForeColor = SystemColors.WindowText;
            this.LogTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Occurs when the SaveLogAsLinkLabel control is clicked.
        /// Opens the save file dialog
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="LinkLabelLinkClickedEventArgs" /> object that contains the event data. </param>

        private void SaveLogAsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Show the "save file" dialog
            var dialogResult = this.SaveFileDialog.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }

            // Write the contents of the log to a specified file path
            File.WriteAllText(this.SaveFileDialog.FileName, this.LogTextBox.Text);
        }

        /// <summary>
        /// Saves the UI settings to file
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        public void SaveSettings(object sender, EventArgs e)
        {
            // Only save values if the form has loaded
            if (!this.loaded)
            {
                return;
            }

            // Code should be exclusive to only one thread
            lock (new object())
            {
                // TablesCheckBox
                this.settingsFile.Data.Tables = this.TablesCheckBox.Checked;

                // ViewsCheckBox
                this.settingsFile.Data.Views = this.ViewsCheckBox.Checked;

                // StoredProceduresCheckBox
                this.settingsFile.Data.StoredProcedures = this.StoredProceduresCheckBox.Checked;

                // StoredProceduresCheckBox
                this.settingsFile.Data.TableValuedFunctions = this.TableValuedFunctionsCheckBox.Checked;

                // CSharpRadioButton / VisualBasicRadioButton
                if (this.CSharpRadioButton.Checked)
                {
                    this.settingsFile.Data.Language = Language.CSharp;
                }
                else if (this.VisualBasicRadioButton.Checked)
                {
                    this.settingsFile.Data.Language = Language.VisualBasic;
                }

                // UpdateProjectFilesCheckBox
                this.settingsFile.Data.UpdateProjectFiles = this.UpdateProjectFilesCheckBox.Checked;

                // ConnectionStringTextBox
                this.settingsFile.Data.ConnectionString = this.ConnectionStringTextBox.Text;

                // CompanyNameTextBox
                this.settingsFile.Data.CompanyName = this.CompanyNameTextBox.Text;

                // RootNamespaceTextBox
                this.settingsFile.Data.RootNamespace = this.RootNamespaceTextBox.Text;

                // ModelProjectNamespaceTextBox
                this.settingsFile.Data.ModelProjectNamespace = this.ModelProjectNamespaceTextBox.Text;

                // RepositoryProjectNamespaceTextBox
                this.settingsFile.Data.RepositoryProjectNamespace = this.RepositoryProjectNamespaceTextBox.Text;

                // ServiceProjectNamespaceTextBox
                this.settingsFile.Data.ServiceProjectNamespace = this.ServiceProjectNamespaceTextBox.Text;

                // OutputDirectoryTextBox
                this.settingsFile.Data.OutputDirectory = this.OutputDirectoryTextBox.Text;

                // AssemblyLocationTextBox
                this.settingsFile.Data.AssemblyLocation = this.AssemblyLocationTextBox.Text;

                // BuiltInTypeTextBox
                this.settingsFile.Data.BuiltInType = this.BuiltInTypeTextBox.Text;

                // IocMappingsConfigFilePath
                this.settingsFile.Data.IocMappingsConfigFilePath = this.IocMappingsConfigFilePathTextBox.Text;

                // Write to file
                this.settingsFile.Save();
            }
        }

        /// <summary>
        /// Occurs when the SelectDeselectAllDBObjectsLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="System.EventArgs" /> object that contains the event data. </param>
        private void SelectDeselectAllDBObjectsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.DBObjectsCheckedListBox.Items.Count == 0)
            {
                return;
            }

            for (var count = 0; count <= (this.DBObjectsCheckedListBox.Items.Count - 1); count++)
            {
                this.DBObjectsCheckedListBox.SetItemChecked(count, this.allDbObjectsSelected);
            }

            this.allDbObjectsSelected = !this.allDbObjectsSelected;
        }

        /// <summary>
        /// Occurs when the SelectDeselectAllPOCOTypesLinkLabel control is clicked.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An <see cref="EventArgs" /> object that contains the event data. </param>
        private void SelectDeselectAllPOCOTypesLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.POCOTypesCheckedListBox.Items.Count == 0)
            {
                return;
            }

            for (var count = 0; count <= (this.POCOTypesCheckedListBox.Items.Count - 1); count++)
            {
                this.POCOTypesCheckedListBox.SetItemChecked(count, this.allPocoTypesSelected);
            }

            this.allPocoTypesSelected = !this.allPocoTypesSelected;
        }
    }
}
