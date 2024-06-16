namespace WindowsFormsAppEditTable2.Models
{
    public class Category
    {
        public int idLoai { get; set; }
        public string tenLoaiSp { get; set; }

        public string nameDisplay { get => $"{idLoai} - {tenLoaiSp}"; set { nameDisplay = value; } }
    }
}
