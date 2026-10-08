using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;

namespace Staybnb.Services
{
    public class HostApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HostApplicationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Approves a HostApplication:
        /// 1. Adds Host role
        /// 2. Removes Guest role (user is either Guest OR Host, not both)
        /// 3. Logs the action
        /// </summary>
        public async Task<bool> ApproveApplicationAsync(int applicationId, string adminId)
        {
            var application = await _context.HostApplications
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Status == ApplicationStatus.Approved)
                return false;

            var user = application.ApplicationUser;
            if (user == null) return false;

            // Add Host role
            if (!await _userManager.IsInRoleAsync(user, "Host"))
            {
                await _userManager.AddToRoleAsync(user, "Host");
            }

            // Remove Guest role
            if (await _userManager.IsInRoleAsync(user, "Guest"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Guest");
            }

            // Update status
            application.Status = ApplicationStatus.Approved;

            // Log activity
            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = adminId,
                Action = $"Approved host application for {user.Email}",
                ActivityType = ActivityType.ApplicationApproved,
                CreatedAt = DateTime.Now
            });

            // Notify user
            _context.Notifications.Add(new Notification
            {
                UserId = user.Id,
                Title = "Host Application Approved",
                Message = $"Congratulations! Your application for '{application.Property?.Title}' has been approved.",
                Type = NotificationType.ApplicationApproved
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, string adminId, string notes)
        {
            var application = await _context.HostApplications
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) return false;

            application.Status = ApplicationStatus.Rejected;
            application.AdminNotes = notes;

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = adminId,
                Action = $"Rejected host application ID {applicationId}",
                ActivityType = ActivityType.ApplicationRejected,
                CreatedAt = DateTime.Now
            });

            if (application.ApplicationUser != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = application.ApplicationUser.Id,
                    Title = "Host Application Rejected",
                    Message = $"Unfortunately, your application was not approved. Notes: {notes}",
                    Type = NotificationType.ApplicationRejected
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}