Imports System.IO

Namespace Logic
    Public NotInheritable Class ExceptionUtility
        Private Sub ExceptionUtility()

        End Sub

        Public Shared Sub LogException(exc As Exception, source As String)
            Dim logFile As String = "~/App_Data/ErrorLog.txt"
            logFile = HttpContext.Current.Server.MapPath(logFile)

            Dim sw As New StreamWriter(logFile, True)
            sw.WriteLine("", DateTime.Now)
            sw.WriteLine("********** {0} **********", DateTime.Now)
            If exc.InnerException IsNot Nothing Then
                sw.Write("Inner Exception Type: ")
                sw.WriteLine(exc.InnerException.GetType().ToString())
                sw.Write("Inner Exception: ")
                sw.WriteLine(exc.InnerException.Message)
                sw.Write("Inner Source: ")
                sw.WriteLine(exc.InnerException.Source)
                If exc.InnerException.StackTrace IsNot Nothing Then
                    sw.WriteLine("Inner Stack Trace: ")
                    sw.WriteLine(exc.InnerException.StackTrace)
                End If
            End If

            sw.Write("Exception Type: ")
            sw.WriteLine(exc.GetType().ToString())
            sw.WriteLine("Exception: " + exc.Message)
            sw.WriteLine("Source: " + source)
            sw.WriteLine("Stack Trace: ")
            If exc.StackTrace IsNot Nothing Then
                sw.WriteLine(exc.StackTrace)
                sw.WriteLine()
            End If

            sw.Close()
        End Sub
    End Class
End Namespace
