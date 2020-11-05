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

    Sub Application_Error(sender As Object, e As EventArgs)
        Dim exc As Exception = Server.GetLastError()

        If TypeOf exc Is HttpUnhandledException Then
            If exc.InnerException IsNot Nothing Then
                exc = New Exception(exc.InnerException.Message)
                Server.Transfer("ErrorPage.aspx?handler=Application_Error%20-%20Global.asax", True)
            End If
        End If
    End Sub
End Class