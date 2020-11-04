Imports WingtipToys.Models
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework

Namespace Logic
    Friend Class RoleActions
        Friend Sub AddUserAndRole()
            Dim context = New ApplicationDbContext()
            Dim IdRoleResult As IdentityResult
            Dim IdUserResult As IdentityResult

            Dim roleStore As New RoleStore(Of IdentityRole)(context)
            Dim roleMgr As New RoleManager(Of IdentityRole)(roleStore)

            If Not roleMgr.RoleExists("canEdit") Then
                IdRoleResult = roleMgr.Create(New IdentityRole With {.Name = "canEdit"})
            End If

            Dim userMgr As New UserManager(Of ApplicationUser)(New UserStore(Of ApplicationUser)(context))
            Dim appUser = New ApplicationUser With {
                .UserName = "canEditUser@wingtiptoys.com",
                .Email = "canEditUser@wingtiptoys.com"
            }
            IdUserResult = userMgr.Create(appUser, "Pa$$word1")

            If Not userMgr.IsInRole(userMgr.FindByEmail("canEditUser@wingtiptoys.com").Id, "canEdit") Then
                IdUserResult = userMgr.AddToRole(userMgr.FindByEmail("canEditUser@wingtiptoys.com").Id, "canEdit")
            End If
        End Sub
    End Class

End Namespace
