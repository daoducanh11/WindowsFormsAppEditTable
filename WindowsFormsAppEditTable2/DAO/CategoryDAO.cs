using System.Data;

namespace WindowsFormsAppEditTable2.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;
        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new CategoryDAO();
                return CategoryDAO.instance;
            }
            private set
            {
                CategoryDAO.instance = value;
            }
        }
        private CategoryDAO() { }
        public DataTable GetCategories()
        {
            string query = $"SELECT * FROM LoaiSp";
            return DataProvider.Instance.ExecuteQuery(query);
        }
    }
}
