using AutoMapper;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IAuditLogRepository; // ✅ Thêm namespace ghi log

using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Handles the update of a role.
    /// </summary>
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly IRoleCommandRepository _roleCommandRepository;
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IAuditLogRepository _auditLogRepository; // ✅ thêm repository log
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRoleCommandHandler"/> class.
        /// </summary>
        public UpdateRoleCommandHandler(
            IRoleCommandRepository roleCommandRepository,
            IPrivilegeRepository privilegeRepository,
            IAuditLogRepository auditLogRepository, // ✅ thêm vào constructor
            IMapper mapper,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleCommandRepository = roleCommandRepository;
            _privilegeRepository = privilegeRepository;
            _auditLogRepository = auditLogRepository; // ✅ gán vào
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the update role request.
        /// </summary>
        public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role with ID {RoleId}", request.RoleId);

            // ✅ 1. Kiểm tra role có tồn tại không
            var existingRole = await _roleCommandRepository.GetByIdWithTrackingAsync(request.RoleId, cancellationToken);
            if (existingRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", request.RoleId);
                throw new KeyNotFoundException($"Role with ID {request.RoleId} not found.");
            }

            // ✅ 2. Kiểm tra quyền hợp lệ
            var privileges = new List<Privilege>();
            if (request.Dto.PrivilegeIds != null && request.Dto.PrivilegeIds.Any())
            {
                privileges = await _privilegeRepository.GetPrivilegesByIdsAsync(request.Dto.PrivilegeIds, cancellationToken);
                if (privileges.Count != request.Dto.PrivilegeIds.Count)
                {
                    _logger.LogWarning("One or more privilege IDs are invalid");
                    throw new KeyNotFoundException("One or more privilege IDs are invalid.");
                }
            }

            // ✅ 3. Cập nhật thông tin role
            existingRole.Name = request.Dto.Name.Trim();
            existingRole.Description = request.Dto.Description;
            existingRole.Code = request.Dto.Code.Trim(); // ✅ cập nhật RoleCode
            // ✅ 4. Cập nhật lại danh sách quyền
            existingRole.RolePrivileges.Clear();
            foreach (var privilege in privileges)
            {
                existingRole.RolePrivileges.Add(new RolePrivilege
                {
                    RoleId = existingRole.RoleId,
                    PrivilegeId = privilege.PrivilegeId
                });
            }

            // ✅ Sau khi lưu thay đổi role thành công
            await _roleCommandRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully updated role with ID {RoleId}", request.RoleId);

            // ✅ Ghi log AC03 - chuẩn theo model AuditLog của bạn
            try
            {
                // Danh sách quyền mới
                var privilegeNames = privileges.Any()
                    ? string.Join(", ", privileges.Select(p => p.Name))
                    : "(no privileges)";

                var auditLog = new AuditLog
                {
                    UserEmail = "admin@fpt.com", // ⚠️ TODO: nếu bạn có HttpContext, lấy email từ token
                    EntityName = "Role",
                    Action = "Update",
                    Changes = $"Updated role '{existingRole.Name}' with privileges: {privilegeNames}",
                    Timestamp = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog, cancellationToken);

                _logger.LogInformation("Audit log recorded for updating role: {RoleName}", existingRole.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit log for role update (Role ID {RoleId})", request.RoleId);
            }


            // ✅ 7. Map lại DTO để trả về FE
            var roleDto = _mapper.Map<RoleDto>(existingRole);
            roleDto.Privileges = privileges.Select(p => p.Name).ToList();

            return roleDto;
        }
    }
}
