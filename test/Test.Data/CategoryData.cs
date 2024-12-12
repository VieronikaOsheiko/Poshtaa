using Domain.Category;

namespace Test.Data;

public static class CategoryData
{
    public static Category DefaultCategory
        => Category.New(CategoryId.New(), name: "Default Category", inCountry: true, material: "Wood", size: "Medium");
    public static Category TestCategory
        => Category.New(CategoryId.New(), name: "TestCategory", inCountry: true, material: "Wood", size: "Medium");
    
}