using System;
using System.Windows.Forms;
using CodeLab.Assets.EFUpdateHelper;

namespace CodeLab.Assets.EFUpdateHelperDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookLibraryEntities context = new BookLibraryEntities();
            
            Book bk = new Book() { ID = 1 };
            context.PrepareEntityForUpdate(bk, context.Books);
            bk.Author = "My Updated author name";
            context.SaveChanges(UpdateMode.Allow);
        }
    }
}
