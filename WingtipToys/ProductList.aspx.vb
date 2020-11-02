Imports WingtipToys.Models
Imports System.Web.ModelBinding

Partial Public Class ProductList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Function GetProducts(<QueryString("id")> categoryId As Integer?) As IQueryable(Of Product)
        Dim _db = New WingtipToys.Models.ProductContext()
        Dim query As IQueryable(Of Product) = _db.Products
        If categoryId.HasValue AndAlso categoryId > 0 Then
            query = query.Where(Function(p) p.CategoryID = categoryId)
        End If
        Return query
    End Function

End Class