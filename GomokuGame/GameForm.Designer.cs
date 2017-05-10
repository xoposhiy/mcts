namespace GomokuGame
{
	partial class GameForm
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// GameForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 452);
			this.Name = "GameForm";
			this.Text = "GameForm";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameForm_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameForm_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GameForm_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

