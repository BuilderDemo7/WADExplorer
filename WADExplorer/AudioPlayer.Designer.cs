namespace WADExplorer
{
    partial class AudioPlayer
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.StopButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.AudioDescription = new System.Windows.Forms.Label();
            this.ExportStreamButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StopButton
            // 
            this.StopButton.BackColor = System.Drawing.Color.Transparent;
            this.StopButton.Image = global::WADExplorer.Properties.Resources.stopbtn_small;
            this.StopButton.Location = new System.Drawing.Point(77, 3);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(68, 68);
            this.StopButton.TabIndex = 1;
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.Color.Transparent;
            this.PlayButton.Image = global::WADExplorer.Properties.Resources.playbtn_small;
            this.PlayButton.Location = new System.Drawing.Point(3, 3);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(68, 68);
            this.PlayButton.TabIndex = 0;
            this.PlayButton.UseVisualStyleBackColor = false;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // AudioDescription
            // 
            this.AudioDescription.AutoSize = true;
            this.AudioDescription.Location = new System.Drawing.Point(151, 3);
            this.AudioDescription.Name = "AudioDescription";
            this.AudioDescription.Size = new System.Drawing.Size(33, 13);
            this.AudioDescription.TabIndex = 2;
            this.AudioDescription.Text = "None";
            // 
            // ExportStreamButton
            // 
            this.ExportStreamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ExportStreamButton.Location = new System.Drawing.Point(151, 44);
            this.ExportStreamButton.Name = "ExportStreamButton";
            this.ExportStreamButton.Size = new System.Drawing.Size(75, 23);
            this.ExportStreamButton.TabIndex = 3;
            this.ExportStreamButton.Text = "Export";
            this.ExportStreamButton.UseVisualStyleBackColor = true;
            this.ExportStreamButton.Click += new System.EventHandler(this.ExportStreamButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(151, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Audio File";
            // 
            // AudioPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExportStreamButton);
            this.Controls.Add(this.AudioDescription);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.PlayButton);
            this.Name = "AudioPlayer";
            this.Size = new System.Drawing.Size(353, 70);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button PlayButton;
        public System.Windows.Forms.Button StopButton;
        public System.Windows.Forms.Label AudioDescription;
        private System.Windows.Forms.Button ExportStreamButton;
        public System.Windows.Forms.Label label1;
    }
}
