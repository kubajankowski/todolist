using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace v2todo
{
    public partial class Form1 : Form
    {
        private BindingList<ToDoEntry> entries = new BindingList<ToDoEntry>();
        public Form1()
        {
            InitializeComponent();
            entriesSource.DataSource = entries;
            CreateNewItem();
        }
        private void CreateNewItem()
        {
            ToDoEntry newEntry = (ToDoEntry)entriesSource.AddNew();
            newEntry.Title = "Nowe zadanie";
            newEntry.DueDate = DateTime.Now;
            newEntry.Description = "";
            entriesSource.ResetCurrentItem();
        }
        private void MakeListViewItemForNewEntry(int newItemIndex)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems.Add("");
            item.SubItems.Add("");
            item.SubItems.Add("");
            entriesListView.Items.Insert(newItemIndex, item);
        }
        private void UpdateListViewItem(int itemIndex)
        {
            ListViewItem item = entriesListView.Items[itemIndex];
            ToDoEntry entry = entries[itemIndex];
            item.SubItems[0].Text = entry.Title;
            item.SubItems[1].Text = entry.DueDate.ToShortDateString();
            item.SubItems[2].Text = entry.Description;
        }
        private void RemoveListViewItem(int deletedItemIndex)
        {
            entriesListView.Items.RemoveAt(deletedItemIndex);
        }
        private void entriesSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    MakeListViewItemForNewEntry(e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveListViewItem(e.NewIndex);
                    break;
                case ListChangedType.ItemChanged:
                    UpdateListViewItem(e.NewIndex);
                    break;
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            CreateNewItem();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (entriesListView.SelectedIndices.Count != 0)
            {
                int entryIndex = entriesListView.SelectedIndices[0];
                entriesSource.RemoveAt(entryIndex);
            }
        }

        private void entriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (entriesListView.SelectedIndices.Count != 0)
            {
                int entryIndex = entriesListView.SelectedIndices[0];
                entriesSource.Position = entryIndex;
            }
        }
        private void ExportList()
        {
            if (entriesListView.Items.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
                saveFileDialog.Title = "Zapisz listę zadań";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (ListViewItem item in entriesListView.Items)
                        {
                            string title = item.SubItems[0].Text;
                            string dueDate = item.SubItems[1].Text;
                            string description = item.SubItems[2].Text;
                            writer.WriteLine($"Tytuł: {title}, Termin: {dueDate}, Opis: {description}");
                        }
                    }

                    MessageBox.Show("Lista zadań została wyeksportowana.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Lista zadań jest pusta.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ImportList()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            openFileDialog.Title = "Importuj listę zadań";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Wyczyść istniejące wpisy na liście
                entries.Clear();
                entriesSource.ResetBindings(false);
                entriesListView.Items.Clear();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Przetwarzaj każdą linię z pliku
                        // Zakładając, że linia ma format: "Tytuł: <tytuł zadania>, Termin: <data>"
                        string[] parts = line.Split(new[] { "Tytuł: ", ", Termin: " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string title = parts[0];
                            string dueDate = parts[1];

                            // Dodaj wpis do listy
                            ToDoEntry newEntry = new ToDoEntry { Title = title, DueDate = DateTime.Parse(dueDate) };
                            entries.Add(newEntry);

                            // Dodaj element do widoku listy
                            MakeListViewItemForNewEntry(entries.Count - 1);
                            UpdateListViewItem(entries.Count - 1);
                        }
                    }
                }

                MessageBox.Show("Lista zadań została zaimportowana.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportList();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportList();
        }
    }
}
