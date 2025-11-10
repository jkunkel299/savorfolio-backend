using savorfolio_backend.Models.DTOs;

namespace Tests.TestData;

public class FallbackInstructionData
{
    public static TheoryData<string, InstructionDTO[]> InstructionTestCases() => new()
    {
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <h2>Instructions:</h2>
                <ol class='instructions-list'>
                    <li class='instruction-1'>Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined.</li>
                    <li class='instruction-2'>On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes.</li>
                </ol>
            </body></html>",
            new InstructionDTO[]
            {
                new() { StepNumber = 1, InstructionText = "Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined." },
                new() { StepNumber = 2, InstructionText = "On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes." }
            }
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <h2>Instructions:</h2>
                <ol>
                    <li>Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined.</li>
                    <li>On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes.</li>
                </ol>
            </body></html>",
            new InstructionDTO[]
            {
                new() { StepNumber = 1, InstructionText = "Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined." },
                new() { StepNumber = 2, InstructionText = "On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes." }
            }
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <h2>Directions:</h2>
                <ol>
                    <li>Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined.</li>
                    <li>On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes.</li>
                </ol>
            </body></html>",
            new InstructionDTO[]
            {
                new() { StepNumber = 1, InstructionText = "Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined." },
                new() { StepNumber = 2, InstructionText = "On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes." }
            }
        },
        {
            @"<!DOCTYPE html>
            <html><body>
                <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
                <ol>
                    <li>Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined.</li>
                    <li>On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes.</li>
                </ol>
            </body></html>",
            new InstructionDTO[]
            {
                new() { StepNumber = 1, InstructionText = "Prepare the dough. In the bowl of a stand mixer or a large mixing bowl, combine the yeast, flour, and salt and mix until combined." },
                new() { StepNumber = 2, InstructionText = "On low speed, beat in the milk, sugar, egg, and vanilla extract. Mix just until the dough comes together, another 2-3 minutes." }
            }
        }

    };
}