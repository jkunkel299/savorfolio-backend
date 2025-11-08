namespace Tests.TestData;

public class IngredientFallbackData
{
    private static readonly string[] ingredients =
        [
            "1 1/2 teaspoons instant yeast (SAF Gold instant yeast suggested)",
            "2 1/4 cups (270 grams) all purpose flour",
            "1/2 teaspoon salt",
            "1/2 cup (113.5 grams) whole milk",
        ];

    public static TheoryData<string, string[]> IngredientFallbackTestCases() => new()
    {
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <h2>Ingredients</h2>
                <ul class='ingredient-list'>
                    <li class='ingredient-1'>1 1/2 teaspoons instant yeast (SAF Gold instant yeast suggested)</li>
                    <li class='ingredient-2'>2 1/4 cups (270 grams) all purpose flour</li>
                    <li class='ingredient-3'>1/2 teaspoon salt</li>
                    <li class='ingredient-4'>1/2 cup (113.5 grams) whole milk</li>
                </ul>
            </body></html>",
            ingredients
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <h2>Ingredients</h2>
                <ul>
                    <li>1 1/2 teaspoons instant yeast (SAF Gold instant yeast suggested)</li>
                    <li>2 1/4 cups (270 grams) all purpose flour</li>
                    <li>1/2 teaspoon salt</li>
                    <li>1/2 cup (113.5 grams) whole milk</li>
                </ul>
            </body></html>",
            ingredients
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <ul>
                    <li>1 1/2 teaspoons instant yeast (SAF Gold instant yeast suggested)</li>
                    <li>2 1/4 cups (270 grams) all purpose flour</li>
                    <li>1/2 teaspoon salt</li>
                    <li>1/2 cup (113.5 grams) whole milk</li>
                </ul>
            </body></html>",
            ingredients
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            </body></html>",
            ["Could not find ingredients"]
        }
    };
}