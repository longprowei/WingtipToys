Imports WingtipToys.Models
Imports WingtipToys.Logic

Namespace Admin
    Public Class AdminPage
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim productAction As String = Request.QueryString("ProductAction")
            If productAction = "add" Then
                LabelAddStatus.Text = "Product added!"
            End If

            If productAction = "remove" Then
                LabelRemoveStatus.Text = "Product removed!"
            End If
        End Sub

        Protected Sub AddProductButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim fileOK As Boolean = False
            Dim path As String = Server.MapPath("~/Catalog/Images/")
            If ProductImage.HasFile Then
                Dim fileExtension As String = System.IO.Path.GetExtension(ProductImage.FileName).ToLower()
                Dim allowedExtensions As String() = {".gif", ".png", ".jpeg", ".jpg"}

                For i As Integer = 0 To allowedExtensions.Length - 1
                    If fileExtension = allowedExtensions(i) Then
                        fileOK = True
                    End If
                Next
            End If

            If fileOK Then
                Try
                    ProductImage.PostedFile.SaveAs(path + ProductImage.FileName)
                    ProductImage.PostedFile.SaveAs(path & "Thumbs/" & ProductImage.FileName)
                Catch ex As Exception
                    LabelAddStatus.Text = ex.Message
                End Try

                Dim products As New AddProducts()
                Dim addSuccess As Boolean = products.AddProduct(AddProductName.Text, AddProductDescription.Text,
                                                                AddProductPrice.Text, DropDownAddCategory.SelectedValue, ProductImage.FileName)
                If addSuccess Then
                    Dim pageUrl As String = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Count() - Request.Url.Query.Count())
                    Response.Redirect(pageUrl + "?ProductAction=add")
                Else
                    LabelAddStatus.Text = "Unable to add new product to database."
                End If
            Else
                LabelAddStatus.Text = "Unable to accept file type."
            End If
        End Sub

        Public Function GetCategories() As IQueryable
            Dim _db As New WingtipToys.Models.ProductContext()
            Dim query As IQueryable = _db.Categories
            Return query
        End Function

        Public Function GetProducts() As IQueryable
            Dim _db As New WingtipToys.Models.ProductContext()
            Dim query As IQueryable = _db.Products
            Return query
        End Function

        Protected Sub RemoveProductButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Using _db As New WingtipToys.Models.ProductContext()
                Dim productId As Integer = Convert.ToInt16(DropDownRemoveProduct.SelectedValue)
                Dim myItem = (From c In _db.Products Where c.ProductID = productId Select c).FirstOrDefault()
                If myItem IsNot Nothing Then
                    _db.Products.Remove(myItem)
                    _db.SaveChanges()

                    Dim pageUrl As String = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Count() - Request.Url.Query.Count())
                    Response.Redirect(pageUrl + "?ProductAction=remove")
                Else
                    LabelRemoveStatus.Text = "Unable to locate product."
                End If
            End Using
        End Sub
    End Class
End Namespace