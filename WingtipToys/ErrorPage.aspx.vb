Imports WingtipToys.Logic

Public Class ErrorPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim generalErrorMsg As String = "A problem has occurred on this web site. Please try again. " +
          "If this error continues, please contact support."
        Dim httpErrorMsg As String = "An HTTP error occurred. Page Not found. Please try again."
        Dim unhandledErrorMsg = "The error was unhandled by application code."

        FriendlyErrorMsg.Text = generalErrorMsg

        Dim errorHandler As String = Request.QueryString("handler")
        If errorHandler Is Nothing Then
            errorHandler = "Error Page"
        End If

        Dim ex As Exception = Server.GetLastError()
        Dim errorMsg As String = Request.QueryString("msg")
        If errorMsg = "404" Then
            ex = New HttpException(404, httpErrorMsg, ex)
            FriendlyErrorMsg.Text = ex.Message
        End If

        If ex Is Nothing Then
            ex = New Exception(unhandledErrorMsg)
        End If

        If Request.IsLocal Then
            ErrorDetailedMsg.Text = ex.Message

            ErrorHandlerLabel.Text = errorHandler

            DetailedErrorPanel.Visible = True

            If ex.InnerException IsNot Nothing Then
                InnerMessage.Text = ex.GetType().ToString() + "<br/>" + ex.InnerException.Message
                InnerTrace.Text = ex.InnerException.StackTrace
            Else
                InnerMessage.Text = ex.GetType().ToString()
                If ex.StackTrace IsNot Nothing Then
                    InnerTrace.Text = ex.StackTrace.ToString().TrimStart()
                End If
            End If
        End If

        ExceptionUtility.LogException(ex, errorHandler)

        Server.ClearError()
    End Sub

End Class