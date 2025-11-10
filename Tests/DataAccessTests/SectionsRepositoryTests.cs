using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Tests.Fixtures;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class SectionsRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private static readonly Mock<ISectionsRepository> mockSectionsRepo = new();
    private readonly SectionsRepository _repository = new(sqliteDbFixture.Context);
    private readonly RecipeRepository _recipeRepository = new(sqliteDbFixture.Context);
    private static readonly JObject _expectedViewRecipeSections;
    private static readonly JObject _expectedAddRecipeSections;

    static SectionsRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeSectionsDTO.json");
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipeSections.json");

        _expectedViewRecipeSections = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedAddRecipeSections = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }

    [Fact]
    public async Task AddNewSectionsAsync_Test()
    {
        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipeSections["recipeSummary"]?.ToObject<RecipeDTO>();
        // initialize the list of ingredient DTOs to add to the table
        var sectionsList = _expectedAddRecipeSections["recipeSections"]?.ToObject<List<SectionDTO>>();

        // initialize the number of records expected to be added to the table: 3
        int expectedRecordCount = 3;
        // initialize the expected list <SectionDTO> return
        List<SectionDTO> expectedSections = [];
        for(int i = 0; i < sectionsList!.Count; i++)
        {
            expectedSections.Add(new SectionDTO
            {
                Id = i + 4,
                RecipeId = 4,
                SectionName = sectionsList[i].SectionName,
                SortOrder = sectionsList[i].SortOrder
            });
        }
        // mock the return of the dependent function GetSectionsByRecipeAsync
        mockSectionsRepo.Setup(r => r.GetSectionsByRecipeAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedSections);
        // convert expected result into JSON
        var expectedJson = JsonConvert.SerializeObject(expectedSections);
        JToken expectedSectionsToken = JToken.Parse(expectedJson);

        // call AddNewRecipeAsync with the DTO -- this is necessary to avoid foreign key violations
        var recipeId = await _recipeRepository.AddNewRecipeAsync(addRecipeDTO!);
        // call AddNewRecipeSectionsAsync with the sections list and recipe ID
        (int records, var sections) = await _repository.AddNewRecipeSectionsAsync(sectionsList!, recipeId);

        // Convert sections result to JSON Token
        var actualJson = JsonConvert.SerializeObject(sections);
        JToken actualToken = JToken.Parse(actualJson);

        // assert the expected record count is equal to the actual record count
        Assert.Equal(expectedRecordCount, records);
        // assert the expected list of section DTOs is equal to the returned list
        Assert.True(JToken.DeepEquals(expectedSectionsToken, actualToken));
    }
    
    [Fact]
    public async Task GetSectionsByRecipeAsync_Test()
    {
        // initialize test recipe ID
        int recipeId = 3;
        // initialize expected sections as a list of DTOs for case matching
        var expectedSectionsDTO = _expectedViewRecipeSections["RecipeSections"]?.ToObject<List<SectionDTO>>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedSectionsDTO);
        JToken expectedSectionsToken = JToken.Parse(expectedJson);

        // Call GetSectionsByRecipeAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await _repository.GetSectionsByRecipeAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedSectionsToken, actualToken));
    }
}