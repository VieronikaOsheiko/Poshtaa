namespace Domain.Category
{
    public class Category
    {
        public CategoryId Id { get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        public bool InCountry { get; set; }
        public string Size { get; set; }

        
        public Category(CategoryId id, string name, bool inCountry, string material, string size)
        {
            Id = id;
            Name = name;
            Material = material;
            InCountry = inCountry;
            Size = size;
        }

        public static Category New(CategoryId id, string name, bool inCountry, string material, string size)
            => new Category(id, name, inCountry, material, size);
        public void UpdateDetails(string name,bool inCountry, string material, string sizer)
        {
            Name = name;
            Material = material;
            InCountry = inCountry;
            Size = sizer;
        }
    }
}