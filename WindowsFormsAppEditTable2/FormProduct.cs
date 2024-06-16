using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsAppEditTable2.DAO;
using WindowsFormsAppEditTable2.Models;
using WindowsFormsAppEditTable2.Utils;

namespace WindowsFormsAppEditTable2
{
    public partial class FormProduct : Form
    {
        public FormProduct(string strId)
        {
            InitializeComponent();

            int id = int.Parse(strId);
            Product product = ProductDAO.Instance.GetByID(id);
            List<Category> categories = Util.ConvertDataTable<Category>(CategoryDAO.Instance.GetCategories());
            comboBox1.DataSource = categories;
            comboBox1.DisplayMember = "tenLoaiSp";
            textBoxID.Text = strId;
            if (product != null)
            {
                textBoxID.ReadOnly = true;
                textBoxTen.Text = product.ten;
                textBoxGia.Text = product.gia.ToString();
                textBoxSL.Text = product.soLuong.ToString();
                foreach (Category item in categories)
                {
                    if (item.idLoai == product.idLoai)
                        comboBox1.SelectedItem = item;
                }
                if (product.ngayNhap.HasValue)
                    dateTimePicker1.Value = product.ngayNhap.Value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool res = false;
            double gia = 0;
            int sl = 0;
            if(!double.TryParse(textBoxGia.Text, out gia))
            {
                MessageBox.Show("Giá sản phẩm phải là số", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if(gia < 0)
            {
                MessageBox.Show("Giá sản phẩm phải lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!int.TryParse(textBoxSL.Text, out sl))
            {
                MessageBox.Show("Số lượng tồn phải là số", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (sl < 0)
            {
                MessageBox.Show("Số lượng tồn phải lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxID.ReadOnly)
            {
                Product product = new Product();
                if (comboBox1.SelectedItem != null)
                {
                    Category category = comboBox1.SelectedItem as Category;
                    product.idLoai = category.idLoai;
                }
                else
                {
                    MessageBox.Show("Loại sản phẩm không đúng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                product.idSp = int.Parse(textBoxID.Text);
                product.ten = textBoxTen.Text;
                product.gia = double.Parse(textBoxGia.Text);
                product.soLuong = int.Parse(textBoxSL.Text);
                if(dateTimePicker1.Checked)
                    product.ngayNhap = dateTimePicker1.Value;
                res = ProductDAO.Instance.Update(product);
            }
            else
            {
                int id = 0;
                if(int.TryParse(textBoxID.Text, out id))
                {
                    if(id == 0)
                    {
                        MessageBox.Show("ID sản phẩm phải khác 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Product product = ProductDAO.Instance.GetByID(id);
                    if(product != null)
                    {
                        MessageBox.Show("ID sản phẩm đã tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    product = new Product();
                    if (comboBox1.SelectedItem != null)
                    {
                        Category category = comboBox1.SelectedItem as Category;
                        product.idLoai = category.idLoai;
                    }
                    else
                    {
                        MessageBox.Show("Loại sản phẩm không đúng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    product.idSp = id;
                    product.ten = textBoxTen.Text;
                    product.gia = double.Parse(textBoxGia.Text);    
                    product.soLuong = int.Parse(textBoxSL.Text);
                    if (dateTimePicker1.Checked)
                        product.ngayNhap = dateTimePicker1.Value;
                    res = ProductDAO.Instance.Insert(product);
                }
                else
                {
                    MessageBox.Show("ID sản phẩm phải là số", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if(res)
                this.Close();
            else
                MessageBox.Show("Lưu không thành công", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
