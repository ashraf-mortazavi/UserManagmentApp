using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Persistence.DataSeeder;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IServiceProvider services, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        if (!await db.Roles.AnyAsync(ct))
        {
            Role superAdminRole = new Role { Name = "SuperAdmin", Description = "Full access" };
            await roleManager.CreateAsync(superAdminRole);
            Role adminRole = new Role { Name = "Admin", Description = "Administrative access" };
            await roleManager.CreateAsync(adminRole);
            Role managerRole = new Role { Name = "Manager", Description = "Managerial access" };
            await roleManager.CreateAsync(managerRole);
            Role operatorRole = new Role { Name = "Operator", Description = "Operatorial access" };
            await roleManager.CreateAsync(operatorRole);

            await db.SaveChangesAsync(ct);

            superAdminRole.NextLowerRoleId = adminRole.Id;
            adminRole.NextLowerRoleId = managerRole.Id;
            managerRole.NextLowerRoleId = operatorRole.Id;
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Permissions.AnyAsync(ct))
        {
            List<Permission> permissions = new List<Permission>()
            {
                new Permission { Key = "menu.all.view", Name = "View All" },
                new Permission { Key = "menu.dashboard.view", Name = "View Dashboard" },
                new Permission { Key = "menu.users.view", Name = "View Users" },
                new Permission { Key = "menu.users.create", Name = "Create User" },
                new Permission { Key = "menu.reports.view", Name = "View Reports" },
                new Permission { Key = "menu.create.progress", Name = "Create Progress" },
                new Permission { Key = "menu.progress.view", Name = "View Progress" },
                new Permission { Key = "menu.create.operator", Name = "Create Operator" },
                new Permission { Key = "menu.view.operator", Name = "View Operator" }
            };
            db.Permissions.AddRange(permissions);
            await db.SaveChangesAsync(ct);
        }

        Dictionary<string, Role> roles = await db.Roles.ToDictionaryAsync(x => x.Name, ct);
        Dictionary<string, Permission> allPermissions = await db.Permissions.ToDictionaryAsync(x => x.Key, ct);

        if (!await db.RolePermissions.AnyAsync(ct))
        {
            // SuperAdmin role permissions (still IsSuperAdmin bypass exists)
            List<int> superAdminPermissionIds = allPermissions.Values.Select(p => p.Id).ToList();
            db.RolePermissions.AddRange(superAdminPermissionIds.Select(pid =>
            new RolePermission
            {
                RoleId = roles["SuperAdmin"].Id,
                PermissionId = pid
            }));


            // Admin permissions
            var adminPermissions = new[]
            {

                "menu.users.view",
                "menu.users.create",
                "menu.create.progress",
                "menu.progress.view",
                "menu.create.operator",
                "menu.view.operator"
            };

            db.RolePermissions.AddRange(adminPermissions.Select(k => new RolePermission
            {
                RoleId = roles["Admin"].Id,
                PermissionId = allPermissions[k].Id
            }));
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Users.AnyAsync(ct))
        {
            User user = new User
            {
                FirstName = "Ashraf",
                LastName = "Mortazavi",
                NationalCode = "0670252204",
                Enabled = true,
                CreatedById = null,
                CreatedAt = DateTime.UtcNow
            };

            user.UserName = "Ashraf";
            user.Email = "ashraf@example.com";
            user.PhoneNumber = "09157732147";

            var result = await userManager.CreateAsync(user, "Am@700511");
            if (result.Succeeded)
            {
                await db.SaveChangesAsync(ct);

                var createdUser = await userManager.FindByNameAsync("Ashraf");

                var superAdminRoleExist = await roleManager.FindByNameAsync("SuperAdmin");
                if (superAdminRoleExist == null)
                {
                    superAdminRoleExist = await roleManager.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "SUPERADMIN");
                }
                await userManager.AddToRoleAsync(createdUser, superAdminRoleExist.Name);
                await db.SaveChangesAsync(ct);
            }
        }

        if (!await db.Zones.AnyAsync(ct))
        {
            var zones = new List<Zone>
            {
                new Zone { Name = "تهران", Description = "استان تهران" },
                new Zone { Name = "اصفهان", Description = "استان اصفهان" },
                new Zone { Name = "فارس", Description = "استان فارس" },
                new Zone { Name = "خراسان رضوی", Description = "استان خراسان رضوی" },
                new Zone { Name = "آذربایجان شرقی", Description = "استان آذربایجان شرقی" }
            };
            db.Zones.AddRange(zones);
            await db.SaveChangesAsync(ct);

            var savedZones = await db.Zones.ToListAsync(ct);

            var areas = new List<Area>
            {
                // تهران
                new Area { Name = "شمال تهران", Description = "مناطق شمالی شهر تهران شامل تجریش، نیاوران، فرمانیه", ZoneId = savedZones[0].Id },
                new Area { Name = "جنوب تهران", Description = "مناطق جنوبی شهر تهران شامل شوش، مولوی، جوادیه", ZoneId = savedZones[0].Id },
                new Area { Name = "شرق تهران", Description = "مناطق شرقی شهر تهران شامل تهرانپارس، مجیدیه، وحیدیه", ZoneId = savedZones[0].Id },
                new Area { Name = "غرب تهران", Description = "مناطق غربی شهر تهران شامل تهرانسر، شهران، چیتگر", ZoneId = savedZones[0].Id },
                new Area { Name = "مرکز تهران", Description = "مناطق مرکزی شهر تهران شامل انقلاب، آزادی، امیرآباد", ZoneId = savedZones[0].Id },

                // اصفهان
                new Area { Name = "مرکز اصفهان", Description = "میدان نقش جهان، کاخ عالی قاپو، بازار بزرگ", ZoneId = savedZones[1].Id },
                new Area { Name = "شرق اصفهان", Description = "منطقه جی، خمینی شهر، دولت آباد", ZoneId = savedZones[1].Id },
                new Area { Name = "غرب اصفهان", Description = "منطقه زرین شهر، فولادشهر، مبارکه", ZoneId = savedZones[1].Id },

                // فارس
                new Area { Name = "شیراز", Description = "مرکز استان فارس، حافظیه، ناصرخسرو", ZoneId = savedZones[2].Id },
                new Area { Name = "کازرون", Description = "شهرستان کازرون، بیشاپور", ZoneId = savedZones[2].Id },
                new Area { Name = "لارستان", Description = " شهرستان لار، بندرعباس هرمزگان", ZoneId = savedZones[2].Id },

                // خراسان رضوی
                new Area { Name = "مشهد", Description = "مرکز استان خراسان رضوی، حرم امام رضا(ع)", ZoneId = savedZones[3].Id },
                new Area { Name = "نیشابور", Description = "شهرستان نیشابور، آرامگاه عطار", ZoneId = savedZones[3].Id },
                new Area { Name = "سبزوار", Description = "شهرستان سبزوار، خوشاب", ZoneId = savedZones[3].Id },

                // آذربایجان شرقی
                new Area { Name = "تبریز", Description = "مرکز استان آذربایجان شرقی، بازار تبریز", ZoneId = savedZones[4].Id },
                new Area { Name = "مراغه", Description = "شهرستان مراغه، رصدخانه مراغه", ZoneId = savedZones[4].Id },
                new Area { Name = "اهر", Description = "شهرستان اهر، کلیسای استفانوس مقدس", ZoneId = savedZones[4].Id }
            };
            db.Areas.AddRange(areas);
            await db.SaveChangesAsync(ct);
        }
    }
}