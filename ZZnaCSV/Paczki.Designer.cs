
using System.Windows.Forms;

namespace Przelewy
{
    partial class Paczki
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Paczki));
            this.pacz = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button = new System.Windows.Forms.Button();
            this.barcode = new System.Windows.Forms.TextBox();
            this.checkPlat = new System.Windows.Forms.CheckBox();
            this.checkKaucja = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pacz
            // 
            this.pacz.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pacz.ItemHeight = 16;
            this.pacz.Location = new System.Drawing.Point(25, 48);
            this.pacz.Name = "pacz";
            this.pacz.Size = new System.Drawing.Size(408, 180);
            this.pacz.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(22, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Proszę wybrać paczkę: ";
            // 
            // label2
            // 
            this.label2.Enabled = false;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(22, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Proszę wprowadzić kod kreskowy: ";
            // 
            // label3
            // 
            this.label3.Enabled = false;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(22, 322);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(411, 140);
            this.label3.TabIndex = 0;
            // 
            // button
            // 
            this.button.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button.Location = new System.Drawing.Point(84, 495);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(282, 41);
            this.button.TabIndex = 4;
            this.button.Text = "Dodaj";
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // barcode
            // 
            this.barcode.Enabled = false;
            this.barcode.Location = new System.Drawing.Point(25, 281);
            this.barcode.Name = "barcode";
            this.barcode.Size = new System.Drawing.Size(408, 20);
            this.barcode.TabIndex = 7;
            this.barcode.TextChanged += new System.EventHandler(this.barcode_TextChanged);
            // 
            // checkPlat
            // 
            this.checkPlat.Checked = true;
            this.checkPlat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkPlat.Enabled = false;
            this.checkPlat.Location = new System.Drawing.Point(100, 465);
            this.checkPlat.Name = "checkPlat";
            this.checkPlat.Size = new System.Drawing.Size(104, 24);
            this.checkPlat.TabIndex = 8;
            this.checkPlat.Text = "Płatności";
            // 
            // checkKaucja
            // 
            this.checkKaucja.Enabled = false;
            this.checkKaucja.Location = new System.Drawing.Point(262, 465);
            this.checkKaucja.Name = "checkKaucja";
            this.checkKaucja.Size = new System.Drawing.Size(104, 24);
            this.checkKaucja.TabIndex = 8;
            this.checkKaucja.Text = "Kaucje";
            // 
            // Paczki
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 548);
            this.Controls.Add(this.pacz);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button);
            this.Controls.Add(this.barcode);
            this.Controls.Add(this.checkPlat);
            this.Controls.Add(this.checkKaucja);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Paczki";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dodawanie dokumentów do paczki";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Paczki_Close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.ListBox pacz;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button;
        private TextBox barcode;
        private CheckBox checkPlat;
        private CheckBox checkKaucja;

    }
}