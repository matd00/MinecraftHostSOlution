using ServerHost.Domain.Enums;

namespace ServerHost.Application.DTOs;

public record GameServerDto(
    Guid Id,
    string Name,
    int Port,
    ServerStatus Status,
    DateTime CreatedAt
);
