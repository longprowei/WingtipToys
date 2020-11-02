Imports WingtipToys.Models
Imports System.Web.ModelBinding

Partial Public Class ProductDetails
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Function GetProduct(<QueryString("productID")> productId As Integer?) As IQueryable(Of Product)
        Dim _db = New WingtipToys.Models.ProductContext()
        Dim query As IQueryable(Of Product) = _db.Products

        If productId.HasValue AndAlso productId > 0 Then
            query = query.Where(Function(p) p.ProductID = productId)
        Else
            query = Nothing
        End If

        Return query
    End Function

End Class