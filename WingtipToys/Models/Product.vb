Imports System.ComponentModel.DataAnnotations
Namespace Models
    Public Class Product
        <ScaffoldColumn(False)>
        Public Property ProductID As Integer
        <Required, StringLength(100), Display(Name:="Name")>
        Public Property ProductName As String
        <Required, StringLength(10000), Display(Name:="Product Description"), DataType(DataType.MultilineText)>
        Public Property Description As String

        Public Property ImagePath As String

        <Display(Name:="Price")>
        Public Property UnitPrice As Double?

        Public Property CategoryID As Integer?

        Public Overridable Property Category As Category

    End Class
End Namespace
