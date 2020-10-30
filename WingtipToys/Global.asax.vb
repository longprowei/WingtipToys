Imports System.Web.Optimization
Imports System.Data.Entity
Imports WingtipToys.Models

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)

        Database.SetInitializer(New ProductDatabaseInitializer())
    End Sub
End Class