using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsAppEditTable2.DAO;
using WindowsFormsAppEditTable2.Models;
using WindowsFormsAppEditTable2.Utils;

namespace WindowsFormsAppEditTable2
{
    public partial class Form1 : Form
    {
        List<Category> categories = new List<Category>();
        int pageSize = 18;
        bool isSearch = true;
        bool isCreate = false;
        DataTable dataTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            categories = Util.ConvertDataTable<Category>(CategoryDAO.Instance.GetCategories()).ToList();
            comboBoxLoai.DataSource = categories;
            comboBoxLoai.DisplayMember = "nameDisplay";
            comboBoxLoai.ValueMember = "idLoai";
            GetData();
            //bindingNavigatorAddNewItem.Click += bindingNavigatorAddNewItem_Click;
            //toolStripButtonDelete.Click += ToolStripButtonDelete_Click;
            //bindingNavigatorDeleteItem.Click += BindingNavigatorDeleteItem_Click;
            //bindingSource1.ListChanged += BindingSource_ListChanged;
        }

        private void GetData()
        {
            if (isSearch)
            {
                int currentPage = int.Parse(comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "1");
                dataTable = ProductDAO.Instance.GetProducts(currentPage, pageSize);
                bindingSource1.DataSource = dataTable;
                dataGridView1.DataSource = bindingSource1;

                int totalRecord = ProductDAO.Instance.GetCountProduct();
                labelTotal.Text = totalRecord.ToString();
                int totalPage = totalRecord / pageSize + (totalRecord % pageSize > 0 ? 1 : 0);
                SetValueCombobox(totalPage, currentPage);

                AddBNBinding();

                //int idLoai = int.Parse(dataGridView1.Rows[0].Cells[6].Value.ToString());
                //foreach (Category item in categories)
                //{
                //    if (item.idLoai == idLoai)
                //    {
                //        comboBoxLoai.SelectedItem = item;
                //        break;
                //    }
                //}
                //if (dataGridView1.Rows[0].Cells[5].Value == null)
                //{
                //    dateTimePicker1.Checked = false;
                //}
            }
        }

        private void AddBNBinding()
        {
            textBoxID.DataBindings.Clear();
            textBoxID.DataBindings.Add(new Binding("Text", dataGridView1.DataSource, "idSp", true, DataSourceUpdateMode.OnPropertyChanged));
            textBoxTen.DataBindings.Clear();
            textBoxTen.DataBindings.Add(new Binding("Text", dataGridView1.DataSource, "ten", true, DataSourceUpdateMode.OnValidation));
            dateTimePicker1.DataBindings.Clear();
            dateTimePicker1.DataBindings.Add(new Binding("Value", dataGridView1.DataSource, "ngayNhap", true, DataSourceUpdateMode.OnValidation));

            bindingSource1.CurrentChanged += (sender, e) =>
            {
                object value = ((DataRowView)bindingSource1.Current)["ngayNhap"];
                if (value != DBNull.Value && value != null)
                {
                    dateTimePicker1.Checked = true;
                }
                else
                {
                    dateTimePicker1.Checked = false;
                }

                object valueLoai = ((DataRowView)bindingSource1.Current)["idLoai"];
                if (valueLoai != DBNull.Value && valueLoai != null)
                {
                    comboBoxLoai.SelectedValue = int.Parse(valueLoai.ToString());
                }
            };

            textBoxGia.DataBindings.Clear();
            textBoxGia.DataBindings.Add(new Binding("Text", dataGridView1.DataSource, "gia", true, DataSourceUpdateMode.OnValidation));
            numericUpDown1.DataBindings.Clear();
            numericUpDown1.DataBindings.Add(new Binding("Text", dataGridView1.DataSource, "soLuong", true, DataSourceUpdateMode.OnValidation));
            comboBoxLoai.DataBindings.Clear();
            comboBoxLoai.DataBindings.Add(new Binding("SelectedValue", dataGridView1.DataSource, "idLoai", true, DataSourceUpdateMode.OnValidation));

            //dateTimePicker1.DataBindings.Add(new Binding("Checked", dataGridView1.DataSource, "ngayNhap", true, DataSourceUpdateMode.OnValidation));
            //dateTimePicker1.DataBindings["Checked"].BindingComplete += (sender, e) =>
            //{
            //    if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate && bindingSource1.Current != null)
            //    {
            //        object value = ((DataRowView)bindingSource1.Current)["ngayNhap"];
            //        if (value != DBNull.Value && value != null)
            //        {
            //            dateTimePicker1.Checked = true;
            //        }
            //        else
            //        {
            //            dateTimePicker1.Checked = false;
            //        }
            //    }
            //};
        }
        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            bindingSource1.CancelEdit();
            buttonAddItem.Enabled = true;
            buttonEdit.Enabled = true;
            buttonDeleteItem.Enabled = true;
            buttonUpdate.Enabled = false;
            buttonCancel.Enabled = false;
            dataGridView1.Enabled = true;

            textBoxID.Enabled = false;
            foreach (Control ctrl in panel2.Controls)
            {
                ctrl.Enabled = false;
            }
            if(isCreate)
            {
                bindingSource1.RemoveCurrent();
                bindingSource1.MoveFirst();
                isCreate = false;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            DataRowView dv = (DataRowView)bindingSource1.Current;
            double gia = 0;
            if (!double.TryParse(dv.Row["gia"].ToString(), out gia))
            {
                MessageBox.Show("Giá sản phẩm phải là số", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (gia < 0)
            {
                MessageBox.Show("Giá sản phẩm phải lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isCreate)
            {
                int id = 0;
                if (int.TryParse(dv.Row["idSp"].ToString(), out id))
                {
                    Product product = ProductDAO.Instance.GetByID(id);
                    if (product != null)
                    {
                        MessageBox.Show("ID sản phẩm đã tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    product = new Product();
                    //Category category = comboBoxLoai.SelectedItem as Category;
                    product.idLoai = int.Parse(dv.Row["idLoai"].ToString());
                    product.idSp = id;
                    product.ten = dv.Row["ten"].ToString();
                    product.gia = double.Parse(dv.Row["gia"].ToString());
                    product.soLuong = int.Parse(dv.Row["soLuong"].ToString());
                    //if (dateTimePicker1.Checked)
                    try
                    {
                        product.ngayNhap = (DateTime)dv.Row["ngayNhap"];
                    }
                    catch (Exception)
                    {

                    }
                        
                    ProductDAO.Instance.Insert(product);
                    isCreate = false;
                }
                else
                {
                    MessageBox.Show("ID sản phẩm phải là số", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                Product product = new Product();
                //Category category = comboBoxLoai.SelectedItem as Category;
                product.idLoai = int.Parse(dv.Row["idLoai"].ToString());
                product.idSp = int.Parse(dv.Row["idSp"].ToString());
                product.ten = dv.Row["ten"].ToString();
                product.gia = double.Parse(dv.Row["gia"].ToString());
                product.soLuong = int.Parse(dv.Row["soLuong"].ToString());
                try
                {
                    product.ngayNhap = (DateTime)dv.Row["ngayNhap"];
                }
                catch (Exception)
                {

                }
                ProductDAO.Instance.Update(product);
            }
            buttonAddItem.Enabled = true;
            buttonEdit.Enabled = true;

            textBoxID.Enabled = false;
            foreach (Control ctrl in panel2.Controls)
            {
                ctrl.Enabled = false;
            }
            buttonUpdate.Enabled = false;
            buttonCancel.Enabled = false;
            dataGridView1.Enabled = true;
        }

        
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            textBoxID.Enabled = false;
            dataGridView1.Enabled = false;
            foreach (Control ctrl in panel2.Controls)
            {
                ctrl.Enabled = true;
            }
            buttonUpdate.Enabled = true;
            buttonCancel.Enabled = true;
            buttonAddItem.Enabled = false;
            buttonEdit.Enabled = false;
            buttonDeleteItem.Enabled = false;   
        }
        
        private void buttonAddItem_Click(object sender, EventArgs e)
        {
            bindingSource1.AddNew();
            bindingSource1.MoveLast();
            dataGridView1.Enabled = false;
            buttonEdit.Enabled = false;
            buttonUpdate.Enabled = true;
            buttonCancel.Enabled = true;
            buttonDeleteItem.Enabled=false;
            //textBoxID.Enabled = true;
            //textBoxID.Text = new Random().Next(100, 99999).ToString();
            foreach (Control ctrl in panel2.Controls)
            {
                ctrl.Enabled = true;
            }
            isCreate = true;
        }

        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            DataRowView dv = (DataRowView)bindingSource1.Current;
            if (dv != null)
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa " + dv.Row["ten"].ToString() + " ?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    ProductDAO.Instance.Delete(Convert.ToInt32(dv.Row["idSp"].ToString()));
                    bindingSource1.Remove(dv);
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            GetData();
            bindingSource1.MoveFirst();
        }

        private void bindingSource1_AddingNew(object sender, AddingNewEventArgs e)
        {
            // Tạo một dòng mới trong DataTable
            DataRow newRow = dataTable.NewRow();

            // Thiết lập giá trị mặc định cho các cột của dòng mới
            newRow["idSp"] = new Random().Next(100, 99999);
            newRow["ten"] = "";
            newRow["gia"] = 0; // Giá trị mặc định cho Age, bạn có thể thay đổi tùy ý
            newRow["soLuong"] = 0;
            newRow["tenLoaiSp"] = "";
            newRow["idLoai"] = categories[0].idLoai;
            newRow["tenLoaiSp"] = categories[0].tenLoaiSp;

            // Thêm dòng mới vào DataTable
            dataTable.Rows.Add(newRow);

            // Gán dòng mới vào biến e.NewObject để thêm vào BindingSource
            e.NewObject = newRow;

        }

        #region Các hàm liên quan
        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Kiểm tra xem sự kiện ListChanged có phải là xóa dòng không
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                DataRowView dv = (DataRowView)bindingSource1.Current;
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Nếu người dùng chọn "No" hoặc đóng hộp thoại, hủy việc xóa
                if (result == DialogResult.No)
                {
                    bindingSource1.CancelEdit(); // Hủy thao tác xóa
                }
            }
        }

        private void BindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            bindingSource1.CancelEdit();
            DataRowView dv = (DataRowView)bindingSource1.Current;
            if (dv == null)
                return;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa " + dv.Row["ten"].ToString() + " ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ProductDAO.Instance.Delete(Convert.ToInt32(dv.Row["idSp"].ToString()));
                GetData();
            }
            else
                return;
        }

        private void SetValueCombobox(int totalPage, int currentPage)
        {
            comboBox1.Items.Clear();
            for (int i = 1; i <= totalPage; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
            isSearch = false;
            comboBox1.SelectedIndex = currentPage - 1;
            isSearch = true;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            FormProduct f = new FormProduct(row.Cells[0].Value.ToString());
            f.ShowDialog();
            GetData();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Guid id = Guid.NewGuid();
            FormProduct f = new FormProduct(new Random().Next(100, 99999).ToString());
            f.ShowDialog();
            GetData();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DataRowView dv = (DataRowView)bindingSource1.Current;
            if (dv == null)
                return;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa " + dv.Row["ten"].ToString() + " ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ProductDAO.Instance.Delete(Convert.ToInt32(dv.Row["idSp"].ToString()));
                GetData();
            }
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
                comboBox1.SelectedIndex--;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < comboBox1.Items.Count - 1)
                comboBox1.SelectedIndex++;
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < comboBox1.Items.Count - 1)
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            //DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            //int idLoai = int.Parse(row.Cells[6].Value.ToString());
            //foreach (Category item in categories)
            //{
            //    if (item.idLoai == idLoai)
            //    {
            //        comboBoxLoai.SelectedItem = item;
            //        break;
            //    }    
            //}
            //if (row.Cells[5].Value.ToString().Length == 0)
            //{
            //    dateTimePicker1.Checked = false;
            //}
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //DataRowView dv = (DataRowView)bindingSource1.Current;
            //if (dv == null)
            //    return;
            //int idLoai = 0;
            //int.TryParse(dv.Row["idLoai"].ToString(), out idLoai);
            //foreach (Category item in categories)
            //{
            //    if (item.idLoai == idLoai)
            //    {
            //        comboBoxLoai.SelectedItem = item;
            //        break;
            //    }
            //}
            //if (dv.Row["ngayNhap"].ToString().Length == 0)
            //{
            //    dateTimePicker1.Checked = false;
            //}
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            buttonEdit.Enabled = false;
            buttonUpdate.Enabled = true;
            buttonCancel.Enabled = true;

            textBoxID.Enabled = true;
            textBoxID.Text = new Random().Next(100, 99999).ToString();
            textBoxTen.Enabled = true;
            textBoxGia.Enabled = true;
            numericUpDown1.Enabled = true;
            comboBoxLoai.Enabled = true;
            dateTimePicker1.Enabled = true;
            buttonUpdate.Enabled = true;
            buttonCancel.Enabled = true;
            isCreate = true;
        }

        private void ToolStripButtonDelete_Click(object sender, EventArgs e)
        {
            DataRowView dv = (DataRowView)bindingSource1.Current;
            if (dv != null)
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa " + dv.Row["ten"].ToString() + " ?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //bindingSource1.Remove(dv);
                    ProductDAO.Instance.Delete(Convert.ToInt32(dv.Row["idSp"].ToString()));
                    GetData();
                }
            }
        }
        #endregion
    }
}
