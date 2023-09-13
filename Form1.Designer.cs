namespace convex_hull
{
	partial class FormConvexHull
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
			this.buttonGenerateRandom = new System.Windows.Forms.Button();
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.generate_benchmarks = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonGenerateRandom
			// 
			this.buttonGenerateRandom.Location = new System.Drawing.Point(785, 12);
			this.buttonGenerateRandom.Name = "buttonGenerateRandom";
			this.buttonGenerateRandom.Size = new System.Drawing.Size(155, 74);
			this.buttonGenerateRandom.TabIndex = 0;
			this.buttonGenerateRandom.Text = "Generate from random points";
			this.buttonGenerateRandom.UseVisualStyleBackColor = true;
			this.buttonGenerateRandom.Click += new System.EventHandler(this.buttonGenerateRandom_Click);
			// 
			// PictureBox
			// 
			this.PictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.PictureBox.Location = new System.Drawing.Point(12, 12);
			this.PictureBox.Name = "PictureBox";
			this.PictureBox.Size = new System.Drawing.Size(736, 535);
			this.PictureBox.TabIndex = 1;
			this.PictureBox.TabStop = false;
			this.PictureBox.Click += new System.EventHandler(this.PictureBox_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(785, 118);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(155, 74);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "Remove point";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// buttonClear
			// 
			this.buttonClear.Location = new System.Drawing.Point(785, 226);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(155, 74);
			this.buttonClear.TabIndex = 4;
			this.buttonClear.Text = "Clear canvas";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// generate_benchmarks
			// 
			this.generate_benchmarks.Location = new System.Drawing.Point(785, 337);
			this.generate_benchmarks.Name = "generate_benchmarks";
			this.generate_benchmarks.Size = new System.Drawing.Size(155, 74);
			this.generate_benchmarks.TabIndex = 5;
			this.generate_benchmarks.Text = "Generate benchmarks";
			this.generate_benchmarks.UseVisualStyleBackColor = true;
			this.generate_benchmarks.Click += new System.EventHandler(this.generate_benchmarks_Click);
			// 
			// FormConvexHull
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(962, 559);
			this.Controls.Add(this.generate_benchmarks);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.PictureBox);
			this.Controls.Add(this.buttonGenerateRandom);
			this.Name = "FormConvexHull";
			this.Text = "Convex Hull";
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonGenerateRandom;
		private System.Windows.Forms.PictureBox PictureBox;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.Button generate_benchmarks;
	}
}

