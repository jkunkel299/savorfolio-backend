namespace Tests.TestData;

public class GetIngByPatternData
{
    public static TheoryData<string, string, string[]> GetIngByPatternTestCases() => new()
    {
        // document
        // pattern
        // expected
        {
            "wprmPattern.html",
            "wprm-recipe-ingredient",
            [
                "12 Tablespoons Butter (cold, cut into cubes)",
                "1/2 cup Dark Brown Sugar(may substitute light)",
                "1/2 cup Sugar",
                "1 large Egg",
                "2 Tablespoons Molasses",
                "2 cups Flour",
                "1 teaspoon Baking Soda",
                "1 teaspoon Cornstarch",
                "1/2 teaspoon Salt (1/4 tsp if you using salted butter)",
                "1 teaspoon Cinnamon",
                "1 1/4 teaspoon Ground Ginger *(may reduce to 1 teaspoon)",
                "1/2 teaspoon Ground Nutmeg(may reduce to 1/4 teaspoon)",
                "1/8 teaspoon Ground Cloves(optional)",
                "1 1/2 cups Chocolate Chips or Chunks"
            ]
        },
        {
            "tastyPattern.html",
            "tasty-recipes-ingredients-body",
            [
                "1 pound wide egg noodles",
                "3 tablespoons butter, divided",
                "1 small white onion, thinly sliced",
                "4 cloves garlic, minced",
                "1 pound baby bella mushrooms*",
                "1/2 cup dry white wine",
                "1.5 cups vegetable stock",
                "1 tablespoon Worcestershire sauce (here is a vegetarian brand)",
                "3 1/2 tablespoons flour",
                "3 small sprigs of fresh thyme (or 1/4 teaspoon dried thyme)",
                "1/2 cup plain Greek yogurt or light sour cream",
                "Kosher salt and freshly-cracked black pepper",
                "optional toppings: freshly-grated Parmesan cheese, chopped fresh parsley, extra black pepper"
            ]
        },
        {
            "mv-createPattern.html",
            "mv-create-ingredients",
            [
                "2 large eggs",
                "1/3 cup vegetable oil",
                "1/3 cup milk",
                "2 Tablespoons honey",
                "1 (14.75 ounce) can cream style corn",
                "1/2 cup sour cream",
                "2 (8.5 ounce) boxes Jiffy Corn Muffin Mix"
            ]
        }
    };
}