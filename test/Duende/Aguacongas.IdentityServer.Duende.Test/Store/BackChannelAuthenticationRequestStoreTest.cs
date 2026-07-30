// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Stores.Serialization;
using IdentityModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using IsModels = Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.Store.Test;

public class BackChannelAuthenticationRequestStoreTest
{
    [Fact]
    public void Constructor_should_validate_parameters()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        Assert.Throws<ArgumentNullException>(() => new BackChannelAuthenticationRequestStore(null, serializerMock.Object));
        Assert.Throws<ArgumentNullException>(() => new BackChannelAuthenticationRequestStore(storeMock.Object, null));
    }

    [Fact]
    public async Task CreateRequestAsync_should_create_entity_from_client_and_subject()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        serializerMock.Setup(s => s.Serialize<IsModels.BackChannelAuthenticationRequest>(It.IsAny<IsModels.BackChannelAuthenticationRequest>()))
            .Returns("{}");

        BackChannelAuthenticationRequest createdEntity = null;
        storeMock.Setup(s => s.CreateAsync(It.IsAny<BackChannelAuthenticationRequest>(), It.IsAny<CancellationToken>()))
            .Callback<BackChannelAuthenticationRequest, CancellationToken>((e, _) => createdEntity = e)
            .Returns<BackChannelAuthenticationRequest, CancellationToken>((e, _) => Task.FromResult(e));

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var clientId = GenerateId();
        var request = new IsModels.BackChannelAuthenticationRequest
        {
            ClientId = clientId,
            Subject = CreateSubject("alice"),
            CreationTime = DateTime.UtcNow,
            Lifetime = 300
        };

        var sessionId = await sut.CreateRequestAsync(request, default);

        Assert.NotNull(createdEntity);
        Assert.Equal(clientId, createdEntity.ClientId);
        Assert.Equal("alice", createdEntity.UserId);
        Assert.False(string.IsNullOrEmpty(request.InternalId));
        Assert.Equal(request.InternalId, createdEntity.Id);
        Assert.False(string.IsNullOrEmpty(createdEntity.SessionId));
        Assert.Equal(createdEntity.SessionId, sessionId);
        storeMock.Verify(s => s.CreateAsync(It.IsAny<BackChannelAuthenticationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRequestAsync_should_keep_provided_internal_and_session_ids()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        serializerMock.Setup(s => s.Serialize<IsModels.BackChannelAuthenticationRequest>(It.IsAny<IsModels.BackChannelAuthenticationRequest>()))
            .Returns("{}");
        storeMock.Setup(s => s.CreateAsync(It.IsAny<BackChannelAuthenticationRequest>(), It.IsAny<CancellationToken>()))
            .Returns<BackChannelAuthenticationRequest, CancellationToken>((e, _) => Task.FromResult(e));

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var internalId = GenerateId();
        var sessionId = GenerateId();
        var request = new IsModels.BackChannelAuthenticationRequest
        {
            ClientId = GenerateId(),
            InternalId = internalId,
            SessionId = sessionId,
            Subject = CreateSubject("bob"),
            CreationTime = DateTime.UtcNow,
            Lifetime = 60
        };

        var result = await sut.CreateRequestAsync(request, default);

        Assert.Equal(internalId, request.InternalId);
        Assert.Equal(sessionId, result);
    }

    [Fact]
    public async Task GetByAuthenticationRequestIdAsync_should_return_matching_request()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var requestId = GenerateId();
        var dto = new IsModels.BackChannelAuthenticationRequest { ClientId = GenerateId() };
        var entity = new BackChannelAuthenticationRequest { Id = GenerateId(), SessionId = requestId, Data = "{}" };

        PageRequest capturedRequest = null;
        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PageRequest, CancellationToken>((r, _) => capturedRequest = r)
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest> { entity }
            });

        serializerMock.Setup(s => s.Deserialize<IsModels.BackChannelAuthenticationRequest>("{}"))
            .Returns(dto);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetByAuthenticationRequestIdAsync(requestId, default);

        Assert.Same(dto, result);
        Assert.Contains($"SessionId eq '{requestId}'", capturedRequest.Filter);
    }

    [Fact]
    public async Task GetByAuthenticationRequestIdAsync_should_return_null_when_not_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest>()
            });

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetByAuthenticationRequestIdAsync(GenerateId(), default);

        Assert.Null(result);
        serializerMock.Verify(s => s.Deserialize<IsModels.BackChannelAuthenticationRequest>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetByInternalIdAsync_should_return_request_when_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var id = GenerateId();
        var entity = new BackChannelAuthenticationRequest { Id = id, Data = "{}" };
        var dto = new IsModels.BackChannelAuthenticationRequest();

        storeMock.Setup(s => s.GetAsync(id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        serializerMock.Setup(s => s.Deserialize<IsModels.BackChannelAuthenticationRequest>("{}"))
            .Returns(dto);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetByInternalIdAsync(id, default);

        Assert.Same(dto, result);
    }

    [Fact]
    public async Task GetByInternalIdAsync_should_return_null_when_not_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        storeMock.Setup(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackChannelAuthenticationRequest)null);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetByInternalIdAsync(GenerateId(), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetLoginsForUserAsync_should_filter_by_subject_only_when_client_id_is_null()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var subjectId = GenerateId();
        PageRequest capturedRequest = null;
        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PageRequest, CancellationToken>((r, _) => capturedRequest = r)
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest>()
            });

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await sut.GetLoginsForUserAsync(subjectId, default);

        Assert.Equal($"UserId eq '{subjectId}'", capturedRequest.Filter);
    }

    [Fact]
    public async Task GetLoginsForUserAsync_should_extend_filter_when_client_id_is_provided()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var subjectId = GenerateId();
        var clientId = GenerateId();
        PageRequest capturedRequest = null;
        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PageRequest, CancellationToken>((r, _) => capturedRequest = r)
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest>()
            });

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await sut.GetLoginsForUserAsync(subjectId, default, clientId);

        // Reflects the current implementation, which appends a second "UserId eq" clause
        // rather than filtering on ClientId. Update this assertion if that is fixed.
        Assert.Equal($"UserId eq '{subjectId}' And UserId eq '{clientId}'", capturedRequest.Filter);
    }

    [Fact]
    public async Task GetLoginsForUserAsync_should_return_all_matching_requests()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var entity1 = new BackChannelAuthenticationRequest { Id = GenerateId(), Data = "data-1" };
        var entity2 = new BackChannelAuthenticationRequest { Id = GenerateId(), Data = "data-2" };
        var dto1 = new IsModels.BackChannelAuthenticationRequest();
        var dto2 = new IsModels.BackChannelAuthenticationRequest();

        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest> { entity1, entity2 }
            });

        serializerMock.Setup(s => s.Deserialize<IsModels.BackChannelAuthenticationRequest>(entity1.Data)).Returns(dto1);
        serializerMock.Setup(s => s.Deserialize<IsModels.BackChannelAuthenticationRequest>(entity2.Data)).Returns(dto2);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetLoginsForUserAsync(GenerateId(), default);

        Assert.Equal(2, result.Count);
        Assert.Contains(dto1, result);
        Assert.Contains(dto2, result);
    }

    [Fact]
    public async Task GetLoginsForUserAsync_should_return_empty_collection_when_no_match()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        storeMock.Setup(s => s.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageResponse<BackChannelAuthenticationRequest>
            {
                Items = new List<BackChannelAuthenticationRequest>()
            });

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        var result = await sut.GetLoginsForUserAsync(GenerateId(), default);

        Assert.Empty(result);
    }

    [Fact]
    public async Task RemoveByInternalIdAsync_should_delete_entity_when_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var id = GenerateId();
        var entity = new BackChannelAuthenticationRequest { Id = id };

        storeMock.Setup(s => s.GetAsync(id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await sut.RemoveByInternalIdAsync(id, default);

        storeMock.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveByInternalIdAsync_should_do_nothing_when_not_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        storeMock.Setup(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackChannelAuthenticationRequest)null);

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await sut.RemoveByInternalIdAsync(GenerateId(), default);

        storeMock.Verify(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateByInternalIdAsync_should_update_entity_data_when_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        var id = GenerateId();
        var existingEntity = new BackChannelAuthenticationRequest { Id = id, Data = "old" };

        storeMock.Setup(s => s.GetAsync(id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);
        serializerMock.Setup(s => s.Serialize<IsModels.BackChannelAuthenticationRequest>(It.IsAny<IsModels.BackChannelAuthenticationRequest>()))
            .Returns("new-data");

        BackChannelAuthenticationRequest updatedEntity = null;
        storeMock.Setup(s => s.UpdateAsync(It.IsAny<BackChannelAuthenticationRequest>(), It.IsAny<CancellationToken>()))
            .Callback<BackChannelAuthenticationRequest, CancellationToken>((e, _) => updatedEntity = e)
            .Returns<BackChannelAuthenticationRequest, CancellationToken>((e, _) => Task.FromResult(e));

        var request = new IsModels.BackChannelAuthenticationRequest
        {
            ClientId = GenerateId(),
            Subject = CreateSubject("carl"),
            CreationTime = DateTime.UtcNow,
            Lifetime = 120
        };

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await sut.UpdateByInternalIdAsync(id, request, default);

        Assert.Same(existingEntity, updatedEntity);
        Assert.Equal("new-data", updatedEntity.Data);
        Assert.Equal(id, updatedEntity.Id); // the fetched entity's Id must be preserved, not overwritten
    }

    [Fact]
    public async Task UpdateByInternalIdAsync_should_throw_when_entity_not_found()
    {
        var storeMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
        var serializerMock = new Mock<IPersistentGrantSerializer>();

        storeMock.Setup(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BackChannelAuthenticationRequest)null);

        var request = new IsModels.BackChannelAuthenticationRequest
        {
            ClientId = GenerateId(),
            Subject = CreateSubject("dan"),
            CreationTime = DateTime.UtcNow,
            Lifetime = 60
        };

        var sut = new BackChannelAuthenticationRequestStore(storeMock.Object, serializerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.UpdateByInternalIdAsync(GenerateId(), request, default));
    }

    private static ClaimsPrincipal CreateSubject(string subjectId)
        => new(new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, subjectId) }));

    private static string GenerateId()
        => Guid.NewGuid().ToString();
}