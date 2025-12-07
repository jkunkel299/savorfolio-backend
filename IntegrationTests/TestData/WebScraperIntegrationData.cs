using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace IntegrationTests.TestData;

public class WebScraperIntegrationData
{
    public static TheoryData<string, string> WebScraperTestCases() =>
        new()
        {
            // url
            // file path
            {
                "https://www.modernhoney.com/fall-chocolate-chip-spiced-cookie-levain-bakery-fall-cookie-knock-off/#wprm-recipe-container-24948",
                "ExpectedData/wprmDraftDTO.json"
            },
            {
                "https://www.gimmesomeoven.com/mushroom-stroganoff",
                "ExpectedData/tastyDraftDTO.json"
            },
            {
                "https://keytomylime.com/jiffy-cornbread-with-creamed-corn-recipe",
                "ExpectedData/mvCreateDraftDTO.json"
            },
            {
                "https://fashionablefoods.com/2014/06/27/2014618roasted-chili-lime-cod",
                "ExpectedData/noPatternDraftDTO.json"
            },
        };
}
