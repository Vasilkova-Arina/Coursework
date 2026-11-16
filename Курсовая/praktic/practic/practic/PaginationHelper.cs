using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practic
{
    public class PaginationHelper
    {
        private DataGridView grid;
        private Form parentForm;
        private int pageSize;
        private int currentPage = 1;

        public PaginationHelper(DataGridView dataGridView, Form form, int rowsPerPage = 10)
        {
            grid = dataGridView;
            parentForm = form;
            pageSize = rowsPerPage;
        }

        public void Draw(int x, int y)
        {
            for (int j = parentForm.Controls.Count - 1; j >= 0; --j)
            {
                if (parentForm.Controls[j].Name.StartsWith("page"))
                    parentForm.Controls.RemoveAt(j);
            }

            int rowsCount = grid.Rows.Count;
            int pagesCount = (int)Math.Ceiling((double)rowsCount / pageSize);

            if (pagesCount <= 1) return;

            LinkLabel[] ll = new LinkLabel[pagesCount];
            int step = 35; 

            for (int i = 0; i < pagesCount; ++i)
            {
                ll[i] = new LinkLabel();
                ll[i].Text = Convert.ToString(i + 1);
                ll[i].Name = "page" + i;
                ll[i].AutoSize = true;
                ll[i].Location = new Point(x, y);
                ll[i].Click += new EventHandler(LinkLabel_Click);
                ll[i].LinkColor = Color.Black;
                ll[i].Font = new Font("Segoe UI", 12f, FontStyle.Bold);

                parentForm.Controls.Add(ll[i]);
                ll[i].BringToFront();

                x += step;
            }

            LinkLabel_Click(ll[0], EventArgs.Empty);
        }
        private void LinkLabel_Click(object sender, EventArgs e)
        {
            LinkLabel l = sender as LinkLabel;
            if (l == null) return;

            foreach (Control ctrl in parentForm.Controls)
                if (ctrl is LinkLabel && ctrl.Name.StartsWith("page"))
                    ((LinkLabel)ctrl).LinkBehavior = LinkBehavior.AlwaysUnderline;

            l.LinkBehavior = LinkBehavior.NeverUnderline;

            int numPage = Convert.ToInt32(l.Text) - 1;
            currentPage = numPage + 1;

            int countRows = grid.Rows.Count;
            int start = numPage * pageSize;
            int stop = start + pageSize;

            if (grid.DataSource != null)
            {
                CurrencyManager cm = (CurrencyManager)parentForm.BindingContext[grid.DataSource];
                cm.SuspendBinding();

                for (int j = 0; j < countRows; ++j)
                {
                    grid.Rows[j].Visible = (j >= start && j < stop);
                }

                cm.ResumeBinding();
            }
        }
    }
}
