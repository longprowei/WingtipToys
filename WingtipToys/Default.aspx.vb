Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Private Sub Page_Error(ByVal sender As Object, ByVal e As EventArgs)
        Dim exc As Exception = Server.GetLastError()

        If TypeOf exc Is InvalidOperationException Then
            Server.Transfer("ErrorPage.aspx?handler=Page_Error%20-%20Default.aspx", True)
        End If
    End Sub
End Class