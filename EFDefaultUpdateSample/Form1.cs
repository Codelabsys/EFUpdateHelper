using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeLab.Assets.EFDefaultUpdateSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StandardEFUpdateApproach();
        }

        private void StandardEFUpdateApproach()
        {
            GC.Collect();
            BookLibraryEntities ctx = new BookLibraryEntities();
            //First trip to SQL Server:executes select statement if run for the first time
            Book book = ctx.Books.First(bk => bk.ID == 1);

            book.Author = "new Updated Author Name";

            //Second trip to SQL Server:Update the author
            ctx.SaveChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateApproachThatWillCrash();

        }
        private void UpdateApproachThatWillCrash()
        {
            try
            {
                GC.Collect();
                BookLibraryEntities ctx = new BookLibraryEntities();
                Book b = new Book();
                b.ID = 1;
                ctx.Books.Attach(b);

                DbEntityEntry entry = ctx.Entry(b);

                //The commented lines represent the entry state right after attaching
                //entry.State = EntityState.Unchanged;
                //entry.Property("ID").IsModified = false;
                //entry.Property("Author").IsModified = false;
                //entry.Property("Name").IsModified = false;
                //entry.Property("Category").IsModified = false;

                b.Author = "New Author";

                ctx.ChangeTracker.DetectChanges();

                 entry = ctx.Entry(b);
                


                //This is how the entry will look like now
                //entry.State = EntityState.Modified;
                //entry.Property("ID").IsModified = false;
                //entry.Property("Author").IsModified = true;
                //entry.Property("Name").IsModified = false;
                //entry.Property("Category").IsModified = false;

                ctx.SaveChanges();


            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Will crash in EF and wil not even reach SQL server since Name prop is null while it is mandatory in Metadata
            }
        }
    }
}
