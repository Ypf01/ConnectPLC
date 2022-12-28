using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Pframe.Tools
{
	public class DataGridViewStyle
	{
		public void DgvStyle1(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
			dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Blue;
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 128, 255);
			dgv.ColumnHeadersDefaultCellStyle.Font = new Font("宋体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 192);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 128, 255);
			dgv.RowsDefaultCellStyle.SelectionForeColor = Color.Blue;
			dgv.GridColor = Color.FromArgb(0, 0, 192);
			dgv.ColumnHeadersHeight = 28;
		}
        
		public void DgvStyle2(DataGridView dgv)
		{
			dgv.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken;
			dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
			dgv.ColumnHeadersHeight = 28;
			dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken;
			dgv.DefaultCellStyle.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dgv.RowTemplate.DividerHeight = 1;
			dgv.EnableHeadersVisualStyles = false;
		}
        
		public void DgvStyle3(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(26, 29, 48);
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(115, 168, 223);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(42, 42, 71);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(115, 168, 223);
			dgv.GridColor = Color.FromArgb(214, 214, 214);
			dgv.ColumnHeadersHeight = 28;
		}
        
		public void DgvStyle4(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(26, 29, 48);
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 29, 48);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(42, 42, 71);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 42, 71);
		}
        
		public void DgvStyle5(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(68, 88, 124);
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(68, 88, 124);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(44, 61, 90);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 61, 90);
			dgv.GridColor = Color.FromArgb(214, 214, 214);
			dgv.ColumnHeadersHeight = 28;
		}
       
		public void DgvStyle6(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(26, 29, 48);
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 29, 48);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(42, 42, 71);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 42, 71);
		}
        
		public void DgvStyle7(DataGridView dgv)
		{
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(68, 88, 124);
			dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(68, 88, 124);
			dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(44, 61, 90);
			dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 61, 90);
			dgv.GridColor = Color.FromArgb(214, 214, 214);
			dgv.ColumnHeadersHeight = 28;
		}
        
		public void DgvRowPostPaint(DataGridView dgv, DataGridViewRowPostPaintEventArgs e)
		{
			try
			{
				SolidBrush brush = new SolidBrush(dgv.RowHeadersDefaultCellStyle.ForeColor);
				string s = (e.RowIndex + 1).ToString();
				e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				StringFormat stringFormat = new StringFormat();
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth, dgv.RowTemplate.Height), stringFormat);
			}
			catch (Exception ex)
			{
				MessageBox.Show("添加行号时发生错误，错误信息：" + ex.Message, "操作失败");
			}
		}
        
	}
}
