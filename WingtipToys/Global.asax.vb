Imports System.Web.Optimization
Imports System.Data.Entity
Imports WingtipToys.Models
Imports WingtipToys.Logic

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)

        Database.SetInitializer(New ProductDatabaseInitializer())

        Dim roleActions As New RoleActions()
        roleActions.AddUserAndRole()

        Dim routeActions As New RouteActions()
        routeActions.RegisterCustomRoutes(RouteTable.Routes)
    End Sub
End Class