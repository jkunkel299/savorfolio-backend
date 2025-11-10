using AngleSharp.Dom;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IFallbackHeuristics
{
    // string GetBestMatch(IEnumerable<IElement> labelElements, string labelPattern);
    // string MatchEnum<TEnum>(IDocument document) where TEnum : Enum;
    string ExtractTitle(IDocument document);
    string ExtractDescription(IDocument document);
    string ExtractTimeNearLabel(IDocument document, string labelPattern);
    string ExtractServings(IDocument document);
    TagStringsDTO ExtractTags(IDocument document);
    // List<string> ExtractDietaryTags(string documentText);
    List<InstructionDTO> ExtractInstructions(IDocument document);
    (int? temp, string? temp_unit) ExtractBakeTemp(IDocument document);
}