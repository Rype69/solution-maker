﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.UI.Windows
{
    partial class SqlParametersForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlParametersForm));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.SqlParametersDataGridView = new System.Windows.Forms.DataGridView();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SqlParametersDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.AutoScroll = true;
            this.MainPanel.Controls.Add(this.SqlParametersDataGridView);
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1274, 639);
            this.MainPanel.TabIndex = 0;
            // 
            // SqlParametersDataGridView
            // 
            this.SqlParametersDataGridView.AllowUserToAddRows = false;
            this.SqlParametersDataGridView.AllowUserToDeleteRows = false;
            this.SqlParametersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SqlParametersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SqlParametersDataGridView.Location = new System.Drawing.Point(0, 0);
            this.SqlParametersDataGridView.Name = "SqlParametersDataGridView";
            this.SqlParametersDataGridView.ReadOnly = true;
            this.SqlParametersDataGridView.RowTemplate.Height = 28;
            this.SqlParametersDataGridView.Size = new System.Drawing.Size(1274, 639);
            this.SqlParametersDataGridView.TabIndex = 0;
            this.SqlParametersDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.SqlParametersDataGridView_DataBindingComplete);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(640, 645);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 37);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(559, 645);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 37);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // SqlParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1275, 694);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.MainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1297, 750);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1297, 750);
            this.Name = "SqlParametersForm";
            this.Text = "Solution Maker - Specify Stored Procedure Parameter Values";
            this.Load += new System.EventHandler(this.SqlParametersForm_Load);
            this.MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SqlParametersDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.DataGridView SqlParametersDataGridView;
    }
}