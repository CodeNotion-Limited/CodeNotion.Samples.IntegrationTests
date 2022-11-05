namespace CodeNotion.Template.Business.Enums;

public enum ErrorType
{
    EntityNotFound,
    CannotUpdateDatabase,
    NotHandled,
    EmptyFile,
    FileNotFound,
    InvalidSchema,
    InvalidId,
    SerialRequired,
    MeshGroupNotFound,
    PartialSerial,
    InvalidBomFilter,
    SourceError,
    BothMeshGroupIdAndMachineAccessoryIdAreNotAllowed,
    InvalidRequest,
    SerialOrMachineVariantRequired,
    SerialNotFound,
    MissingInput,
    OrderAlreadyConfirmed
}