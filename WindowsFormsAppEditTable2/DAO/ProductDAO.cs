using System.Data;
using WindowsFormsAppEditTable2.Models;

namespace WindowsFormsAppEditTable2.DAO
{
    public class ProductDAO
    {
        private static ProductDAO instance;
        public static ProductDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProductDAO();
                return ProductDAO.instance;
            }
            private set
            {
                ProductDAO.instance = value;
            }
        }
        private ProductDAO() { }
        public DataTable GetProducts(int currentPage, int pageSize)
        {
            string query = $"SELECT idSp, ten, gia, soLuong, SanPham.idLoai, LoaiSp.tenLoaiSp [tenLoaiSp], ngayNhap FROM SanPham LEFT JOIN LoaiSp ON SanPham.idLoai = LoaiSp.idLoai ORDER BY idSp DESC OFFSET {(currentPage - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public int GetCountProduct()
        {
            return (int)DataProvider.Instance.ExecuteScalar("SELECT COUNT(1) FROM SanPham");
        }

        public Product GetByID(int id)
        {
            string query = $"SELECT * FROM SanPham WHERE idSp = {id}";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
                return Utils.Util.GetItem<Product>(item);
            return null;
        }

        public bool Insert(Product item)
        {
            string query = $"INSERT INTO SanPham (idSp, ten, gia, soLuong, idLoai, ngayNhap) VALUES({item.idSp}, N'{item.ten}', {item.gia}, {item.soLuong}, {item.idLoai}, " + (item.ngayNhap.HasValue ? ($"'{item.ngayNhap.Value.ToString("MM/dd/yyyy HH:mm:ss")}'") : "NULL") + ")";
            return DataProvider.Instance.ExecuteNonQuery(query) > 0;
        }

        public bool Update(Product item)
        {
            string query = $"UPDATE SanPham SET ten = N'{item.ten}', gia = {item.gia}, soLuong = {item.soLuong}, idLoai = {item.idLoai}, ngayNhap = " + (item.ngayNhap.HasValue ? ($"'{item.ngayNhap.Value.ToString("MM/dd/yyyy HH:mm:ss")}'") : "NULL") + $" WHERE idSp = {item.idSp}";
            return DataProvider.Instance.ExecuteNonQuery(query) > 0;
        }

        public bool Delete(int idSp)
        {
            string query = string.Format("DELETE SanPham WHERE idSp = {0}", idSp);
            return DataProvider.Instance.ExecuteNonQuery(query) > 0;
        }
    }
}
