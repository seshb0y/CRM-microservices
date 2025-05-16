namespace ControllerFirst.DTO.Requests;

public record HttpRegisterRequest
(string Username, string Password, string ConfirmPassword, string Email);


