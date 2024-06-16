using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppEditTable2.Models
{
    public class Product
    {
        public Product()
        {

        }

        public int idSp { get; set; }
        public string ten { get; set; }
        public double gia { get; set; }
        public int soLuong { get; set; }
        public int idLoai { get; set; }
        public string tenLoaiSp { get; set; }
        public DateTime? ngayNhap { get; set; }
    }
}
