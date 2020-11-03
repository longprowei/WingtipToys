Imports System.ComponentModel.DataAnnotations

Namespace Models
    Public Class OrderDetail
        Public Property OrderDetailId As Integer
        Public Property OrderId As Integer
        Public Property Username As String
        Public Property ProductId As Integer
        Public Property Quantity As Integer
        Public Property UnitPrice As Double?
    End Class
End Namespace
