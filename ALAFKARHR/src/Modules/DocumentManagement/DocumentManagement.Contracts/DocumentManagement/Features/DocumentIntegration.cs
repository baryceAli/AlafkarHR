namespace DocumentManagement.Contracts.DocumentManagement.Features;

public record SourceDocumentReference(string SourceModule, string SourceEntity, Guid SourceRecordId);
