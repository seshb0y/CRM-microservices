using CRMSolution.Grpc.Users;

namespace CRMSolution.DTO.Requests;

public record HttpChangeUserDataRequest(string username, string newEmail, UserRole role, string oldEmail);